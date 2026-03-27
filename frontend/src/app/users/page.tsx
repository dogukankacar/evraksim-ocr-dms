"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import api from "@/lib/api";
import ProtectedRoute from "@/components/ProtectedRoute";
import { isAdmin, getUser } from "@/lib/auth";
import { ApiResponse, UserDto } from "@/types";

export default function UsersPage() {
  const router = useRouter();
  const [users, setUsers] = useState<UserDto[]>([]);
  const [loading, setLoading] = useState(true);
  const currentUser = getUser();

  useEffect(() => {
    if (!isAdmin()) {
      router.push("/");
      return;
    }
    loadUsers();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [router]);

  const loadUsers = async () => {
    setLoading(true);
    try {
      const res = await api.get<ApiResponse<UserDto[]>>("/users");
      setUsers(res.data.data ?? []);
    } catch {
      alert("Kullanıcılar yüklenirken hata oluştu.");
    } finally {
      setLoading(false);
    }
  };

  const toggleAdmin = async (userId: string, currentEmail: string) => {
    if (currentEmail === "admin@dms.com") {
      alert("Ana admin hesabı değiştirilemez.");
      return;
    }
    
    try {
      await api.post(`/users/${userId}/toggle-admin`);
      loadUsers();
    } catch {
      alert("Yetki değiştirilemedi.");
    }
  };

  return (
    <ProtectedRoute>
      <div className="max-w-5xl mx-auto px-4 py-8">
        <h1 className="text-2xl font-bold text-gray-800 mb-6">Kullanıcı Yönetimi</h1>

        {loading ? (
          <div className="text-center py-12">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          </div>
        ) : (
          <div className="bg-white rounded-lg shadow-md overflow-hidden">
            <table className="w-full text-sm text-left">
              <thead className="bg-gray-50 text-gray-600">
                <tr>
                  <th className="px-6 py-3">Ad Soyad</th>
                  <th className="px-6 py-3">E-posta</th>
                  <th className="px-6 py-3">Roller</th>
                  <th className="px-6 py-3 text-right">İşlem</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {users.map((u) => {
                  const hasAdmin = u.roles?.includes("Admin");
                  const isSuperAdmin = u.email === "admin@dms.com";
                  const isSelf = u.id === currentUser?.id;

                  return (
                    <tr key={u.id} className="hover:bg-gray-50">
                      <td className="px-6 py-4 font-medium text-gray-800">{u.fullName}</td>
                      <td className="px-6 py-4 text-gray-600">{u.email}</td>
                      <td className="px-6 py-4">
                        <div className="flex gap-2">
                          {u.roles?.map(r => (
                            <span key={r} className={`px-2 py-1 rounded text-xs font-medium ${r === 'Admin' ? 'bg-blue-100 text-blue-800' : 'bg-gray-100 text-gray-800'}`}>
                              {r}
                           </span>
                          ))}
                        </div>
                      </td>
                      <td className="px-6 py-4 text-right">
                        {!isSuperAdmin && !isSelf && (
                          <button
                            onClick={() => toggleAdmin(u.id, u.email)}
                            className={`px-3 py-1.5 rounded text-xs font-medium transition ${
                              hasAdmin 
                                ? "bg-red-50 text-red-600 hover:bg-red-100 border border-red-200" 
                                : "bg-blue-50 text-blue-600 hover:bg-blue-100 border border-blue-200"
                            }`}
                          >
                            {hasAdmin ? "Yetkiyi Al" : "Admin Yap"}
                          </button>
                        )}
                        {isSuperAdmin && <span className="text-xs text-gray-400 italic">Değiştirilemez</span>}
                        {isSelf && !isSuperAdmin && <span className="text-xs text-gray-400 italic">Kendiniz</span>}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </ProtectedRoute>
  );
}
