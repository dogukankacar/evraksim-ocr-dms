export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: string[];
}

export interface PagedResult<T> {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  items: T[];
}

export interface DocumentDto {
  id: number;
  title: string;
  description?: string;
  originalFileName: string;
  contentType: string;
  fileSize: number;
  ocrContent?: string;
  categoryId: number;
  categoryName: string;
  uploadedByUserId: string;
  uploadedByName: string;
  currentVersion: number;
  createdAt: string;
  updatedAt: string;
}

export interface DocumentVersionDto {
  id: number;
  documentId: number;
  versionNumber: number;
  fileSize: number;
  ocrContent?: string;
  uploadedByName: string;
  createdAt: string;
}

export interface CategoryDto {
  id: number;
  name: string;
  description?: string;
}

export interface AuditLogDto {
  id: number;
  userId: string;
  userName: string;
  action: string;
  entityType: string;
  entityId: number;
  details?: string;
  timestamp: string;
}

export interface AuthResponseDto {
  token: string;
  email: string;
  fullName: string;
  roles: string[];
}

export interface UserDto {
  id: string;
  email: string;
  fullName: string;
  roles: string[];
}
