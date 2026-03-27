"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import api from "@/lib/api";
import ProtectedRoute from "@/components/ProtectedRoute";
import VersionHistory from "@/components/VersionHistory";
import { isAdmin } from "@/lib/auth";
import {
  ApiResponse,
  DocumentDto,
  DocumentVersionDto,
} from "@/types";

function formatFileSize(bytes: number): string {
  if (bytes < 1024) return bytes + " B";
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + " KB";
  return (bytes / (1024 * 1024)).toFixed(1) + " MB";
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString("tr-TR", {
    year: "numeric",
    month: "long",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

export default function DocumentDetailPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id as string;
  const [doc, setDoc] = useState<DocumentDto | null>(null);
  const [versions, setVersions] = useState<DocumentVersionDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (id) loadDocument();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]);

  const loadDocument = async () => {
    try {
      const [docRes, versRes] = await Promise.all([
        api.get<ApiResponse<DocumentDto>>(`/documents/${id}`),
        api.get<ApiResponse<DocumentVersionDto[]>>(
          `/documents/${id}/versions`
        ),
      ]);
      setDoc(docRes.data.data ?? null);
      setVersions(versRes.data.data ?? []);
    } catch {
      /* handled */
    } finally {
      setLoading(false);
    }
  };

  const handleDownload = async () => {
    try {
      const response = await api.get(`/documents/${id}/download`, {
        responseType: "blob",
      });
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const a = document.createElement("a");
      a.href = url;
      a.download = doc?.originalFileName ?? "download";
      a.click();
      window.URL.revokeObjectURL(url);
    } catch {
      alert("İndirme hatası.");
    }
  };

  const handleDelete = async () => {
    if (!confirm("Bu evrakı silmek istediğinize emin misiniz?")) return;
    try {
      await api.delete(`/documents/${id}`);
      router.push("/documents");
    } catch {
      alert("Silme hatası.");
    }
  };

  if (loading) {
    return (
      <ProtectedRoute>
        <div className="flex items-center justify-center py-20">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>
      </ProtectedRoute>
    );
  }

  if (!doc) {
    return (
      <ProtectedRoute>
        <div className="max-w-3xl mx-auto px-4 py-8">
          <p className="text-gray-500">Evrak bulunamadı.</p>
        </div>
      </ProtectedRoute>
    );
  }

  return (
    <ProtectedRoute>
      <div className="max-w-3xl mx-auto px-4 py-8">
        <div className="bg-white rounded-lg shadow-md p-6">
          <div className="flex justify-between items-start mb-4">
            <div>
              <h1 className="text-2xl font-bold text-gray-800">{doc.title}</h1>
              <span className="inline-block mt-1 text-sm bg-blue-100 text-blue-800 px-3 py-1 rounded">
                {doc.categoryName}
              </span>
            </div>
            <div className="flex gap-2">
              <button
                onClick={handleDownload}
                className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition text-sm"
              >
                İndir
              </button>
              {isAdmin() && (
                <button
                  onClick={handleDelete}
                  className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition text-sm"
                >
                  Sil
                </button>
              )}
            </div>
          </div>

          {doc.description && (
            <p className="text-gray-600 mb-4">{doc.description}</p>
          )}

          <div className="grid grid-cols-2 gap-4 text-sm text-gray-600 mb-6">
            <div>
              <span className="font-medium">Dosya:</span>{" "}
              {doc.originalFileName}
            </div>
            <div>
              <span className="font-medium">Boyut:</span>{" "}
              {formatFileSize(doc.fileSize)}
            </div>
            <div>
              <span className="font-medium">Versiyon:</span>{" "}
              {doc.currentVersion}
            </div>
            <div>
              <span className="font-medium">Tür:</span> {doc.contentType}
            </div>
            <div>
              <span className="font-medium">Yükleyen:</span>{" "}
              {doc.uploadedByName}
            </div>
            <div>
              <span className="font-medium">Tarih:</span>{" "}
              {formatDate(doc.createdAt)}
            </div>
          </div>

          {doc.ocrContent && (
            <div className="mb-6">
              <h2 className="text-lg font-semibold text-gray-700 mb-2">
                OCR İçerik
              </h2>
              <div className="bg-gray-50 p-4 rounded-lg text-sm text-gray-600 max-h-48 overflow-y-auto">
                {doc.ocrContent}
              </div>
            </div>
          )}

          <div>
            <h2 className="text-lg font-semibold text-gray-700 mb-3">
              Versiyon Geçmişi
            </h2>
            <VersionHistory documentId={doc.id} versions={versions} />
          </div>
        </div>
      </div>
    </ProtectedRoute>
  );
}
