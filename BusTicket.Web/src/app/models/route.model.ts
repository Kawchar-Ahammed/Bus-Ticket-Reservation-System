export interface Route {
  id: string;
  fromCity: string;
  toCity: string;
  distanceInKm: number;
  estimatedDuration: string; // TimeSpan as string (e.g., "02:30:00")
  isActive: boolean;
  createdAt: string;
}

export interface RouteDetail extends Route {
  updatedAt?: string;
  createdBy?: string;
  updatedBy?: string;
}

export interface CreateRouteRequest {
  fromCity: string;
  toCity: string;
  distanceInKm: number;
  durationHours: number;
  durationMinutes: number;
}

export interface UpdateRouteRequest {
  id: string;
  distanceInKm: number;
  durationHours: number;
  durationMinutes: number;
  isActive: boolean;
}
