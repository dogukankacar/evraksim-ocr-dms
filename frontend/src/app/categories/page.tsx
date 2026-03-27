"use client";

import { useEffect, useState } from "react";
import api from "@/lib/api";
import ProtectedRoute from "@/components/ProtectedRoute";
import { isAdmin } from "@/lib/auth";
import { ApiResponse, CategoryDto } from "@/types";

export default function CategoriesPage() {
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [editId, setEditId] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    loadCategories();
  }, []);

  const loadCategories = async () => {
    try {
      const res = await api.get<ApiResponse<CategoryDto[]>>("/categories");
      setCategories(res.data.data ?? []);
    } catch {
      /* handled */
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    try {
      if (editId) {
        await api.put(`/categories/${editId}`, { name, description });
      } else {
        await api.post("/categories", { name, description });
      }
      setName("");
      setDescription("");
      setEditId(null);
      loadCategories();
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: ApiResponse<unknown> } };
      setError(axiosErr.response?.data?.message ?? "İşlem başarısız.");
    }
  };

  const handleEdit = (cat: CategoryDto) => {
    setEditId(cat.id);
    setName(cat.name);
    setDescription(cat.description ?? "");
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Bu kategoriyi silmek istediğinize emin misiniz?")) return;
    try {
      await api.delete(`/categories/${id}`);
      loadCategories();
    } catch {
      alert("Silme hatası.");
    }
  };

  return (
    <ProtectedRoute>
      <div className="max-w-3xl mx-auto px-4 py-8">
        <h1 className="text-2xl font-bold text-gray-800 mb-6">Kategoriler</h1>

        {isAdmin() && (
          <div className="bg-white rounded-lg shadow-md p-5 mb-6">
            <h2 className="text-lg font-semibold text-gray-700 mb-3">
              {editId ? "Kategori Düzenle" : "Yeni Kategori"}
            </h2>
            {error && (
              <div className="bg-red-50 text-red-600 p-3 rounded mb-3 text-sm">
                {error}
              </div>
            )}
            <form onSubmit={handleSubmit} className="space-y-3">
              <input
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
                placeholder="Kategori adı"
                required
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <input
                type="text"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Açıklama (opsiyonel)"
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <div className="flex gap-2">
                <button
                  type="submit"
                  className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
                >
                  {editId ? "Güncelle" : "Ekle"}
                </button>
                {editId && (
                  <button
                    type="button"
                    onClick={() => {
                      setEditId(null);
                      setName("");
                      setDescription("");
                    }}
                    className="px-6 py-2 bg-gray-300 text-gray-700 rounded-lg hover:bg-gray-400 transition"
                  >
                    İptal
                  </button>
                )}
              </div>
            </form>
          </div>
        )}

        {loading ? (
          <div className="text-center py-12">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          </div>
        ) : (
          <div className="space-y-3">
            {categories.map((cat) => (
              <div
                key={cat.id}
                className="bg-white rounded-lg shadow-sm p-4 flex items-center justify-between border"
              >
                <div>
                  <h3 className="font-medium text-gray-800">{cat.name}</h3>
                  {cat.description && (
                    <p className="text-sm text-gray-500">{cat.description}</p>
                  )}
                </div>
                {isAdmin() && (
                  <div className="flex gap-2">
                    <button
                      onClick={() => handleEdit(cat)}
                      className="text-sm text-blue-600 hover:text-blue-800"
                    >
                      Düzenle
                    </button>
                    <button
                      onClick={() => handleDelete(cat.id)}
                      className="text-sm text-red-600 hover:text-red-800"
                    >
                      Sil
                    </button>
                  </div>
                )}
              </div>
            ))}
          </div>
        )}
      </div>
    </ProtectedRoute>
  );
}
