export interface AdminBooking {
  ticketId: string;
  ticketNumber: string;
  passengerId: string;
  passengerName: string;
  phoneNumber: string;
  email: string;
  companyId: string;
  companyName: string;
  busName: string;
  busNumber: string;
  fromCity: string;
  toCity: string;
  journeyDate: string;
  departureTime: string; // TimeSpan format "HH:MM:SS"
  seatNumber: string;
  bookingDate: string;
  fare: number;
  currency: string;
  bookingStatus: string; // Confirmed, Cancelled, Pending
  isConfirmed: boolean;
  isCancelled: boolean;
  confirmationDate?: string;
}

export interface AdminBookingDetail extends AdminBooking {
  gender?: string;
  age?: number;
  arrivalTime: string; // TimeSpan format "HH:MM:SS"
  boardingPoint: string;
  droppingPoint: string;
  busScheduleId: string;
  seatId: string;
  createdAt: string;
  cancelledAt?: string;
  cancellationReason?: string;
}

export interface BookingFilters {
  companyId?: string;
  bookingStatus?: string; // Confirmed, Cancelled, Pending
  journeyDateFrom?: string;
  journeyDateTo?: string;
  bookingDateFrom?: string;
  bookingDateTo?: string;
  searchTerm?: string; // Search by ticket number, passenger name, or phone
}
