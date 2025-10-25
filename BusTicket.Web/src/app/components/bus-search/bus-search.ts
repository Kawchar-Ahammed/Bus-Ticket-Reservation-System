import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { BusApi } from '../../services/bus-api';
import { BusScheduleDto } from '../../models/bus.models';

@Component({
  selector: 'app-bus-search',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './bus-search.html',
  styleUrl: './bus-search.scss',
})
export class BusSearch {
  searchForm: FormGroup;
  busSchedules: BusScheduleDto[] = [];
  loading = false;
  searched = false;
  minDate = new Date();

  constructor(
    private fb: FormBuilder,
    private busApi: BusApi,
    private router: Router
  ) {
    this.searchForm = this.fb.group({
      fromCity: ['', Validators.required],
      toCity: ['', Validators.required],
      journeyDate: [new Date(), Validators.required]
    });
  }

  searchBuses() {
    if (this.searchForm.invalid) {
      return;
    }

    this.loading = true;
    this.searched = false;

    const formValue = this.searchForm.value;
    const journeyDate = new Date(formValue.journeyDate);
    const formattedDate = journeyDate.toISOString().split('T')[0];

    this.busApi.searchBuses({
      fromCity: formValue.fromCity,
      toCity: formValue.toCity,
      journeyDate: formattedDate
    }).subscribe({
      next: (schedules) => {
        this.busSchedules = schedules;
        this.loading = false;
        this.searched = true;
      },
      error: (error) => {
        console.error('Error searching buses:', error);
        this.loading = false;
        this.searched = true;
      }
    });
  }

  selectBus(schedule: BusScheduleDto) {
    this.router.navigate(['/seats', schedule.busScheduleId]);
  }

  formatDuration(duration: string): string {
    return duration;
  }
}

