import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AdminAuthService } from '../services/admin-auth.service';

export const adminAuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(AdminAuthService);
  const router = inject(Router);
  
  if (authService.isAuthenticated()) {
    // Check role-based access if required
    const requiredRoles = route.data['roles'] as string[] | undefined;
    
    if (requiredRoles && requiredRoles.length > 0) {
      if (authService.hasRole(requiredRoles)) {
        return true;
      } else {
        // User doesn't have required role
        router.navigate(['/admin/unauthorized']);
        return false;
      }
    }
    
    return true;
  }
  
  // Not authenticated - redirect to login
  router.navigate(['/admin/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
