import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Company, CompanyDetail, CreateCompanyRequest, UpdateCompanyRequest } from '../models/company.model';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5258/api/admin/companies';

  getAll(): Observable<Company[]> {
    return this.http.get<Company[]>(this.apiUrl);
  }

  getById(id: string): Observable<CompanyDetail> {
    return this.http.get<CompanyDetail>(`${this.apiUrl}/${id}`);
  }

  create(company: CreateCompanyRequest): Observable<{id: string}> {
    return this.http.post<{id: string}>(this.apiUrl, company);
  }

  update(id: string, company: UpdateCompanyRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, company);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
