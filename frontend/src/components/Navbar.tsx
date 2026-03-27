"use client";

import Link from "next/link";
import { useRouter, usePathname } from "next/navigation";
import { useEffect, useState } from "react";
import { clearAuth, getUser, isAdmin, isAuthenticated } from "@/lib/auth";
import { UserDto } from "@/types";

export default function Navbar() {
  const router = useRouter();
  const pathname = usePathname();
  const [user, setUser] = useState<UserDto | null>(null);
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setMounted(true);
    setUser(getUser());
  }, [pathname]);

  if (!mounted) return null;

  if (pathname === "/login" || pathname === "/register") return null;

  const handleLogout = () => {
    clearAuth();
    setUser(null);
    router.push("/login");
  };

  if (!isAuthenticated() && !user) return null;

  return (
    <nav className="bg-white shadow-md">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16 items-center">
          <div className="flex items-center space-x-8">
            <Link href="/" className="text-xl font-bold text-blue-600">
              DMS
            </Link>
            <Link
              href="/documents"
              className="text-gray-700 hover:text-blue-600 transition"
            >
              Evraklar
            </Link>
            <Link
              href="/documents/upload"
              className="text-gray-700 hover:text-blue-600 transition"
            >
              Yükle
            </Link>
            <Link
              href="/categories"
              className="text-gray-700 hover:text-blue-600 transition"
            >
              Kategoriler
            </Link>
            {isAdmin() && (
              <>
                <Link
                  href="/logs"
                  className="text-gray-700 hover:text-blue-600 transition"
                >
                  Loglar
                </Link>
                <Link
                  href="/users"
                  className="text-gray-700 hover:text-blue-600 transition"
                >
                  Kullanıcılar
                </Link>
              </>
            )}
          </div>
          <div className="flex items-center space-x-4">
            <span className="text-sm text-gray-600">
              {user?.fullName}
              {isAdmin() && (
                <span className="ml-1 text-xs bg-blue-100 text-blue-800 px-2 py-0.5 rounded">
                  Admin
                </span>
              )}
            </span>
            <button
              onClick={handleLogout}
              className="text-sm text-red-600 hover:text-red-800 transition"
            >
              Çıkış
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
}
