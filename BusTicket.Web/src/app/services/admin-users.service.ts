import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminUser, AdminRole, CreateAdminUserDto, UpdateAdminUserDto, ChangeAdminUserRoleDto } from '../models/admin-user.model';

@Injectable({
  providedIn: 'root'
})
export class AdminUsersService {
  private apiUrl = 'http://localhost:5258/api/admin/users';

  constructor(private http: HttpClient) {}

  getAll(role?: AdminRole, isActive?: boolean): Observable<AdminUser[]> {
    let params = new HttpParams();
    if (role !== undefined && role !== null) {
      params = params.set('role', role.toString());
    }
    if (isActive !== undefined && isActive !== null) {
      params = params.set('isActive', isActive.toString());
    }
    return this.http.get<AdminUser[]>(this.apiUrl, { params });
  }

  getById(id: string): Observable<AdminUser> {
    return this.http.get<AdminUser>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateAdminUserDto): Observable<string> {
    return this.http.post<string>(this.apiUrl, dto);
  }

  update(dto: UpdateAdminUserDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${dto.id}`, dto);
  }

  changeRole(dto: ChangeAdminUserRoleDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${dto.id}/role`, dto);
  }

  deactivate(id: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/deactivate`, {});
  }

  activate(id: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/activate`, {});
  }
}
