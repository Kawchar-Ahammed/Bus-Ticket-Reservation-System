import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BusScheduleService } from '../../../services/bus-schedule.service';
import { BusSchedule } from '../../../models/bus-schedule.model';
import { filter } from 'rxjs';

@Component({
  selector: 'app-bus-schedules-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './bus-schedules-list.component.html',
  styleUrls: ['./bus-schedules-list.component.scss']
})
export class BusSchedulesListComponent implements OnInit {
  schedules: BusSchedule[] = [];
  filteredSchedules: BusSchedule[] = [];
  searchTerm: string = '';
  showDeleteModal = false;
  selectedSchedule: BusSchedule | null = null;

  constructor(
    private busScheduleService: BusScheduleService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSchedules();
    
    // Reload schedules when navigating back to this route
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      if (this.router.url === '/admin/schedules' || this.router.url.startsWith('/admin/schedules?')) {
        this.loadSchedules();
      }
    });
  }

  loadSchedules(): void {
    this.busScheduleService.getAll().subscribe({
      next: (schedules) => {
        this.schedules = schedules;
        this.filteredSchedules = schedules;
      },
      error: (error) => {
        console.error('Error loading schedules:', error);
      }
    });
  }

  onSearch(): void {
    if (!this.searchTerm.trim()) {
      this.filteredSchedules = this.schedules;
      return;
    }

    const search = this.searchTerm.toLowerCase();
    this.filteredSchedules = this.schedules.filter(schedule =>
      schedule.busNumber.toLowerCase().includes(search) ||
      schedule.busName.toLowerCase().includes(search) ||
      schedule.fromCity.toLowerCase().includes(search) ||
      schedule.toCity.toLowerCase().includes(search)
    );
  }

  confirmDelete(schedule: BusSchedule): void {
    this.selectedSchedule = schedule;
    this.showDeleteModal = true;
  }

  deleteSchedule(): void {
    if (!this.selectedSchedule) return;

    this.busScheduleService.delete(this.selectedSchedule.id).subscribe({
      next: () => {
        this.loadSchedules();
        this.showDeleteModal = false;
        this.selectedSchedule = null;
      },
      error: (error) => {
        console.error('Error deleting schedule:', error);
        this.showDeleteModal = false;
      }
    });
  }

  formatTime(timeSpan: string): string {
    // TimeSpan format: "HH:MM:SS"
    const parts = timeSpan.split(':');
    return `${parts[0]}:${parts[1]}`;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
  }

  getStatusClass(isActive: boolean): string {
    return isActive ? 'status-active' : 'status-inactive';
  }

  getStatusText(isActive: boolean): string {
    return isActive ? 'Active' : 'Inactive';
  }

  getSeatClass(availableSeats: number, totalSeats: number): string {
    const percentage = (availableSeats / totalSeats) * 100;
    if (percentage > 50) return 'seats-high';
    if (percentage > 20) return 'seats-medium';
    return 'seats-low';
  }
}
