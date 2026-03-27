"use client";

import { useEffect, useState } from "react";
import api from "@/lib/api";
import ProtectedRoute from "@/components/ProtectedRoute";
import DocumentCard from "@/components/DocumentCard";
import SearchBar from "@/components/SearchBar";
import {
  ApiResponse,
  PagedResult,
  DocumentDto,
  CategoryDto,
} from "@/types";

export default function DocumentsPage() {
  const [documents, setDocuments] = useState<DocumentDto[]>([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [selectedCategory, setSelectedCategory] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadCategories();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    loadDocuments();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, selectedCategory]);

  const loadCategories = async () => {
    try {
      const res = await api.get<ApiResponse<CategoryDto[]>>("/categories");
      setCategories(res.data.data ?? []);
    } catch {
      /* handled */
    }
  };

  const loadDocuments = async () => {
    setLoading(true);
    try {
      let url = `/documents?pageNumber=${page}&pageSize=12`;
      if (selectedCategory) url += `&categoryId=${selectedCategory}`;
      const res =
        await api.get<ApiResponse<PagedResult<DocumentDto>>>(url);
      setDocuments(res.data.data?.items ?? []);
      setTotalPages(res.data.data?.totalPages ?? 1);
    } catch {
      /* handled */
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async (query: string) => {
    if (!query.trim()) {
      loadDocuments();
      return;
    }
    setLoading(true);
    try {
      const res = await api.get<ApiResponse<DocumentDto[]>>(
        `/documents/search?q=${encodeURIComponent(query)}`
      );
      setDocuments(res.data.data ?? []);
      setTotalPages(1);
    } catch {
      /* handled */
    } finally {
      setLoading(false);
    }
  };

  return (
    <ProtectedRoute>
      <div className="max-w-7xl mx-auto px-4 py-8">
        <h1 className="text-2xl font-bold text-gray-800 mb-6">Evraklar</h1>

        <div className="mb-6">
          <SearchBar onSearch={handleSearch} />
        </div>

        <div className="flex gap-2 mb-6 flex-wrap">
          <button
            onClick={() => {
              setSelectedCategory(null);
              setPage(1);
            }}
            className={`px-4 py-1.5 rounded-full text-sm transition ${
              !selectedCategory
                ? "bg-blue-600 text-white"
                : "bg-gray-200 text-gray-700 hover:bg-gray-300"
            }`}
          >
            Tümü
          </button>
          {categories.map((cat) => (
            <button
              key={cat.id}
              onClick={() => {
                setSelectedCategory(cat.id);
                setPage(1);
              }}
              className={`px-4 py-1.5 rounded-full text-sm transition ${
                selectedCategory === cat.id
                  ? "bg-blue-600 text-white"
                  : "bg-gray-200 text-gray-700 hover:bg-gray-300"
              }`}
            >
              {cat.name}
            </button>
          ))}
        </div>

        {loading ? (
          <div className="text-center py-12">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          </div>
        ) : documents.length === 0 ? (
          <p className="text-gray-500 text-center py-12">Evrak bulunamadı.</p>
        ) : (
          <>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {documents.map((doc) => (
                <DocumentCard key={doc.id} doc={doc} />
              ))}
            </div>

            {totalPages > 1 && (
              <div className="flex justify-center items-center gap-4 mt-8">
                <button
                  onClick={() => setPage((p) => Math.max(1, p - 1))}
                  disabled={page === 1}
                  className="px-4 py-2 bg-gray-200 rounded-lg disabled:opacity-50 hover:bg-gray-300 transition"
                >
                  Önceki
                </button>
                <span className="text-sm text-gray-600">
                  {page} / {totalPages}
                </span>
                <button
                  onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                  disabled={page === totalPages}
                  className="px-4 py-2 bg-gray-200 rounded-lg disabled:opacity-50 hover:bg-gray-300 transition"
                >
                  Sonraki
                </button>
              </div>
            )}
          </>
        )}
      </div>
    </ProtectedRoute>
  );
}
