export interface DashboardStatistics {
  totalCompanies: number;
  activeCompanies: number;
  totalBuses: number;
  activeBuses: number;
  totalRoutes: number;
  activeRoutes: number;
  totalSchedules: number;
  todaySchedules: number;
  totalBookings: number;
  todayBookings: number;
  weekBookings: number;
  confirmedBookings: number;
  pendingBookings: number;
  cancelledBookings: number;
  recentBookings: RecentBooking[];
}

export interface RecentBooking {
  id: string;
  ticketNumber: string;
  passengerName: string;
  companyName: string;
  route: string;
  bookingDate: Date;
  fareAmount: number;
  status: string;
}
