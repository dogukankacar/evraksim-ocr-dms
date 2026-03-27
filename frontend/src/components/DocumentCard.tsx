"use client";

import Link from "next/link";
import { DocumentDto } from "@/types";

function formatFileSize(bytes: number): string {
  if (bytes < 1024) return bytes + " B";
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + " KB";
  return (bytes / (1024 * 1024)).toFixed(1) + " MB";
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString("tr-TR", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

export default function DocumentCard({ doc }: { doc: DocumentDto }) {
  return (
    <Link href={`/documents/${doc.id}`}>
      <div className="bg-white rounded-lg shadow-md p-5 hover:shadow-lg transition cursor-pointer border border-gray-100">
        <div className="flex justify-between items-start mb-3">
          <h3 className="text-lg font-semibold text-gray-800 truncate">
            {doc.title}
          </h3>
          <span className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded whitespace-nowrap ml-2">
            {doc.categoryName}
          </span>
        </div>
        {doc.description && (
          <p className="text-sm text-gray-600 mb-3 line-clamp-2">
            {doc.description}
          </p>
        )}
        <div className="flex items-center justify-between text-xs text-gray-500">
          <div className="flex items-center space-x-3">
            <span>{doc.originalFileName}</span>
            <span>{formatFileSize(doc.fileSize)}</span>
            <span>v{doc.currentVersion}</span>
          </div>
          <span>{formatDate(doc.createdAt)}</span>
        </div>
        <div className="mt-2 text-xs text-gray-400">
          Yükleyen: {doc.uploadedByName}
        </div>
      </div>
    </Link>
  );
}
