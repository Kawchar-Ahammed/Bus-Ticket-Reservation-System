export interface BusSchedule {
  id: string;
  busId: string;
  busNumber: string;
  busName: string;
  routeId: string;
  fromCity: string;
  toCity: string;
  journeyDate: string;
  departureTime: string; // TimeSpan as string
  arrivalTime: string;
  fareAmount: number;
  fareCurrency: string;
  boardingCity: string;
  droppingCity: string;
  isActive: boolean;
  availableSeats: number;
  totalSeats: number;
  createdAt: string;
}

export interface BusScheduleDetail extends BusSchedule {
  companyName: string;
  routeDistance: number;
  updatedAt?: string;
  createdBy?: string;
  updatedBy?: string;
}

export interface CreateBusScheduleRequest {
  busId: string;
  routeId: string;
  journeyDate: string; // ISO date string
  departureHour: number;
  departureMinute: number;
  arrivalHour: number;
  arrivalMinute: number;
  fareAmount: number;
  fareCurrency: string;
  boardingCity: string;
  droppingCity: string;
}

export interface UpdateBusScheduleRequest {
  id: string;
  journeyDate: string;
  departureHour: number;
  departureMinute: number;
  arrivalHour: number;
  arrivalMinute: number;
  fareAmount: number;
  fareCurrency: string;
  boardingCity: string;
  droppingCity: string;
  isActive: boolean;
}
