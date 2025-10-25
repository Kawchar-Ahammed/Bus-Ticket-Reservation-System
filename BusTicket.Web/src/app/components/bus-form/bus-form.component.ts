import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { BusService } from '../../services/bus.service';
import { CompanyService } from '../../services/company.service';
import { CreateBusRequest, UpdateBusRequest } from '../../models/bus.model';
import { Company } from '../../models/company.model';

@Component({
  selector: 'app-bus-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './bus-form.component.html',
  styleUrl: './bus-form.component.scss'
})
export class BusFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private busService = inject(BusService);
  private companyService = inject(CompanyService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  busForm: FormGroup;
  companies: Company[] = [];
  isEditMode = false;
  busId: string | null = null;
  loading = false;
  loadingCompanies = false;
  submitting = false;
  errorMessage = '';
  successMessage = '';

  constructor() {
    this.busForm = this.fb.group({
      busNumber: ['', [Validators.required, Validators.maxLength(50)]],
      busName: ['', [Validators.required, Validators.maxLength(200)]],
      companyId: ['', [Validators.required]],
      totalSeats: [40, [Validators.required, Validators.min(1), Validators.max(100)]],
      description: ['', [Validators.maxLength(1000)]],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadCompanies();
    
    this.route.params.subscribe(params => {
      this.busId = params['id'];
      this.isEditMode = !!this.busId;

      if (this.isEditMode && this.busId) {
        this.loadBus(this.busId);
      }
    });
  }

  loadCompanies(): void {
    this.loadingCompanies = true;
    this.companyService.getAll().subscribe({
      next: (companies) => {
        this.companies = companies.filter(c => c.isActive);
        this.loadingCompanies = false;
      },
      error: (error) => {
        console.error('Error loading companies:', error);
        this.errorMessage = 'Failed to load companies. Please refresh the page.';
        this.loadingCompanies = false;
      }
    });
  }

  loadBus(id: string): void {
    this.loading = true;
    this.errorMessage = '';

    this.busService.getById(id).subscribe({
      next: (bus) => {
        this.busForm.patchValue({
          busNumber: bus.busNumber,
          busName: bus.busName,
          companyId: bus.companyId,
          totalSeats: bus.totalSeats,
          description: bus.description || '',
          isActive: bus.isActive
        });
        // Disable bus number in edit mode (it's like a unique identifier)
        this.busForm.get('busNumber')?.disable();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading bus:', error);
        this.errorMessage = 'Failed to load bus details. Please try again.';
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.busForm.invalid) {
      this.busForm.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.isEditMode && this.busId) {
      this.updateBus();
    } else {
      this.createBus();
    }
  }

  createBus(): void {
    const request: CreateBusRequest = {
      busNumber: this.busForm.value.busNumber,
      busName: this.busForm.value.busName,
      companyId: this.busForm.value.companyId,
      totalSeats: this.busForm.value.totalSeats,
      description: this.busForm.value.description || undefined
    };

    this.busService.create(request).subscribe({
      next: (response) => {
        this.successMessage = 'Bus created successfully!';
        setTimeout(() => {
          this.router.navigate(['/admin/buses']);
        }, 1500);
      },
      error: (error) => {
        console.error('Error creating bus:', error);
        this.errorMessage = error.error?.message || 'Failed to create bus. Please try again.';
        this.submitting = false;
      }
    });
  }

  updateBus(): void {
    if (!this.busId) return;

    const request: UpdateBusRequest = {
      id: this.busId,
      busName: this.busForm.value.busName,
      description: this.busForm.value.description || undefined,
      isActive: this.busForm.value.isActive
    };

    this.busService.update(this.busId, request).subscribe({
      next: () => {
        this.successMessage = 'Bus updated successfully!';
        setTimeout(() => {
          this.router.navigate(['/admin/buses']);
        }, 1500);
      },
      error: (error) => {
        console.error('Error updating bus:', error);
        this.errorMessage = error.error?.message || 'Failed to update bus. Please try again.';
        this.submitting = false;
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/buses']);
  }

  getFieldError(fieldName: string): string {
    const field = this.busForm.get(fieldName);
    
    if (field?.hasError('required')) {
      return `${this.getFieldLabel(fieldName)} is required`;
    }
    
    if (field?.hasError('min')) {
      const min = field.errors?.['min'].min;
      return `${this.getFieldLabel(fieldName)} must be at least ${min}`;
    }
    
    if (field?.hasError('max')) {
      const max = field.errors?.['max'].max;
      return `${this.getFieldLabel(fieldName)} must not exceed ${max}`;
    }
    
    if (field?.hasError('maxlength')) {
      const maxLength = field.errors?.['maxlength'].requiredLength;
      return `${this.getFieldLabel(fieldName)} must not exceed ${maxLength} characters`;
    }
    
    return '';
  }

  getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      busNumber: 'Bus number',
      busName: 'Bus name',
      companyId: 'Company',
      totalSeats: 'Total seats',
      description: 'Description',
      isActive: 'Status'
    };
    return labels[fieldName] || fieldName;
  }

  hasFieldError(fieldName: string): boolean {
    const field = this.busForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }
}
