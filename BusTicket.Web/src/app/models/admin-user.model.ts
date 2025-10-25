export interface AdminUser {
  id: string;
  email: string;
  fullName: string;
  role: AdminRole;
  roleName: string;
  isActive: boolean;
  lastLoginDate?: Date;
  createdAt: Date;
  updatedAt?: Date;
}

export enum AdminRole {
  SuperAdmin = 0,
  Admin = 1,
  Operator = 2
}

export interface CreateAdminUserDto {
  email: string;
  password: string;
  fullName: string;
  role: AdminRole;
}

export interface UpdateAdminUserDto {
  id: string;
  fullName: string;
}

export interface ChangeAdminUserRoleDto {
  id: string;
  newRole: AdminRole;
}
