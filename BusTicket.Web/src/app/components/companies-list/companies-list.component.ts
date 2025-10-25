import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { CompanyService } from '../../services/company.service';
import { Company } from '../../models/company.model';
import { filter, Subscription } from 'rxjs';

@Component({
  selector: 'app-companies-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './companies-list.component.html',
  styleUrls: ['./companies-list.component.scss']
})
export class CompaniesListComponent implements OnInit, OnDestroy {
  private companyService = inject(CompanyService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private routerSubscription?: Subscription;
  
  companies: Company[] = [];
  filteredCompanies: Company[] = [];
  loading = false;
  searchTerm = '';
  showDeleteConfirm = false;
  companyToDelete: Company | null = null;

  ngOnInit(): void {
    // Initial load
    this.loadCompanies();
    
    // Subscribe to route activation to reload data when navigating back
    this.routerSubscription = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      // Check if we're on the companies route
      if (event.url === '/admin/companies') {
        this.loadCompanies();
      }
    });
  }

  ngOnDestroy(): void {
    this.routerSubscription?.unsubscribe();
  }

  loadCompanies(): void {
    this.loading = true;
    this.companyService.getAll().subscribe({
      next: (companies) => {
        this.companies = companies;
        this.filteredCompanies = companies;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading companies:', error);
        this.loading = false;
      }
    });
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm = input.value.toLowerCase();
    
    this.filteredCompanies = this.companies.filter(company =>
      company.name.toLowerCase().includes(this.searchTerm) ||
      company.email?.toLowerCase().includes(this.searchTerm) ||
      company.contactNumber?.toLowerCase().includes(this.searchTerm)
    );
  }

  confirmDelete(company: Company): void {
    this.companyToDelete = company;
    this.showDeleteConfirm = true;
  }

  cancelDelete(): void {
    this.companyToDelete = null;
    this.showDeleteConfirm = false;
  }

  deleteCompany(): void {
    if (!this.companyToDelete) return;

    this.companyService.delete(this.companyToDelete.id).subscribe({
      next: () => {
        this.loadCompanies();
        this.cancelDelete();
      },
      error: (error) => {
        console.error('Error deleting company:', error);
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
