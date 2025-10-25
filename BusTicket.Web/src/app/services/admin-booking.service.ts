import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminBooking, AdminBookingDetail, BookingFilters } from '../models/admin-booking.model';

@Injectable({
  providedIn: 'root'
})
export class AdminBookingService {
  private apiUrl = 'http://localhost:5258/api/admin/bookings';

  constructor(private http: HttpClient) { }

  getAll(filters?: BookingFilters): Observable<AdminBooking[]> {
    let params = new HttpParams();
    
    if (filters) {
      if (filters.companyId) {
        params = params.set('companyId', filters.companyId);
      }
      if (filters.bookingStatus) {
        params = params.set('bookingStatus', filters.bookingStatus);
      }
      if (filters.journeyDateFrom) {
        params = params.set('journeyDateFrom', filters.journeyDateFrom);
      }
      if (filters.journeyDateTo) {
        params = params.set('journeyDateTo', filters.journeyDateTo);
      }
      if (filters.bookingDateFrom) {
        params = params.set('bookingDateFrom', filters.bookingDateFrom);
      }
      if (filters.bookingDateTo) {
        params = params.set('bookingDateTo', filters.bookingDateTo);
      }
      if (filters.searchTerm) {
        params = params.set('searchTerm', filters.searchTerm);
      }
    }
    
    return this.http.get<AdminBooking[]>(this.apiUrl, { params });
  }

  getById(id: string): Observable<AdminBookingDetail> {
    return this.http.get<AdminBookingDetail>(`${this.apiUrl}/${id}`);
  }

  getByTicketNumber(ticketNumber: string): Observable<AdminBookingDetail> {
    return this.http.get<AdminBookingDetail>(`${this.apiUrl}/ticket/${ticketNumber}`);
  }
}
