import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatStepperModule } from '@angular/material/stepper';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { BusApi } from '../../services/bus-api';
import { BookingResponse } from '../../models/bus.models';
import { PaymentProcessing } from '../payments/payment-processing/payment-processing';
import { PaymentReceipt } from '../payments/payment-receipt/payment-receipt';
import { Payment } from '../../models/payment.model';

@Component({
  selector: 'app-booking-confirmation',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatStepperModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    PaymentProcessing,
    PaymentReceipt
  ],
  templateUrl: './booking-confirmation.html',
  styleUrl: './booking-confirmation.scss'
})
export class BookingConfirmation implements OnInit {
  bookingForm: FormGroup;
  scheduleId: string = '';
  selectedSeats: any[] = []; // Store full seat objects
  scheduleDetails: any = null;
  totalFare: number = 0;
  loading = false;
  bookingComplete = false;
  bookingDetails?: BookingResponse;
  
  // Payment flow
  showPaymentModal = false;
  showReceiptModal = false;
  paymentReceipt: Payment | null = null;
  paymentComplete = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private busApi: BusApi,
    private snackBar: MatSnackBar
  ) {
    const navigation = this.router.getCurrentNavigation();
    if (navigation?.extras.state) {
      this.scheduleId = navigation.extras.state['scheduleId'];
      this.selectedSeats = navigation.extras.state['selectedSeats'];
      this.scheduleDetails = navigation.extras.state['scheduleDetails'];
      this.totalFare = navigation.extras.state['totalFare'];
    }

    this.bookingForm = this.fb.group({
      passengerName: ['', [Validators.required, Validators.minLength(3)]],
      passengerEmail: ['', [Validators.required, Validators.email]],
      passengerPhone: ['', [Validators.required, Validators.pattern(/^[0-9]{11}$/)]],
      passengerNid: ['', [Validators.required, Validators.pattern(/^[0-9]{10,17}$/)]]
    });
  }

  ngOnInit(): void {
    if (!this.scheduleId || this.selectedSeats.length === 0) {
      this.router.navigate(['/']);
    }
  }

  get seatNumbers(): string {
    return this.selectedSeats.map((s: any) => s.seatNumber).join(', ');
  }

  confirmBooking(): void {
    if (this.bookingForm.invalid) {
      this.snackBar.open('Please fill all required fields correctly', 'Close', {
        duration: 3000,
        panelClass: ['error-snackbar']
      });
      return;
    }

    this.loading = true;
    const formValue = this.bookingForm.value;

    // Book each seat individually using the correct backend API
    const bookingCalls = this.selectedSeats.map((seat: any) => {
      const bookingData = {
        busScheduleId: this.scheduleId,
        seatId: seat.seatId,
        passengerName: formValue.passengerName,
        phoneNumber: formValue.passengerPhone,
        email: formValue.passengerEmail,
        boardingPoint: this.scheduleDetails?.fromCity ? `${this.scheduleDetails.fromCity}, ${this.scheduleDetails.fromLocation || 'Main Station'}` : 'Dhaka, Gabtali',
        droppingPoint: this.scheduleDetails?.toCity ? `${this.scheduleDetails.toCity}, ${this.scheduleDetails.toLocation || 'Main Station'}` : 'Chittagong, GEC Circle',
        gender: null,
        age: null
      };
      
      return this.busApi.bookSeats(bookingData as any);
    });

    // Use forkJoin for better RxJS handling of multiple API calls
    forkJoin(bookingCalls).subscribe({
      next: (results: any[]) => {
        // Check if all bookings were successful
        const allSuccessful = results.every((r: any) => r?.success);
        
        if (allSuccessful) {
          // Combine all booking results with schedule and seat details
          const allSeatNumbers = this.selectedSeats.map((s: any) => s.seatNumber).join(', ');
          
          // Format departure time properly
          const departureTime = this.scheduleDetails?.departureTime || '';
          
          // Use the boarding/dropping points from the first booking request
          const firstBookingData = this.selectedSeats[0];
          const boardingPoint = this.scheduleDetails?.fromCity 
            ? `${this.scheduleDetails.fromCity}, ${this.scheduleDetails.fromLocation || 'Main Station'}` 
            : 'Dhaka, Gabtali';
          const droppingPoint = this.scheduleDetails?.toCity 
            ? `${this.scheduleDetails.toCity}, ${this.scheduleDetails.toLocation || 'Main Station'}` 
            : 'Chittagong, GEC Circle';
          
          this.bookingDetails = {
            ...results[0],
            seatNumbers: allSeatNumbers,
            totalFare: this.totalFare,
            companyName: this.scheduleDetails?.companyName || 'Green Line',
            busNumber: this.scheduleDetails?.busName || this.scheduleDetails?.busNumber || 'AC Sleeper',
            fromCity: this.scheduleDetails?.fromCity || 'Dhaka',
            toCity: this.scheduleDetails?.toCity || 'Chittagong',
            route: `${this.scheduleDetails?.fromCity || 'Dhaka'} → ${this.scheduleDetails?.toCity || 'Chittagong'}`,
            journeyDate: this.scheduleDetails?.journeyDate || new Date().toISOString(),
            departureTime: departureTime,
            boardingPoint: boardingPoint,
            droppingPoint: droppingPoint
          };
          
          this.bookingComplete = true;
          this.loading = false;
          this.snackBar.open(
            `Successfully booked ${results.length} seat(s)! Proceeding to payment...`,
            'Close',
            { 
              duration: 3000,
              panelClass: ['success-snackbar']
            }
          );
          
          // Auto-open payment modal after successful booking
          setTimeout(() => {
            this.openPaymentModal();
          }, 1000);
        } else {
          const failedSeats = results
            .filter((r: any) => !r?.success)
            .map((r: any, i: number) => this.selectedSeats[i].seatNumber);
          
          throw new Error(`Booking failed for seats: ${failedSeats.join(', ')}`);
        }
      },
      error: (error: any) => {
        console.error('Booking error:', error);
        console.error('Error details:', error.error);
        this.loading = false;
        
        const errorMessage = error.error?.message || error.message || 'Booking failed. Please try again.';
        this.snackBar.open(errorMessage, 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  openPaymentModal(): void {
    this.showPaymentModal = true;
  }

  closePaymentModal(): void {
    this.showPaymentModal = false;
  }

  onPaymentCompleted(payment: Payment): void {
    this.paymentComplete = true;
    this.paymentReceipt = payment;
    this.showPaymentModal = false;
    this.showReceiptModal = true;
    
    this.snackBar.open('Payment completed successfully! 🎉', 'Close', {
      duration: 5000,
      panelClass: ['success-snackbar']
    });
  }

  onPaymentFailed(error: string): void {
    this.snackBar.open(`Payment failed: ${error}. You can try again.`, 'Close', {
      duration: 5000,
      panelClass: ['error-snackbar']
    });
  }

  onPaymentCancelled(): void {
    this.showPaymentModal = false;
    this.snackBar.open('Payment cancelled. You can pay later from your booking history.', 'Close', {
      duration: 4000,
      panelClass: ['info-snackbar']
    });
  }

  closeReceiptModal(): void {
    this.showReceiptModal = false;
  }

  getTicketId(): string {
    return this.bookingDetails?.ticketId || '';
  }

  getCustomerEmail(): string {
    return this.bookingForm.value.passengerEmail || '';
  }

  goToHome(): void {
    this.router.navigate(['/']);
  }

  printTicket(): void {
    if (this.bookingDetails?.ticketNumber) {
      this.router.navigate(['/print-ticket', this.bookingDetails.ticketNumber]);
    }
  }
}
