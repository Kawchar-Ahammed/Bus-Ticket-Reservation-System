import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { BusService } from '../../services/bus.service';
import { Bus } from '../../models/bus.model';
import { filter } from 'rxjs';

@Component({
  selector: 'app-buses-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './buses-list.component.html',
  styleUrl: './buses-list.component.scss'
})
export class BusesListComponent implements OnInit {
  private busService = inject(BusService);
  private router = inject(Router);

  buses: Bus[] = [];
  filteredBuses: Bus[] = [];
  loading = false;
  searchTerm = '';
  showDeleteConfirm = false;
  busToDelete: Bus | null = null;

  ngOnInit(): void {
    this.loadBuses();
    
    // Reload buses when navigating back to this route
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      if (this.router.url === '/admin/buses' || this.router.url.startsWith('/admin/buses?')) {
        this.loadBuses();
      }
    });
  }

  loadBuses(): void {
    this.loading = true;
    this.busService.getAll().subscribe({
      next: (buses) => {
        this.buses = buses;
        this.filteredBuses = buses;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading buses:', error);
        this.loading = false;
      }
    });
  }

  onSearch(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchTerm = target.value.toLowerCase();

    this.filteredBuses = this.buses.filter(bus =>
      bus.busNumber.toLowerCase().includes(this.searchTerm) ||
      bus.busName.toLowerCase().includes(this.searchTerm) ||
      bus.companyName.toLowerCase().includes(this.searchTerm)
    );
  }

  confirmDelete(bus: Bus): void {
    this.busToDelete = bus;
    this.showDeleteConfirm = true;
  }

  cancelDelete(): void {
    this.showDeleteConfirm = false;
    this.busToDelete = null;
  }

  deleteBus(): void {
    if (!this.busToDelete) return;

    this.busService.delete(this.busToDelete.id).subscribe({
      next: () => {
        this.loadBuses();
        this.cancelDelete();
      },
      error: (error) => {
        console.error('Error deleting bus:', error);
        this.cancelDelete();
      }
    });
  }

  getStatusClass(isActive: boolean): string {
    return isActive ? 'status-active' : 'status-inactive';
  }

  getStatusText(isActive: boolean): string {
    return isActive ? 'Active' : 'Inactive';
  }
}
