import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { RouteService } from '../../../services/route.service';
import { Route } from '../../../models/route.model';
import { filter } from 'rxjs';

@Component({
  selector: 'app-routes-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './routes-list.component.html',
  styleUrls: ['./routes-list.component.scss']
})
export class RoutesListComponent implements OnInit {
  routes: Route[] = [];
  filteredRoutes: Route[] = [];
  searchTerm: string = '';
  showDeleteModal = false;
  selectedRoute: Route | null = null;

  constructor(
    private routeService: RouteService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadRoutes();
    
    // Reload routes when navigating back to this route
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      if (this.router.url === '/admin/routes' || this.router.url.startsWith('/admin/routes?')) {
        this.loadRoutes();
      }
    });
  }

  loadRoutes(): void {
    this.routeService.getAll().subscribe({
      next: (routes) => {
        this.routes = routes;
        this.filteredRoutes = routes;
      },
      error: (error) => {
        console.error('Error loading routes:', error);
      }
    });
  }

  onSearch(): void {
    if (!this.searchTerm.trim()) {
      this.filteredRoutes = this.routes;
      return;
    }

    const search = this.searchTerm.toLowerCase();
    this.filteredRoutes = this.routes.filter(route =>
      route.fromCity.toLowerCase().includes(search) ||
      route.toCity.toLowerCase().includes(search)
    );
  }

  confirmDelete(route: Route): void {
    this.selectedRoute = route;
    this.showDeleteModal = true;
  }

  deleteRoute(): void {
    if (!this.selectedRoute) return;

    this.routeService.delete(this.selectedRoute.id).subscribe({
      next: () => {
        this.loadRoutes();
        this.showDeleteModal = false;
        this.selectedRoute = null;
      },
      error: (error) => {
        console.error('Error deleting route:', error);
        this.showDeleteModal = false;
      }
    });
  }

  formatDuration(duration: string): string {
    // Duration is in format "HH:MM:SS"
    const parts = duration.split(':');
    const hours = parseInt(parts[0]);
    const minutes = parseInt(parts[1]);

    if (hours > 0 && minutes > 0) {
      return `${hours}h ${minutes}m`;
    } else if (hours > 0) {
      return `${hours}h`;
    } else {
      return `${minutes}m`;
    }
  }

  getStatusClass(isActive: boolean): string {
    return isActive ? 'status-active' : 'status-inactive';
  }

  getStatusText(isActive: boolean): string {
    return isActive ? 'Active' : 'Inactive';
  }
}
