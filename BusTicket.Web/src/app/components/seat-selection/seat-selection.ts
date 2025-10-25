import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { BusApi } from '../../services/bus-api';
import { SeatDto } from '../../models/bus.models';

@Component({
  selector: 'app-seat-selection',
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule
  ],
  templateUrl: './seat-selection.html',
  styleUrl: './seat-selection.scss',
})
export class SeatSelection implements OnInit {
  scheduleId: string = '';
  seats: SeatDto[] = [];
  selectedSeats: SeatDto[] = [];
  loading = false;
  maxColumns = 0;
  maxRows = 0;
  scheduleDetails: any = null; // Store full schedule details

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private busApi: BusApi
  ) {}

  ngOnInit() {
    this.scheduleId = this.route.snapshot.paramMap.get('scheduleId') || '';
    if (this.scheduleId) {
      this.loadSeats();
    }
  }

  loadSeats() {
    this.loading = true;
    this.busApi.getSeatPlan(this.scheduleId).subscribe({
      next: (response: any) => {
        // Store the full schedule details
        this.scheduleDetails = response;
        
        // Map the response and add isAvailable property
        this.seats = response.seats.map((seat: any) => ({
          ...seat,
          isAvailable: seat.status === 'Available'
        }));
        this.maxColumns = Math.max(...this.seats.map((s: any) => s.column));
        this.maxRows = Math.max(...this.seats.map((s: any) => s.row));
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading seats:', error);
        this.loading = false;
      }
    });
  }

  getSeatsByRow(row: number): SeatDto[] {
    return this.seats.filter(s => s.row === row)
      .sort((a, b) => a.column - b.column);
  }

  isSeatSelected(seat: SeatDto): boolean {
    return this.selectedSeats.some(s => s.seatId === seat.seatId);
  }

  toggleSeat(seat: SeatDto) {
    if (!seat.isAvailable) return;

    const index = this.selectedSeats.findIndex(s => s.seatId === seat.seatId);
    if (index > -1) {
      this.selectedSeats.splice(index, 1);
    } else {
      this.selectedSeats.push(seat);
    }
  }

  getSeatClass(seat: SeatDto): string {
    if (!seat.isAvailable) return 'seat-booked';
    if (this.isSeatSelected(seat)) return 'seat-selected';
    return 'seat-available';
  }

  getTotalFare(): number {
    // Use the fare from schedule details if available
    const farePerSeat = this.scheduleDetails?.fare || 1200;
    return this.selectedSeats.length * farePerSeat;
  }

  proceedToBooking() {
    if (this.selectedSeats.length === 0) return;

    this.router.navigate(['/confirm'], {
      state: {
        scheduleId: this.scheduleId,
        selectedSeats: this.selectedSeats, // Pass full seat objects
        scheduleDetails: this.scheduleDetails, // Pass schedule details
        totalFare: this.getTotalFare()
      }
    });
  }

  goBack() {
    this.router.navigate(['/']);
  }
}

