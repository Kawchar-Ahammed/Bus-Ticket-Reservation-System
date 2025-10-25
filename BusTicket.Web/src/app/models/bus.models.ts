// DTOs matching the .NET backend API responses

export interface BusScheduleDto {
  busScheduleId: string;
  busId: string;
  busNumber: string;
  busName: string;
  companyName: string;
  departureTime: string;
  arrivalTime: string;
  journeyDate: string;
  fromCity: string;
  toCity: string;
  boardingPoint: string;
  droppingPoint: string;
  fare: number;
  currency: string;
  totalSeats: number;
  bookedSeats: number;
  seatsLeft: number;
}

export interface SeatDto {
  seatId: string;
  seatNumber: string;
  row: number;
  column: number;
  status: string;
  isAvailable: boolean;
}

export interface BookingRequest {
  scheduleId: string;
  passengerName: string;
  passengerPhone: string;
  passengerEmail: string;
  passengerNid: string;
  seatNumbers: string[];
}

export interface BookingResponse {
  ticketId: string;
  ticketNumber: string;
  scheduleId: string;
  passengerName: string;
  seatNumbers: string[];
  totalFare: number;
  currency: string;
  bookingDate: string;
  journeyDate: string;
  busNumber: string;
  companyName: string;
  fromCity: string;
  toCity: string;
  departureTime: string;
  boardingPoint: string;
  droppingPoint: string;
}

export interface SearchCriteria {
  fromCity: string;
  toCity: string;
  journeyDate: string;
}
