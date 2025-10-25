import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Payment, getPaymentMethodName, getPaymentStatusName } from '../../../models/payment.model';

@Component({
  selector: 'app-payment-receipt',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './payment-receipt.html',
  styleUrl: './payment-receipt.scss',
})
export class PaymentReceipt {
  @Input() payment!: Payment;
  @Input() ticketDetails: any; // Ticket information if available

  printReceipt(): void {
    window.print();
  }

  downloadReceipt(): void {
    // This would generate a PDF, for now just print
    this.printReceipt();
  }

  getMethodName(method: number): string {
    return getPaymentMethodName(method);
  }

  getStatusName(status: number): string {
    return getPaymentStatusName(status);
  }

  get currentDate(): Date {
    return new Date();
  }
}

