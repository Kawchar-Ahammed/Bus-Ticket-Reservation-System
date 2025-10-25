export enum PaymentMethod {
  Card = 0,
  Cash = 1,
  BankTransfer = 2,
  MobileBanking = 3,
  BKash = 4,
  Nagad = 5,
  Rocket = 6
}

export enum PaymentStatus {
  Pending = 0,
  Processing = 1,
  Completed = 2,
  Failed = 3,
  Refunded = 4,
  PartiallyRefunded = 5
}

export enum PaymentGateway {
  Manual = 0,
  SSLCommerz = 1,
  Stripe = 2,
  BKash = 3,
  Nagad = 4,
  Rocket = 5,
  Mock = 6
}

export interface Payment {
  id: string;
  ticketId: string;
  amount: number;
  currency: string;
  paymentMethod: PaymentMethod;
  paymentMethodName: string;
  status: PaymentStatus;
  statusName: string;
  gateway: PaymentGateway;
  gatewayName: string;
  transactionId: string;
  gatewayTransactionId: string;
  paymentDate: string;
  refundAmount: number;
  refundDate?: string;
  refundReason?: string;
  failureReason?: string;
  transactions: Transaction[];
}

export interface Transaction {
  id: string;
  paymentId: string;
  gatewayTransactionId: string;
  gateway: PaymentGateway;
  gatewayName: string;
  action: string;
  amount: number;
  isSuccess: boolean;
  responseCode?: string;
  responseMessage?: string;
  processedAt: string;
}

export interface CreatePaymentRequest {
  ticketId: string;
  amount: number;
  paymentMethod: PaymentMethod;
  paymentGateway: PaymentGateway;
  customerEmail: string;
}

export interface RefundRequest {
  paymentId: string;
  refundAmount: number;
  reason: string;
}

export interface PaymentGatewayResponse {
  isSuccess: boolean;
  transactionId: string;
  gatewayTransactionId: string;
  amount: number;
  currency: string;
  responseCode?: string;
  message?: string;
  errorMessage?: string;
  rawResponse?: string;
  processedAt: string;
}

// Helper functions for display
export function getPaymentMethodName(method: PaymentMethod): string {
  switch (method) {
    case PaymentMethod.Card: return 'Card';
    case PaymentMethod.Cash: return 'Cash';
    case PaymentMethod.BankTransfer: return 'Bank Transfer';
    case PaymentMethod.MobileBanking: return 'Mobile Banking';
    case PaymentMethod.BKash: return 'bKash';
    case PaymentMethod.Nagad: return 'Nagad';
    case PaymentMethod.Rocket: return 'Rocket';
    default: return 'Unknown';
  }
}

export function getPaymentStatusName(status: PaymentStatus): string {
  switch (status) {
    case PaymentStatus.Pending: return 'Pending';
    case PaymentStatus.Processing: return 'Processing';
    case PaymentStatus.Completed: return 'Completed';
    case PaymentStatus.Failed: return 'Failed';
    case PaymentStatus.Refunded: return 'Refunded';
    case PaymentStatus.PartiallyRefunded: return 'Partially Refunded';
    default: return 'Unknown';
  }
}

export function getPaymentGatewayName(gateway: PaymentGateway): string {
  switch (gateway) {
    case PaymentGateway.Manual: return 'Manual';
    case PaymentGateway.SSLCommerz: return 'SSLCommerz';
    case PaymentGateway.Stripe: return 'Stripe';
    case PaymentGateway.BKash: return 'bKash';
    case PaymentGateway.Nagad: return 'Nagad';
    case PaymentGateway.Rocket: return 'Rocket';
    case PaymentGateway.Mock: return 'Mock';
    default: return 'Unknown';
  }
}

export function getPaymentStatusColor(status: PaymentStatus): string {
  switch (status) {
    case PaymentStatus.Completed: return 'success';
    case PaymentStatus.Processing: return 'info';
    case PaymentStatus.Pending: return 'warning';
    case PaymentStatus.Failed: return 'danger';
    case PaymentStatus.Refunded: return 'secondary';
    case PaymentStatus.PartiallyRefunded: return 'secondary';
    default: return 'secondary';
  }
}
