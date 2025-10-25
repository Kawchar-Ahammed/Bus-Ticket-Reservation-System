export interface Company {
  id: string;
  name: string;
  email?: string;
  contactNumber?: string;
  description?: string;
  isActive: boolean;
  createdAt: Date;
}

export interface CompanyDetail extends Company {
  updatedAt?: Date;
  createdBy?: string;
  updatedBy?: string;
}

export interface CreateCompanyRequest {
  name: string;
  email?: string;
  contactNumber?: string;
  description?: string;
}

export interface UpdateCompanyRequest {
  id: string;
  name: string;
  email?: string;
  contactNumber?: string;
  description?: string;
  isActive: boolean;
}
