import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap, map } from 'rxjs/operators';
import { Payment, CreatePaymentRequest, RefundRequest } from '../models/payment.model';
import { ToastService } from './toast.service';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private apiUrl = 'http://localhost:5258/api/Payments';

  constructor(
    private http: HttpClient,
    private toastService: ToastService
  ) {}

  /**
   * Process payment for a ticket
   */
  processPayment(request: CreatePaymentRequest): Observable<Payment> {
    return this.http.post<any>(`${this.apiUrl}/process`, request).pipe(
      map((response: any) => {
        // Backend returns PaymentResponseDto with nested Payment property
        const payment: Payment = response.payment || response;
        return payment;
      }),
      tap((payment: Payment) => {
        if (payment.status === 2) { // Completed
          this.toastService.showSuccess('Payment processed successfully!');
        } else if (payment.status === 3) { // Failed
          this.toastService.showError(`Payment failed: ${payment.failureReason || 'Unknown error'}`);
        }
      }),
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Refund a payment
   */
  refundPayment(request: RefundRequest): Observable<Payment> {
    return this.http.post<Payment>(`${this.apiUrl}/refund`, request).pipe(
      tap(payment => {
        this.toastService.showSuccess(`Refund of ${payment.refundAmount} ${payment.currency} processed successfully!`);
      }),
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Get payment details by ticket ID
   */
  getPaymentByTicketId(ticketId: string): Observable<Payment> {
    return this.http.get<Payment>(`${this.apiUrl}/ticket/${ticketId}`).pipe(
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Verify payment status by transaction ID
   */
  verifyPayment(transactionId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/verify/${transactionId}`, {}).pipe(
      tap(response => {
        this.toastService.showInfo('Payment verification completed');
      }),
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Handle HTTP errors
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An error occurred while processing payment';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      if (error.error?.message) {
        errorMessage = error.error.message;
      } else if (error.error?.title) {
        errorMessage = error.error.title;
      } else if (error.status === 400) {
        errorMessage = 'Invalid payment request. Please check your input.';
      } else if (error.status === 404) {
        errorMessage = 'Payment or ticket not found.';
      } else if (error.status === 409) {
        errorMessage = 'Payment already processed for this ticket.';
      } else if (error.status === 500) {
        errorMessage = 'Payment gateway error. Please try again later.';
      }
    }

    this.toastService.showError(errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
