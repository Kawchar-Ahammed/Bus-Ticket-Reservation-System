import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BusScheduleDto, SeatDto, BookingRequest, BookingResponse, SearchCriteria } from '../models/bus.models';

@Injectable({
  providedIn: 'root'
})
export class BusApi {
  private apiUrl = 'http://localhost:5258/api';

  constructor(private http: HttpClient) { }

  searchBuses(criteria: SearchCriteria): Observable<BusScheduleDto[]> {
    const params = new HttpParams()
      .set('from', criteria.fromCity)
      .set('to', criteria.toCity)
      .set('journeyDate', criteria.journeyDate);
    
    return this.http.get<BusScheduleDto[]>(`${this.apiUrl}/BusSearch/available`, { params });
  }

  getSeatPlan(scheduleId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/Booking/seat-plan/${scheduleId}`);
  }

  bookSeats(booking: BookingRequest): Observable<BookingResponse> {
    return this.http.post<BookingResponse>(`${this.apiUrl}/Booking/book-seat`, booking);
  }

  // Booking History APIs
  getBookingsByPhone(phoneNumber: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/BookingHistory/by-phone/${phoneNumber}`);
  }

  getBookingByTicket(ticketNumber: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/BookingHistory/by-ticket/${ticketNumber}`);
  }

  cancelBooking(ticketNumber: string, phoneNumber: string, reason: string = ''): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/BookingHistory/cancel`, {
      ticketNumber,
      phoneNumber,
      cancellationReason: reason
    });
  }
}


