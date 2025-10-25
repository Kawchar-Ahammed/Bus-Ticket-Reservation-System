import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import {
  LoginRequest,
  LoginResponse,
  RefreshTokenRequest,
  TokenResponse,
  AdminUser
} from '../models/admin-auth.model';

const ADMIN_USER_KEY = 'admin_user';
const ACCESS_TOKEN_KEY = 'admin_access_token';
const REFRESH_TOKEN_KEY = 'admin_refresh_token';

@Injectable({
  providedIn: 'root'
})
export class AdminAuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  
  private apiUrl = 'http://localhost:5258/api/admin/auth';
  
  private currentUserSubject = new BehaviorSubject<AdminUser | null>(this.getUserFromStorage());
  public currentUser$ = this.currentUserSubject.asObservable();
  
  constructor() {}
  
  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        this.storeAuthData(response);
        const user: AdminUser = {
          userId: response.userId,
          email: response.email,
          name: response.fullName,
          role: response.role
        };
        this.currentUserSubject.next(user);
      })
    );
  }
  
  logout(): void {
    const accessToken = this.getAccessToken();
    if (accessToken) {
      // Call logout endpoint to clear refresh token on server
      this.http.post(`${this.apiUrl}/logout`, {}).subscribe({
        complete: () => {
          this.clearAuthData();
          this.currentUserSubject.next(null);
          this.router.navigate(['/admin/login']);
        },
        error: () => {
          // Clear local data even if server call fails
          this.clearAuthData();
          this.currentUserSubject.next(null);
          this.router.navigate(['/admin/login']);
        }
      });
    } else {
      this.clearAuthData();
      this.currentUserSubject.next(null);
      this.router.navigate(['/admin/login']);
    }
  }
  
  refreshToken(): Observable<TokenResponse> {
    const accessToken = this.getAccessToken();
    const refreshToken = this.getRefreshToken();
    
    if (!accessToken || !refreshToken) {
      throw new Error('No tokens available');
    }
    
    const request: RefreshTokenRequest = { accessToken, refreshToken };
    
    return this.http.post<TokenResponse>(`${this.apiUrl}/refresh-token`, request).pipe(
      tap(response => {
        this.storeTokens(response.accessToken, response.refreshToken);
      })
    );
  }
  
  getCurrentUser(): Observable<AdminUser> {
    return this.http.get<AdminUser>(`${this.apiUrl}/me`);
  }
  
  isAuthenticated(): boolean {
    const token = this.getAccessToken();
    if (!token) return false;
    
    // Check if token is expired
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiry = payload.exp * 1000; // Convert to milliseconds
      return Date.now() < expiry;
    } catch {
      return false;
    }
  }
  
  getAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }
  
  getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }
  
  getCurrentUserValue(): AdminUser | null {
    return this.currentUserSubject.value;
  }
  
  hasRole(roles: string[]): boolean {
    const user = this.getCurrentUserValue();
    return user ? roles.includes(user.role) : false;
  }
  
  private storeAuthData(response: LoginResponse): void {
    const user: AdminUser = {
      userId: response.userId,
      email: response.email,
      name: response.fullName,
      role: response.role
    };
    
    localStorage.setItem(ADMIN_USER_KEY, JSON.stringify(user));
    localStorage.setItem(ACCESS_TOKEN_KEY, response.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, response.refreshToken);
  }
  
  private storeTokens(accessToken: string, refreshToken: string): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
  }
  
  private clearAuthData(): void {
    localStorage.removeItem(ADMIN_USER_KEY);
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
  }
  
  private getUserFromStorage(): AdminUser | null {
    const userStr = localStorage.getItem(ADMIN_USER_KEY);
    if (userStr) {
      try {
        return JSON.parse(userStr);
      } catch {
        return null;
      }
    }
    return null;
  }
}
