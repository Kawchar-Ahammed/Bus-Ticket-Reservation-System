import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Bus, BusDetail, CreateBusRequest, UpdateBusRequest } from '../models/bus.model';

@Injectable({
  providedIn: 'root'
})
export class BusService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5258/api/admin/buses';

  getAll(): Observable<Bus[]> {
    return this.http.get<Bus[]>(this.apiUrl);
  }

  getById(id: string): Observable<BusDetail> {
    return this.http.get<BusDetail>(`${this.apiUrl}/${id}`);
  }

  create(bus: CreateBusRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, bus);
  }

  update(id: string, bus: UpdateBusRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, bus);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
