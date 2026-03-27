"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import api from "@/lib/api";
import ProtectedRoute from "@/components/ProtectedRoute";
import { isAdmin } from "@/lib/auth";
import { ApiResponse, PagedResult, AuditLogDto } from "@/types";

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString("tr-TR", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function actionColor(action: string): string {
  switch (action) {
    case "Upload":
      return "bg-green-100 text-green-800";
    case "Download":
      return "bg-blue-100 text-blue-800";
    case "Delete":
      return "bg-red-100 text-red-800";
    case "Update":
      return "bg-yellow-100 text-yellow-800";
    case "View":
      return "bg-gray-100 text-gray-800";
    default:
      return "bg-gray-100 text-gray-800";
  }
}

export default function LogsPage() {
  const router = useRouter();
  const [logs, setLogs] = useState<AuditLogDto[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!isAdmin()) {
      router.push("/");
      return;
    }
    loadLogs();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, router]);

  const loadLogs = async () => {
    setLoading(true);
    try {
      const res = await api.get<ApiResponse<PagedResult<AuditLogDto>>>(
        `/logs?pageNumber=${page}&pageSize=20`
      );
      setLogs(res.data.data?.items ?? []);
      setTotalPages(res.data.data?.totalPages ?? 1);
    } catch {
      /* handled */
    } finally {
      setLoading(false);
    }
  };

  return (
    <ProtectedRoute>
      <div className="max-w-5xl mx-auto px-4 py-8">
        <h1 className="text-2xl font-bold text-gray-800 mb-6">
          Denetim Logları
        </h1>

        {loading ? (
          <div className="text-center py-12">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          </div>
        ) : logs.length === 0 ? (
          <p className="text-gray-500 text-center py-12">Log kaydı yok.</p>
        ) : (
          <>
            <div className="bg-white rounded-lg shadow-md overflow-hidden">
              <table className="w-full text-sm">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-4 py-3 text-left text-gray-600">
                      Tarih
                    </th>
                    <th className="px-4 py-3 text-left text-gray-600">
                      Kullanıcı
                    </th>
                    <th className="px-4 py-3 text-left text-gray-600">
                      İşlem
                    </th>
                    <th className="px-4 py-3 text-left text-gray-600">
                      Varlık
                    </th>
                    <th className="px-4 py-3 text-left text-gray-600">
                      Detay
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100">
                  {logs.map((log) => (
                    <tr key={log.id} className="hover:bg-gray-50">
                      <td className="px-4 py-3 text-gray-600">
                        {formatDate(log.timestamp)}
                      </td>
                      <td className="px-4 py-3 text-gray-700">
                        {log.userName}
                      </td>
                      <td className="px-4 py-3">
                        <span
                          className={`px-2 py-1 rounded text-xs font-medium ${actionColor(log.action)}`}
                        >
                          {log.action}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-gray-600">
                        {log.entityType} #{log.entityId}
                      </td>
                      <td className="px-4 py-3 text-gray-500 truncate max-w-xs">
                        {log.details ?? "-"}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {totalPages > 1 && (
              <div className="flex justify-center items-center gap-4 mt-6">
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
