import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouteService } from '../../../services/route.service';
import { RouteDetail } from '../../../models/route.model';

@Component({
  selector: 'app-route-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './route-form.component.html',
  styleUrls: ['./route-form.component.scss']
})
export class RouteFormComponent implements OnInit {
  routeForm!: FormGroup;
  isEditMode = false;
  routeId: string | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private routeService: RouteService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    this.routeId = this.route.snapshot.paramMap.get('id');
    if (this.routeId) {
      this.isEditMode = true;
      this.loadRoute(this.routeId);
    }
  }

  initializeForm(): void {
    this.routeForm = this.fb.group({
      fromCity: ['', [Validators.required, Validators.maxLength(100)]],
      toCity: ['', [Validators.required, Validators.maxLength(100)]],
      distanceInKm: [0, [Validators.required, Validators.min(1)]],
      durationHours: [0, [Validators.required, Validators.min(0), Validators.max(71)]],
      durationMinutes: [0, [Validators.required, Validators.min(0), Validators.max(59)]],
      isActive: [true]
    });
  }

  loadRoute(id: string): void {
    this.routeService.getById(id).subscribe({
      next: (route: RouteDetail) => {
        // Parse duration string (e.g., "02:30:00" -> hours: 2, minutes: 30)
        const durationParts = route.estimatedDuration.split(':');
        const hours = parseInt(durationParts[0]);
        const minutes = parseInt(durationParts[1]);

        this.routeForm.patchValue({
          fromCity: route.fromCity,
          toCity: route.toCity,
          distanceInKm: route.distanceInKm,
          durationHours: hours,
          durationMinutes: minutes,
          isActive: route.isActive
        });

        // Disable city fields in edit mode (locations cannot be changed)
        this.routeForm.get('fromCity')?.disable();
        this.routeForm.get('toCity')?.disable();
      },
      error: (error) => {
        console.error('Error loading route:', error);
        this.errorMessage = 'Failed to load route details';
      }
    });
  }

  onSubmit(): void {
    if (this.routeForm.invalid) {
      Object.keys(this.routeForm.controls).forEach(key => {
        this.routeForm.get(key)?.markAsTouched();
      });
      return;
    }

    // Validate that total duration is greater than 0
    const hours = this.routeForm.get('durationHours')?.value || 0;
    const minutes = this.routeForm.get('durationMinutes')?.value || 0;
    if (hours === 0 && minutes === 0) {
      this.errorMessage = 'Total duration must be greater than 0';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    if (this.isEditMode && this.routeId) {
      this.updateRoute();
    } else {
      this.createRoute();
    }
  }

  createRoute(): void {
    const formValue = this.routeForm.value;
    this.routeService.create(formValue).subscribe({
      next: () => {
        this.router.navigate(['/admin/routes']);
      },
      error: (error) => {
        console.error('Error creating route:', error);
        this.errorMessage = error.error?.message || 'Failed to create route';
        this.isLoading = false;
      }
    });
  }

  updateRoute(): void {
    const formValue = this.routeForm.value;
    const updateRequest = {
      id: this.routeId!,
      distanceInKm: formValue.distanceInKm,
      durationHours: formValue.durationHours,
      durationMinutes: formValue.durationMinutes,
      isActive: formValue.isActive
    };

    this.routeService.update(this.routeId!, updateRequest).subscribe({
      next: () => {
        this.router.navigate(['/admin/routes']);
      },
      error: (error) => {
        console.error('Error updating route:', error);
        this.errorMessage = error.error?.message || 'Failed to update route';
        this.isLoading = false;
      }
    });
  }

  getFieldError(fieldName: string): string {
    const control = this.routeForm.get(fieldName);
    if (control?.touched && control?.errors) {
      if (control.errors['required']) return `${this.getFieldLabel(fieldName)} is required`;
      if (control.errors['maxLength']) return `${this.getFieldLabel(fieldName)} is too long`;
      if (control.errors['min']) return `${this.getFieldLabel(fieldName)} must be greater than ${control.errors['min'].min - 1}`;
      if (control.errors['max']) return `${this.getFieldLabel(fieldName)} must be less than ${control.errors['max'].max + 1}`;
    }
    return '';
  }

  getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      fromCity: 'From city',
      toCity: 'To city',
      distanceInKm: 'Distance',
      durationHours: 'Hours',
      durationMinutes: 'Minutes'
    };
    return labels[fieldName] || fieldName;
  }
}
