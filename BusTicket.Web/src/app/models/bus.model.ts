export interface Bus {
  id: string;
  busNumber: string;
  busName: string;
  companyId: string;
  companyName: string;
  totalSeats: number;
  description?: string;
  isActive: boolean;
  createdAt: Date;
}

export interface BusDetail {
  id: string;
  busNumber: string;
  busName: string;
  companyId: string;
  companyName: string;
  totalSeats: number;
  description?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
  createdBy?: string;
  updatedBy?: string;
}

export interface CreateBusRequest {
  busNumber: string;
  busName: string;
  companyId: string;
  totalSeats: number;
  description?: string;
}

export interface UpdateBusRequest {
  id: string;
  busName: string;
  description?: string;
  isActive: boolean;
}
