import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AdminUsersService } from '../../../services/admin-users.service';
import { ToastService } from '../../../services/toast.service';
import { AdminRole } from '../../../models/admin-user.model';

@Component({
  selector: 'app-admin-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-user-form.component.html',
  styleUrl: './admin-user-form.component.css'
})
export class AdminUserFormComponent implements OnInit {
  userForm!: FormGroup;
  isEditMode = false;
  userId: string | null = null;
  isSubmitting = false;
  
  roleOptions = [
    { value: AdminRole.SuperAdmin, label: 'Super Admin' },
    { value: AdminRole.Admin, label: 'Admin' },
    { value: AdminRole.Operator, label: 'Operator' }
  ];

  constructor(
    private fb: FormBuilder,
    private adminUsersService: AdminUsersService,
    private toastService: ToastService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.userId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.userId;
    
    this.initForm();
    
    if (this.isEditMode && this.userId) {
      this.loadUser(this.userId);
    }
  }

  initForm(): void {
    if (this.isEditMode) {
      // Edit mode: only full name can be edited
      this.userForm = this.fb.group({
        fullName: ['', [Validators.required, Validators.maxLength(100)]]
      });
    } else {
      // Create mode: all fields
      this.userForm = this.fb.group({
        email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
        password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(50)]],
        fullName: ['', [Validators.required, Validators.maxLength(100)]],
        role: [AdminRole.Admin, [Validators.required]]
      });
    }
  }

  loadUser(id: string): void {
    this.adminUsersService.getById(id).subscribe({
      next: (user) => {
        this.userForm.patchValue({
          fullName: user.fullName
        });
      },
      error: (error) => {
        console.error('Error loading user:', error);
        this.toastService.error('Failed to load user details');
        this.router.navigate(['/admin/users']);
      }
    });
  }

  onSubmit(): void {
    if (this.userForm.invalid) {
      this.markFormGroupTouched(this.userForm);
      return;
    }

    this.isSubmitting = true;

    if (this.isEditMode && this.userId) {
      // Update user
      const updateDto = {
        id: this.userId,
        fullName: this.userForm.value.fullName
      };

      this.adminUsersService.update(updateDto).subscribe({
        next: () => {
          this.toastService.success('Admin user updated successfully');
          this.router.navigate(['/admin/users']);
        },
        error: (error) => {
          console.error('Error updating user:', error);
          const message = error.error?.message || 'Failed to update admin user';
          this.toastService.error(message);
          this.isSubmitting = false;
        }
      });
    } else {
      // Create user
      const createDto = {
        email: this.userForm.value.email,
        password: this.userForm.value.password,
        fullName: this.userForm.value.fullName,
        role: this.userForm.value.role
      };

      this.adminUsersService.create(createDto).subscribe({
        next: () => {
          this.toastService.success('Admin user created successfully');
          this.router.navigate(['/admin/users']);
        },
        error: (error) => {
          console.error('Error creating user:', error);
          const message = error.error?.message || 'Failed to create admin user';
          this.toastService.error(message);
          this.isSubmitting = false;
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/admin/users']);
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  hasError(controlName: string, errorName: string): boolean {
    const control = this.userForm.get(controlName);
    return !!(control && control.hasError(errorName) && control.touched);
  }
}
