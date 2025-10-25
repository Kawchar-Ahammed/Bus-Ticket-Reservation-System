import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { BusScheduleService } from '../../../services/bus-schedule.service';
import { BusService } from '../../../services/bus.service';
import { RouteService } from '../../../services/route.service';
import { BusScheduleDetail } from '../../../models/bus-schedule.model';
import { Bus } from '../../../models/bus.model';
import { Route as BusRoute } from '../../../models/route.model';

@Component({
  selector: 'app-bus-schedule-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './bus-schedule-form.component.html',
  styleUrls: ['./bus-schedule-form.component.scss']
})
export class BusScheduleFormComponent implements OnInit {
  scheduleForm!: FormGroup;
  isEditMode = false;
  scheduleId: string | null = null;
  isLoading = false;
  errorMessage = '';
  
  buses: Bus[] = [];
  routes: BusRoute[] = [];
  loadingBuses = true;
  loadingRoutes = true;

  constructor(
    private fb: FormBuilder,
    private busScheduleService: BusScheduleService,
    private busService: BusService,
    private routeService: RouteService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    this.loadBuses();
    this.loadRoutes();
    
    this.scheduleId = this.route.snapshot.paramMap.get('id');
    if (this.scheduleId) {
      this.isEditMode = true;
      this.loadSchedule(this.scheduleId);
    }
  }

  initializeForm(): void {
    this.scheduleForm = this.fb.group({
      busId: ['', Validators.required],
      routeId: ['', Validators.required],
      journeyDate: ['', Validators.required],
      departureHour: [0, [Validators.required, Validators.min(0), Validators.max(23)]],
      departureMinute: [0, [Validators.required, Validators.min(0), Validators.max(59)]],
      arrivalHour: [0, [Validators.required, Validators.min(0), Validators.max(23)]],
      arrivalMinute: [0, [Validators.required, Validators.min(0), Validators.max(59)]],
      fareAmount: [0, [Validators.required, Validators.min(1)]],
      fareCurrency: ['BDT', Validators.required],
      boardingCity: ['', [Validators.required, Validators.maxLength(100)]],
      droppingCity: ['', [Validators.required, Validators.maxLength(100)]],
      isActive: [true]
    });
  }

  loadBuses(): void {
    this.busService.getAll().subscribe({
      next: (buses) => {
        this.buses = buses.filter(b => b.isActive);
        this.loadingBuses = false;
      },
      error: (error) => {
        console.error('Error loading buses:', error);
        this.loadingBuses = false;
      }
    });
  }

  loadRoutes(): void {
    this.routeService.getAll().subscribe({
      next: (routes) => {
        this.routes = routes.filter(r => r.isActive);
        this.loadingRoutes = false;
      },
      error: (error) => {
        console.error('Error loading routes:', error);
        this.loadingRoutes = false;
      }
    });
  }

  onRouteChange(routeId: string): void {
    const selectedRoute = this.routes.find(r => r.id === routeId);
    if (selectedRoute && !this.isEditMode) {
      this.scheduleForm.patchValue({
        boardingCity: selectedRoute.fromCity,
        droppingCity: selectedRoute.toCity
      });
    }
  }

  loadSchedule(id: string): void {
    this.busScheduleService.getById(id).subscribe({
      next: (schedule: BusScheduleDetail) => {
        // Parse times
        const departureParts = schedule.departureTime.split(':');
        const arrivalParts = schedule.arrivalTime.split(':');

        this.scheduleForm.patchValue({
          busId: schedule.busId,
          routeId: schedule.routeId,
          journeyDate: schedule.journeyDate.split('T')[0],
          departureHour: parseInt(departureParts[0]),
          departureMinute: parseInt(departureParts[1]),
          arrivalHour: parseInt(arrivalParts[0]),
          arrivalMinute: parseInt(arrivalParts[1]),
          fareAmount: schedule.fareAmount,
          fareCurrency: schedule.fareCurrency,
          boardingCity: schedule.boardingCity,
          droppingCity: schedule.droppingCity,
          isActive: schedule.isActive
        });

        // Disable bus and route in edit mode
        this.scheduleForm.get('busId')?.disable();
        this.scheduleForm.get('routeId')?.disable();
      },
      error: (error) => {
        console.error('Error loading schedule:', error);
        this.errorMessage = 'Failed to load schedule details';
      }
    });
  }

  onSubmit(): void {
    if (this.scheduleForm.invalid) {
      Object.keys(this.scheduleForm.controls).forEach(key => {
        this.scheduleForm.get(key)?.markAsTouched();
      });
      return;
    }

    if (this.loadingBuses || this.loadingRoutes) {
      this.errorMessage = 'Please wait while data is loading';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    if (this.isEditMode && this.scheduleId) {
      this.updateSchedule();
    } else {
      this.createSchedule();
    }
  }

  createSchedule(): void {
    const formValue = this.scheduleForm.value;
    this.busScheduleService.create(formValue).subscribe({
      next: () => {
        this.router.navigate(['/admin/schedules']);
      },
      error: (error) => {
        console.error('Error creating schedule:', error);
        this.errorMessage = error.error?.message || 'Failed to create schedule';
        this.isLoading = false;
      }
    });
  }

  updateSchedule(): void {
    const formValue = this.scheduleForm.value;
    const updateRequest = {
      id: this.scheduleId!,
      ...formValue
    };

    this.busScheduleService.update(this.scheduleId!, updateRequest).subscribe({
      next: () => {
        this.router.navigate(['/admin/schedules']);
      },
      error: (error) => {
        console.error('Error updating schedule:', error);
        this.errorMessage = error.error?.message || 'Failed to update schedule';
        this.isLoading = false;
      }
    });
  }

  getFieldError(fieldName: string): string {
    const control = this.scheduleForm.get(fieldName);
    if (control?.touched && control?.errors) {
      if (control.errors['required']) return `${this.getFieldLabel(fieldName)} is required`;
      if (control.errors['min']) return `${this.getFieldLabel(fieldName)} must be at least ${control.errors['min'].min}`;
      if (control.errors['max']) return `${this.getFieldLabel(fieldName)} must be at most ${control.errors['max'].max}`;
      if (control.errors['maxLength']) return `${this.getFieldLabel(fieldName)} is too long`;
    }
    return '';
  }

  getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      busId: 'Bus',
      routeId: 'Route',
      journeyDate: 'Journey date',
      departureHour: 'Departure hour',
      departureMinute: 'Departure minute',
      arrivalHour: 'Arrival hour',
      arrivalMinute: 'Arrival minute',
      fareAmount: 'Fare amount',
      boardingCity: 'Boarding city',
      droppingCity: 'Dropping city'
    };
    return labels[fieldName] || fieldName;
  }

  getCurrentDate(): string {
    return new Date().toISOString().split('T')[0];
  }
}
