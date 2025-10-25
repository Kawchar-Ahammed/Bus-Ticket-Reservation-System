import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BusSchedule, BusScheduleDetail, CreateBusScheduleRequest, UpdateBusScheduleRequest } from '../models/bus-schedule.model';

@Injectable({
  providedIn: 'root'
})
export class BusScheduleService {
  private apiUrl = 'http://localhost:5258/api/admin/busschedules';

  constructor(private http: HttpClient) {}

  getAll(): Observable<BusSchedule[]> {
    return this.http.get<BusSchedule[]>(this.apiUrl);
  }

  getById(id: string): Observable<BusScheduleDetail> {
    return this.http.get<BusScheduleDetail>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateBusScheduleRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, request);
  }

  update(id: string, request: UpdateBusScheduleRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
