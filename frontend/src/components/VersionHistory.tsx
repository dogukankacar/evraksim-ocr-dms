"use client";

import { DocumentVersionDto } from "@/types";
import api from "@/lib/api";

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString("tr-TR", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function formatFileSize(bytes: number): string {
  if (bytes < 1024) return bytes + " B";
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + " KB";
  return (bytes / (1024 * 1024)).toFixed(1) + " MB";
}

interface VersionHistoryProps {
  documentId: number;
  versions: DocumentVersionDto[];
}

export default function VersionHistory({
  documentId,
  versions,
}: VersionHistoryProps) {
  const handleDownload = async (versionId: number) => {
    try {
      const response = await api.get(
        `/documents/${documentId}/versions/${versionId}/download`,
        { responseType: "blob" }
      );
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const a = document.createElement("a");
      a.href = url;
      a.download = `version-${versionId}`;
      a.click();
      window.URL.revokeObjectURL(url);
    } catch {
      alert("İndirme hatası.");
    }
  };

  if (versions.length === 0) {
    return (
      <p className="text-gray-500 text-sm">Henüz eski versiyon bulunmuyor.</p>
    );
  }

  return (
    <div className="space-y-3">
      {versions.map((v) => (
        <div
          key={v.id}
          className="flex items-center justify-between bg-gray-50 rounded-lg p-3 border"
        >
          <div>
            <span className="font-medium text-gray-700">
              Versiyon {v.versionNumber}
            </span>
            <span className="text-sm text-gray-500 ml-3">
              {formatFileSize(v.fileSize)}
            </span>
            <span className="text-sm text-gray-500 ml-3">
              {formatDate(v.createdAt)}
            </span>
            <span className="text-sm text-gray-400 ml-3">
              {v.uploadedByName}
            </span>
          </div>
          <button
            onClick={() => handleDownload(v.id)}
            className="text-sm text-blue-600 hover:text-blue-800"
          >
            İndir
          </button>
        </div>
      ))}
    </div>
  );
}
