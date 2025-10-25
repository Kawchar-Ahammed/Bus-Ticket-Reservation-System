import { Routes } from '@angular/router';
import { BusSearch } from './components/bus-search/bus-search';
import { SeatSelection } from './components/seat-selection/seat-selection';
import { BookingConfirmation } from './components/booking-confirmation/booking-confirmation';
import { BookingHistory } from './components/booking-history/booking-history';
import { PrintTicket } from './components/print-ticket/print-ticket';
import { AdminLoginComponent } from './components/admin-login/admin-login.component';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { DashboardHomeComponent } from './components/dashboard-home/dashboard-home.component';
import { CompaniesListComponent } from './components/companies-list/companies-list.component';
import { CompanyFormComponent } from './components/company-form/company-form.component';
import { BusesListComponent } from './components/buses-list/buses-list.component';
import { BusFormComponent } from './components/bus-form/bus-form.component';
import { RoutesListComponent } from './components/admin/routes-list/routes-list.component';
import { RouteFormComponent } from './components/admin/route-form/route-form.component';
import { BusSchedulesListComponent } from './components/admin/bus-schedules-list/bus-schedules-list.component';
import { BusScheduleFormComponent } from './components/admin/bus-schedule-form/bus-schedule-form.component';
import { AdminBookingsListComponent } from './components/admin/admin-bookings-list/admin-bookings-list.component';
import { AdminUsersListComponent } from './components/admin/admin-users-list/admin-users-list.component';
import { AdminUserFormComponent } from './components/admin/admin-user-form/admin-user-form.component';
import { adminAuthGuard } from './guards/admin-auth.guard';
import { superAdminGuard } from './guards/super-admin.guard';

export const routes: Routes = [
  // Customer Routes
  { path: '', component: BusSearch },
  { path: 'seats/:scheduleId', component: SeatSelection },
  { path: 'confirm', component: BookingConfirmation },
  { path: 'my-bookings', component: BookingHistory },
  { path: 'print-ticket/:ticketNumber', component: PrintTicket },
  
  // Admin Routes
  { 
    path: 'admin/login', 
    component: AdminLoginComponent 
  },
  { 
    path: 'admin', 
    component: AdminDashboardComponent,
    canActivate: [adminAuthGuard],
    children: [
      { 
        path: '', 
        redirectTo: 'dashboard', 
        pathMatch: 'full' 
      },
      { 
        path: 'dashboard', 
        component: DashboardHomeComponent 
      },
      // Placeholder routes for future implementation
      { 
        path: 'companies', 
        component: CompaniesListComponent
      },
      { 
        path: 'companies/new', 
        component: CompanyFormComponent
      },
      { 
        path: 'companies/:id', 
        component: CompanyFormComponent
      },
      { 
        path: 'buses', 
        component: BusesListComponent
      },
      { 
        path: 'buses/new', 
        component: BusFormComponent
      },
      { 
        path: 'buses/:id', 
        component: BusFormComponent
      },
      { 
        path: 'routes', 
        component: RoutesListComponent
      },
      { 
        path: 'routes/new', 
        component: RouteFormComponent
      },
      { 
        path: 'routes/:id', 
        component: RouteFormComponent
      },
      { 
        path: 'schedules', 
        component: BusSchedulesListComponent
      },
      { 
        path: 'schedules/new', 
        component: BusScheduleFormComponent
      },
      { 
        path: 'schedules/:id', 
        component: BusScheduleFormComponent
      },
      { 
        path: 'bookings', 
        component: AdminBookingsListComponent
      },
      { 
        path: 'users', 
        component: AdminUsersListComponent,
        canActivate: [superAdminGuard]
      },
      { 
        path: 'users/create', 
        component: AdminUserFormComponent,
        canActivate: [superAdminGuard]
      },
      { 
        path: 'users/edit/:id', 
        component: AdminUserFormComponent,
        canActivate: [superAdminGuard]
      },
      { 
        path: 'settings', 
        component: DashboardHomeComponent // TODO: Replace with SettingsComponent
      }
    ]
  },
  
  { path: '**', redirectTo: '' }
];


