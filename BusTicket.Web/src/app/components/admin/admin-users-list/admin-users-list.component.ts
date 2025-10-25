import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminUsersService } from '../../../services/admin-users.service';
import { ToastService } from '../../../services/toast.service';
import { AdminUser, AdminRole } from '../../../models/admin-user.model';

@Component({
  selector: 'app-admin-users-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-users-list.component.html',
  styleUrl: './admin-users-list.component.css'
})
export class AdminUsersListComponent implements OnInit {
  users: AdminUser[] = [];
  filteredUsers: AdminUser[] = [];
  isLoading = false;
  
  // Filters
  selectedRole: AdminRole | '' = '';
  selectedStatus: boolean | '' = '';
  searchText = '';
  
  // Modal state
  showRoleModal = false;
  showConfirmModal = false;
  selectedUser: AdminUser | null = null;
  newRole: AdminRole = AdminRole.Admin;
  confirmAction: 'deactivate' | 'activate' | null = null;
  
  // Enums for template
  AdminRole = AdminRole;
  roleOptions = [
    { value: AdminRole.SuperAdmin, label: 'Super Admin' },
    { value: AdminRole.Admin, label: 'Admin' },
    { value: AdminRole.Operator, label: 'Operator' }
  ];

  constructor(
    private adminUsersService: AdminUsersService,
    private toastService: ToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading = true;
    const role = this.selectedRole !== '' ? this.selectedRole : undefined;
    const isActive = this.selectedStatus !== '' ? this.selectedStatus : undefined;
    
    this.adminUsersService.getAll(role, isActive).subscribe({
      next: (users) => {
        this.users = users;
        this.applyFilters();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.toastService.error('Failed to load admin users');
        this.isLoading = false;
      }
    });
  }

  applyFilters(): void {
    this.filteredUsers = this.users.filter(user => {
      const matchesSearch = !this.searchText || 
        user.email.toLowerCase().includes(this.searchText.toLowerCase()) ||
        user.fullName.toLowerCase().includes(this.searchText.toLowerCase());
      
      return matchesSearch;
    });
  }

  onFilterChange(): void {
    this.loadUsers();
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  createUser(): void {
    this.router.navigate(['/admin/users/create']);
  }

  editUser(user: AdminUser): void {
    this.router.navigate(['/admin/users/edit', user.id]);
  }

  openChangeRoleModal(user: AdminUser): void {
    this.selectedUser = user;
    this.newRole = user.role;
    this.showRoleModal = true;
  }

  closeRoleModal(): void {
    this.showRoleModal = false;
    this.selectedUser = null;
  }

  confirmChangeRole(): void {
    if (!this.selectedUser) return;

    this.adminUsersService.changeRole({
      id: this.selectedUser.id,
      newRole: this.newRole
    }).subscribe({
      next: () => {
        this.toastService.success('User role changed successfully');
        this.closeRoleModal();
        this.loadUsers();
      },
      error: (error) => {
        console.error('Error changing role:', error);
        const message = error.error?.message || 'Failed to change user role';
        this.toastService.error(message);
      }
    });
  }

  openConfirmModal(user: AdminUser, action: 'deactivate' | 'activate'): void {
    this.selectedUser = user;
    this.confirmAction = action;
    this.showConfirmModal = true;
  }

  closeConfirmModal(): void {
    this.showConfirmModal = false;
    this.selectedUser = null;
    this.confirmAction = null;
  }

  confirmStatusChange(): void {
    if (!this.selectedUser || !this.confirmAction) return;

    const serviceCall = this.confirmAction === 'deactivate' 
      ? this.adminUsersService.deactivate(this.selectedUser.id)
      : this.adminUsersService.activate(this.selectedUser.id);

    serviceCall.subscribe({
      next: () => {
        const action = this.confirmAction === 'deactivate' ? 'deactivated' : 'activated';
        this.toastService.success(`User ${action} successfully`);
        this.closeConfirmModal();
        this.loadUsers();
      },
      error: (error) => {
        console.error(`Error ${this.confirmAction}ing user:`, error);
        const message = error.error?.message || `Failed to ${this.confirmAction} user`;
        this.toastService.error(message);
      }
    });
  }

  getRoleBadgeClass(role: AdminRole): string {
    switch (role) {
      case AdminRole.SuperAdmin:
        return 'badge-danger';
      case AdminRole.Admin:
        return 'badge-primary';
      case AdminRole.Operator:
        return 'badge-secondary';
      default:
        return 'badge-secondary';
    }
  }

  getStatusBadgeClass(isActive: boolean): string {
    return isActive ? 'badge-success' : 'badge-warning';
  }

  formatDate(date: Date | undefined): string {
    if (!date) return 'Never';
    return new Date(date).toLocaleString();
  }
}
