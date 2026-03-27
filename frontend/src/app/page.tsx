"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { isAuthenticated } from "@/lib/auth";
import api from "@/lib/api";
import ProtectedRoute from "@/components/ProtectedRoute";
import DocumentCard from "@/components/DocumentCard";
import { ApiResponse, PagedResult, DocumentDto, CategoryDto } from "@/types";

export default function Dashboard() {
  const router = useRouter();
  const [documents, setDocuments] = useState<DocumentDto[]>([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!isAuthenticated()) {
      router.push("/login");
      return;
    }
    loadData();
  }, [router]);

  const loadData = async () => {
    try {
      const [docsRes, catsRes] = await Promise.all([
        api.get<ApiResponse<PagedResult<DocumentDto>>>("/documents?pageSize=6"),
        api.get<ApiResponse<CategoryDto[]>>("/categories"),
      ]);
      setDocuments(docsRes.data.data?.items ?? []);
      setTotalCount(docsRes.data.data?.totalCount ?? 0);
      setCategories(catsRes.data.data ?? []);
    } catch {
      // handled by interceptor
    } finally {
      setLoading(false);
    }
  };

  return (
    <ProtectedRoute>
      <div className="max-w-7xl mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold text-gray-800 mb-8">Dashboard</h1>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          <div className="bg-white rounded-lg shadow p-6">
            <h3 className="text-sm text-gray-500 uppercase">Toplam Evrak</h3>
            <p className="text-3xl font-bold text-blue-600 mt-2">
              {totalCount}
            </p>
          </div>
          <div className="bg-white rounded-lg shadow p-6">
            <h3 className="text-sm text-gray-500 uppercase">Kategoriler</h3>
            <p className="text-3xl font-bold text-green-600 mt-2">
              {categories.length}
            </p>
          </div>
          <div className="bg-white rounded-lg shadow p-6">
            <h3 className="text-sm text-gray-500 uppercase">Hızlı İşlem</h3>
            <button
              onClick={() => router.push("/documents/upload")}
              className="mt-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
            >
              Yeni Evrak Yükle
            </button>
          </div>
        </div>

        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-semibold text-gray-700">Son Evraklar</h2>
          <button
            onClick={() => router.push("/documents")}
            className="text-blue-600 hover:text-blue-800 text-sm"
          >
            Tümünü Gör
          </button>
        </div>

        {loading ? (
          <div className="text-center py-12">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          </div>
        ) : documents.length === 0 ? (
          <p className="text-gray-500 text-center py-12">
            Henüz evrak bulunmuyor.
          </p>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {documents.map((doc) => (
              <DocumentCard key={doc.id} doc={doc} />
            ))}
          </div>
        )}
      </div>
    </ProtectedRoute>
  );
}
