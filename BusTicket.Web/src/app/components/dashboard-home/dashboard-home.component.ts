import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DashboardService } from '../../services/dashboard.service';
import { DashboardStatistics } from '../../models/dashboard.model';
import { ToastService } from '../../services/toast.service';

interface StatCard {
  title: string;
  value: number;
  subtitle: string;
  icon: string;
  color: string;
}

@Component({
  selector: 'app-dashboard-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard-home.component.html',
  styleUrls: ['./dashboard-home.component.scss']
})
export class DashboardHomeComponent implements OnInit {
  statistics?: DashboardStatistics;
  isLoading = false;
  
  stats: StatCard[] = [];
  
  quickActions = [
    { title: 'Add Company', icon: '🏢', route: '/admin/companies/new', color: '#667eea' },
    { title: 'Add Bus', icon: '🚌', route: '/admin/buses/new', color: '#10b981' },
    { title: 'Create Schedule', icon: '📅', route: '/admin/schedules/new', color: '#f59e0b' },
    { title: 'View Bookings', icon: '📋', route: '/admin/bookings', color: '#ef4444' }
  ];
  
  constructor(
    private dashboardService: DashboardService,
    private toastService: ToastService,
    private router: Router
  ) {}
  
  ngOnInit(): void {
    this.loadStatistics();
  }
  
  loadStatistics(): void {
    this.isLoading = true;
    this.dashboardService.getStatistics().subscribe({
      next: (data) => {
        this.statistics = data;
        this.buildStatCards(data);
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading statistics:', error);
        this.toastService.error('Failed to load dashboard statistics');
        this.isLoading = false;
      }
    });
  }
  
  buildStatCards(data: DashboardStatistics): void {
    this.stats = [
      {
        title: 'Companies',
        value: data.totalCompanies,
        subtitle: `${data.activeCompanies} Active`,
        icon: '🏢',
        color: '#667eea'
      },
      {
        title: 'Buses',
        value: data.totalBuses,
        subtitle: `${data.activeBuses} Active`,
        icon: '🚌',
        color: '#10b981'
      },
      {
        title: 'Routes',
        value: data.totalRoutes,
        subtitle: `${data.activeRoutes} Active`,
        icon: '🗺️',
        color: '#f59e0b'
      },
      {
        title: 'Bookings',
        value: data.totalBookings,
        subtitle: `${data.todayBookings} Today`,
        icon: '🎫',
        color: '#ef4444'
      }
    ];
  }
  
  getStatusClass(status: string): string {
    switch (status) {
      case 'Confirmed': return 'status-confirmed';
      case 'Pending': return 'status-pending';
      case 'Cancelled': return 'status-cancelled';
      default: return '';
    }
  }
  
  navigateTo(route: string): void {
    this.router.navigate([route]);
  }
  
  viewBookingDetails(id: string): void {
    this.router.navigate(['/admin/bookings'], { queryParams: { id } });
  }
}
