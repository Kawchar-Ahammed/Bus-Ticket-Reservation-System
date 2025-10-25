import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { BusApi } from '../../services/bus-api';

@Component({
  selector: 'app-booking-history',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTableModule,
    MatChipsModule,
    MatSnackBarModule
  ],
  templateUrl: './booking-history.html',
  styleUrl: './booking-history.scss',
})
export class BookingHistory implements OnInit {
  searchForm: FormGroup;
  searchType: 'phone' | 'ticket' = 'phone';
  loading = false;
  bookings: any[] = [];
  selectedBooking: any = null;
  displayedColumns: string[] = ['ticketNumber', 'passengerName', 'route', 'journeyDate', 'seatNumber', 'status', 'actions'];
  lastSearchPhone = ''; // Store last searched phone for cancellation

  constructor(
    private fb: FormBuilder,
    private busApi: BusApi,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.searchForm = this.fb.group({
      searchValue: ['', [Validators.required, Validators.minLength(10)]]
    });
  }

  ngOnInit() {
    // You can auto-load if phone number is in session/local storage
  }

  setSearchType(type: 'phone' | 'ticket') {
    this.searchType = type;
    this.searchForm.reset();
    this.bookings = [];
    this.selectedBooking = null;
  }

  searchBookings() {
    if (this.searchForm.invalid) {
      return;
    }

    const searchValue = this.searchForm.value.searchValue;
    this.loading = true;

    if (this.searchType === 'phone') {
      this.lastSearchPhone = searchValue; // Store for cancellation
      this.busApi.getBookingsByPhone(searchValue).subscribe({
        next: (bookings: any) => {
          this.bookings = bookings;
          this.loading = false;
          
          if (bookings.length === 0) {
            this.snackBar.open('No bookings found for this phone number', 'Close', {
              duration: 3000
            });
          }
        },
        error: (error: any) => {
          console.error('Error searching bookings:', error);
          this.loading = false;
          this.snackBar.open('Error searching bookings. Please try again.', 'Close', {
            duration: 3000
          });
        }
      });
    } else {
      this.busApi.getBookingByTicket(searchValue).subscribe({
        next: (booking: any) => {
          this.selectedBooking = booking;
          this.bookings = booking ? [booking] : [];
          this.loading = false;
          
          if (!booking) {
            this.snackBar.open('No booking found for this ticket number', 'Close', {
              duration: 3000
            });
          }
        },
        error: (error: any) => {
          console.error('Error searching booking:', error);
          this.loading = false;
          
          if (error.status === 404) {
            this.snackBar.open('Ticket not found', 'Close', {
              duration: 3000
            });
          } else {
            this.snackBar.open('Error searching booking. Please try again.', 'Close', {
              duration: 3000
            });
          }
        }
      });
    }
  }

  viewBookingDetails(booking: any) {
    this.selectedBooking = booking;
  }

  closeDetails() {
    this.selectedBooking = null;
  }

  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'confirmed':
        return 'primary';
      case 'cancelled':
        return 'warn';
      case 'completed':
        return 'accent';
      default:
        return '';
    }
  }

  goBack() {
    this.router.navigate(['/']);
  }

  printTicket(booking: any) {
    // Navigate to print ticket page
    this.router.navigate(['/print-ticket', booking.ticketNumber]);
  }

  cancelBooking(booking: any) {
    const phoneNumber = booking.phoneNumber || this.lastSearchPhone;
    
    if (!phoneNumber) {
      this.snackBar.open('Phone number not available for cancellation', 'Close', {
        duration: 3000
      });
      return;
    }

    // Calculate hours until departure
    const now = new Date();
    const journeyDate = new Date(booking.journeyDate);
    const hoursUntilDeparture = (journeyDate.getTime() - now.getTime()) / (1000 * 60 * 60);
    
    let refundMessage = '';
    if (hoursUntilDeparture >= 24) {
      refundMessage = '✓ Full refund (100%) - Cancelling 24+ hours before departure';
    } else if (hoursUntilDeparture >= 2) {
      refundMessage = '⚠ Partial refund (50%) - Cancelling 2-24 hours before departure';
    } else {
      this.snackBar.open('Cannot cancel less than 2 hours before departure', 'Close', {
        duration: 3000
      });
      return;
    }

    // Show confirmation dialog (simplified version)
    const confirmed = confirm(
      `Cancel Booking?\n\nTicket: ${booking.ticketNumber}\n${refundMessage}\n\nDo you want to proceed?`
    );

    if (confirmed) {
      const reason = prompt('Cancellation reason (optional):') || '';
      this.loading = true;

      this.busApi.cancelBooking(booking.ticketNumber, phoneNumber, reason).subscribe({
        next: (result: any) => {
          this.loading = false;
          
          if (result.success) {
            this.snackBar.open(result.message, 'Close', {
              duration: 5000,
              panelClass: ['success-snackbar']
            });
            
            // Refresh bookings
            this.searchBookings();
          } else {
            this.snackBar.open(result.message, 'Close', {
              duration: 5000,
              panelClass: ['error-snackbar']
            });
          }
        },
        error: (error: any) => {
          console.error('Cancellation error:', error);
          this.loading = false;
          
          const errorMessage = error.error?.message || 'Failed to cancel booking. Please try again.';
          this.snackBar.open(errorMessage, 'Close', {
            duration: 5000,
            panelClass: ['error-snackbar']
          });
        }
      });
    }
  }
}
