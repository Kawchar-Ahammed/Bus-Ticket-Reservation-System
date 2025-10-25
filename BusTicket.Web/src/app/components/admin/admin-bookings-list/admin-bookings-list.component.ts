import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, NavigationEnd } from '@angular/router';
import { AdminBookingService } from '../../../services/admin-booking.service';
import { CompanyService } from '../../../services/company.service';
import { PaymentService } from '../../../services/payment.service';
import { AdminBooking, BookingFilters } from '../../../models/admin-booking.model';
import { Company } from '../../../models/company.model';
import { Payment } from '../../../models/payment.model';
import { PaymentProcessing } from '../../payments/payment-processing/payment-processing';
import { PaymentReceipt } from '../../payments/payment-receipt/payment-receipt';
import { filter } from 'rxjs';

@Component({
  selector: 'app-admin-bookings-list',
  standalone: true,
  imports: [CommonModule, FormsModule, PaymentProcessing, PaymentReceipt],
  templateUrl: './admin-bookings-list.component.html',
  styleUrls: ['./admin-bookings-list.component.scss']
})
export class AdminBookingsListComponent implements OnInit {
  bookings: AdminBooking[] = [];
  filteredBookings: AdminBooking[] = [];
  companies: Company[] = [];
  isLoading = false;
  errorMessage = '';

  // Filters
  filters: BookingFilters = {};
  selectedCompanyId = '';
  selectedBookingStatus = '';
  journeyDateFrom = '';
  journeyDateTo = '';
  bookingDateFrom = '';
  bookingDateTo = '';
  searchTerm = '';

  // For details modal
  selectedBooking: AdminBooking | null = null;
  showDetailsModal = false;

  // For payment modal
  showPaymentModal = false;
  paymentTicketId = '';
  paymentAmount = 0;
  paymentEmail = '';

  // For payment receipt modal
  showReceiptModal = false;
  paymentReceipt: Payment | null = null;

  // Store payment info for each booking
  bookingPayments: Map<string, Payment> = new Map();

  constructor(
    private adminBookingService: AdminBookingService,
    private companyService: CompanyService,
    private paymentService: PaymentService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadCompanies();
    this.loadBookings();
    
    // Reload bookings when navigating back to this route
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      if (this.router.url === '/admin/bookings' || this.router.url.startsWith('/admin/bookings?')) {
        this.loadBookings();
      }
    });
  }

  loadCompanies(): void {
    this.companyService.getAll().subscribe({
      next: (companies) => {
        this.companies = companies.filter(c => c.isActive);
      },
      error: (error) => {
        console.error('Error loading companies:', error);
      }
    });
  }

  loadBookings(): void {
    this.isLoading = true;
    this.errorMessage = '';

    // Build filters object
    const filters: BookingFilters = {};
    if (this.selectedCompanyId) filters.companyId = this.selectedCompanyId;
    if (this.selectedBookingStatus) filters.bookingStatus = this.selectedBookingStatus;
    if (this.journeyDateFrom) filters.journeyDateFrom = this.journeyDateFrom;
    if (this.journeyDateTo) filters.journeyDateTo = this.journeyDateTo;
    if (this.bookingDateFrom) filters.bookingDateFrom = this.bookingDateFrom;
    if (this.bookingDateTo) filters.bookingDateTo = this.bookingDateTo;
    if (this.searchTerm) filters.searchTerm = this.searchTerm;

    this.adminBookingService.getAll(filters).subscribe({
      next: (bookings) => {
        this.bookings = bookings;
        this.filteredBookings = bookings;
        this.isLoading = false;
        
        // Load payment info for each booking
        this.loadPaymentInfo(bookings);
      },
      error: (error) => {
        console.error('Error loading bookings:', error);
        this.errorMessage = 'Failed to load bookings. Please try again.';
        this.isLoading = false;
      }
    });
  }

  loadPaymentInfo(bookings: AdminBooking[]): void {
    // Load payment information for confirmed bookings
    bookings.forEach(booking => {
      if (booking.isConfirmed) {
        this.paymentService.getPaymentByTicketId(booking.ticketId).subscribe({
          next: (payment) => {
            this.bookingPayments.set(booking.ticketId, payment);
          },
          error: () => {
            // Payment not found is okay - might not be paid yet
          }
        });
      }
    });
  }

  getPaymentInfo(ticketId: string): Payment | undefined {
    return this.bookingPayments.get(ticketId);
  }

  getPaymentStatus(booking: AdminBooking): string {
    const payment = this.getPaymentInfo(booking.ticketId);
    if (payment) {
      return payment.statusName;
    }
    // No payment found
    if (booking.isConfirmed) {
      return 'Unpaid';
    }
    if (booking.isCancelled) {
      return 'N/A';
    }
    return 'Pending';
  }

  hasPayment(ticketId: string): boolean {
    return this.bookingPayments.has(ticketId);
  }

  applyFilters(): void {
    this.loadBookings();
  }

  clearFilters(): void {
    this.selectedCompanyId = '';
    this.selectedBookingStatus = '';
    this.journeyDateFrom = '';
    this.journeyDateTo = '';
    this.bookingDateFrom = '';
    this.bookingDateTo = '';
    this.searchTerm = '';
    this.loadBookings();
  }

  formatTime(timeSpan: string): string {
    if (!timeSpan) return '';
    const parts = timeSpan.split(':');
    return `${parts[0]}:${parts[1]}`;
  }

  formatDate(dateString: string): string {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  formatDateTime(dateString: string): string {
    if (!dateString) return '';
    return new Date(dateString).toLocaleString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'confirmed':
        return 'status-confirmed';
      case 'cancelled':
        return 'status-cancelled';
      case 'pending':
        return 'status-pending';
      default:
        return '';
    }
  }

  showDetails(booking: AdminBooking): void {
    this.selectedBooking = booking;
    this.showDetailsModal = true;
  }

  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedBooking = null;
  }

  getTotalFare(): number {
    return this.filteredBookings.reduce((sum, b) => sum + b.fare, 0);
  }

  getConfirmedCount(): number {
    return this.filteredBookings.filter(b => b.isConfirmed && !b.isCancelled).length;
  }

  getCancelledCount(): number {
    return this.filteredBookings.filter(b => b.isCancelled).length;
  }

  getPendingCount(): number {
    return this.filteredBookings.filter(b => !b.isConfirmed && !b.isCancelled).length;
  }

  processPayment(booking: AdminBooking): void {
    this.paymentTicketId = booking.ticketId;
    this.paymentAmount = booking.fare;
    this.paymentEmail = booking.email;
    this.showPaymentModal = true;
  }

  onPaymentCompleted(payment: Payment): void {
    this.bookingPayments.set(this.paymentTicketId, payment);
    this.showPaymentModal = false;
    this.paymentReceipt = payment;
    this.showReceiptModal = true;
    this.loadBookings();
  }

  onPaymentFailed(error: string): void {
    console.error('Payment failed:', error);
  }

  onPaymentCancelled(): void {
    this.showPaymentModal = false;
    this.paymentTicketId = '';
    this.paymentAmount = 0;
    this.paymentEmail = '';
  }

  viewReceipt(booking: AdminBooking): void {
    const payment = this.getPaymentInfo(booking.ticketId);
    if (payment) {
      this.paymentReceipt = payment;
      this.showReceiptModal = true;
    }
  }

  closeReceiptModal(): void {
    this.showReceiptModal = false;
    this.paymentReceipt = null;
  }

  closePaymentModal(): void {
    this.showPaymentModal = false;
    this.paymentTicketId = '';
    this.paymentAmount = 0;
    this.paymentEmail = '';
  }
}
