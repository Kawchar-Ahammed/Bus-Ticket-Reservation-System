import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AdminAuthService } from '../services/admin-auth.service';

export const superAdminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AdminAuthService);
  const router = inject(Router);

  const currentUser = authService.getCurrentUserValue();
  
  if (currentUser && currentUser.role === 'SuperAdmin') {
    return true;
  }

  // Not SuperAdmin, redirect to dashboard
  router.navigate(['/admin/dashboard']);
  return false;
};
