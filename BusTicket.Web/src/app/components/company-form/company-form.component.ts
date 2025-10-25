import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CompanyService } from '../../services/company.service';
import { ToastService } from '../../services/toast.service';
import { CreateCompanyRequest, UpdateCompanyRequest } from '../../models/company.model';

@Component({
  selector: 'app-company-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './company-form.component.html',
  styleUrl: './company-form.component.scss'
})
export class CompanyFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private companyService = inject(CompanyService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastService = inject(ToastService);

  companyForm: FormGroup;
  isEditMode = false;
  companyId: string | null = null;
  loading = false;
  submitting = false;
  errorMessage = '';
  successMessage = '';

  constructor() {
    this.companyForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      email: ['', [Validators.email]],
      contactNumber: ['', [Validators.maxLength(20)]],
      description: ['', [Validators.maxLength(1000)]],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.companyId = params['id'];
      this.isEditMode = !!this.companyId;

      if (this.isEditMode && this.companyId) {
        this.loadCompany(this.companyId);
      }
    });
  }

  loadCompany(id: string): void {
    this.loading = true;
    this.errorMessage = '';

    this.companyService.getById(id).subscribe({
      next: (company) => {
        this.companyForm.patchValue({
          name: company.name,
          email: company.email || '',
          contactNumber: company.contactNumber || '',
          description: company.description || '',
          isActive: company.isActive
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading company:', error);
        this.errorMessage = 'Failed to load company details. Please try again.';
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.companyForm.invalid) {
      this.companyForm.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.isEditMode && this.companyId) {
      this.updateCompany();
    } else {
      this.createCompany();
    }
  }

  createCompany(): void {
    const request: CreateCompanyRequest = {
      name: this.companyForm.value.name,
      email: this.companyForm.value.email || undefined,
      contactNumber: this.companyForm.value.contactNumber || undefined,
      description: this.companyForm.value.description || undefined
    };

    this.companyService.create(request).subscribe({
      next: (response) => {
        this.submitting = false;
        this.toastService.success('Company created successfully!');
        setTimeout(() => {
          this.router.navigate(['/admin/companies']);
        }, 500);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || error.error?.title || 'Failed to create company. Please try again.';
        this.toastService.error(this.errorMessage);
        this.submitting = false;
      }
    });
  }

  updateCompany(): void {
    if (!this.companyId) return;

    const request: UpdateCompanyRequest = {
      id: this.companyId,
      name: this.companyForm.value.name,
      email: this.companyForm.value.email || undefined,
      contactNumber: this.companyForm.value.contactNumber || undefined,
      description: this.companyForm.value.description || undefined,
      isActive: this.companyForm.value.isActive
    };

    this.companyService.update(this.companyId, request).subscribe({
      next: () => {
        this.submitting = false;
        this.toastService.success('Company updated successfully!');
        setTimeout(() => {
          this.router.navigate(['/admin/companies']);
        }, 500);
      },
      error: (error) => {
        console.error('Error updating company:', error);
        this.errorMessage = error.error?.message || 'Failed to update company. Please try again.';
        this.toastService.error(this.errorMessage);
        this.submitting = false;
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/companies']);
  }

  getFieldError(fieldName: string): string {
    const field = this.companyForm.get(fieldName);
    
    if (field?.hasError('required')) {
      return `${this.getFieldLabel(fieldName)} is required`;
    }
    
    if (field?.hasError('email')) {
      return 'Please enter a valid email address';
    }
    
    if (field?.hasError('maxlength')) {
      const maxLength = field.errors?.['maxlength'].requiredLength;
      return `${this.getFieldLabel(fieldName)} must not exceed ${maxLength} characters`;
    }
    
    return '';
  }

  getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      name: 'Company name',
      email: 'Email',
      contactNumber: 'Contact number',
      description: 'Description',
      isActive: 'Status'
    };
    return labels[fieldName] || fieldName;
  }

  hasFieldError(fieldName: string): boolean {
    const field = this.companyForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }
}
