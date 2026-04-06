import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../material.module';
import { LedgerService } from '../../../_service/ledger.service';
import { MatDialog } from '@angular/material/dialog';
import { PaymentDialogComponent } from '../payment-dialog/payment-dialog.component';
import { PaymentDetailsDialogComponent } from '../payment-details-dialog/payment-details-dialog.component';
import { CustomerDetailsDialogComponent } from '../customer-details-dialog/customer-details-dialog.component';
import { AuthService } from '../../../_service/authentication.service';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { customerOutstanding, paymentEntryRequest, ledgerApiResponse } from '../../../_model/ledger.model';


@Component({
  selector: 'app-outstanding-ar',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './outstanding-ar.component.html',
  styleUrls: ['./outstanding-ar.component.css']
})
export class OutstandingARComponent implements OnInit, OnDestroy, AfterViewInit {
  // Data properties
  displayedColumns: string[] = ['customerName', 'balance', 'daysOutstanding', 'lastPaymentDate', 'status', 'actions'];
  dataSource = new MatTableDataSource<customerOutstanding>();

  // UI properties
  loading = true;
  error: string | null = null;
  companyId: string = '';
  currentPage = 1;
  pageSize = 10;

  // ViewChild references
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  // Lifecycle
  private destroy$ = new Subject<void>();

  constructor(
    private ledgerService: LedgerService,
    private dialog: MatDialog,
    private authService: AuthService,
    private toastr: ToastrService
  ) {
    this.companyId = this.authService.getCompanyId() || '';
  }

  ngOnInit(): void {
    this.loadOutstandingCustomers();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Load outstanding customers from API
   */
  loadOutstandingCustomers(): void {
    this.loading = true;
    this.error = null;

    this.ledgerService.getCustomersOutstanding(this.companyId, this.currentPage, this.pageSize)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          if (response.result === 'pass' && response.data) {
            const customers = Array.isArray(response.data) ? response.data : [response.data];
            this.dataSource.data = customers as customerOutstanding[];
          } else {
            this.error = response.errorMessage || 'Failed to load outstanding customers';
            this.toastr.error('Failed to load data', 'Error');
          }
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Error: ' + err.message;
          this.toastr.error('Failed to load outstanding customers', 'Error');
          this.loading = false;
        }
      });
  }

  /**
   * Apply filter to table
   */
  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  /**
   * Format currency
   */
  formatCurrency(value: number | undefined): string {
    if (!value) return '₹0.00';
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(value);
  }

  /**
   * Format date
   */
  formatDate(dateString: string | null | undefined): string {
    if (!dateString) return 'N/A';
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-IN');
    } catch {
      return dateString;
    }
  }

  /**
   * Get CSS class for days outstanding status
   */
  getStatusCss(daysOutstanding: number | undefined): string {
    if (!daysOutstanding) return 'status-ok';
    if (daysOutstanding <= 30) return 'status-ok';
    if (daysOutstanding <= 60) return 'status-warning';
    if (daysOutstanding <= 90) return 'status-alert';
    return 'status-danger';
  }

  /**
   * Get status label
   */
  getStatusLabel(daysOutstanding: number | undefined): string {
    if (!daysOutstanding) return 'Current';
    if (daysOutstanding <= 30) return 'Due Soon';
    if (daysOutstanding <= 60) return 'Overdue';
    if (daysOutstanding <= 90) return 'Heavily Overdue';
    return 'Severely Overdue';
  }

  /**
   * View customer details
   */
  viewCustomer(customerId: string | undefined): void {
    if (!customerId) return;
    this.toastr.info(`Opening details for customer: ${customerId}`);
    this.openCustomerDetailModal(customerId);
  }

  /**
   * Open customer detail modal showing ledger information
   */
  openCustomerDetailModal(customerId: string): void {
    this.ledgerService.getCustomerLedger(customerId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: any) => {
          // Handle direct object response (not wrapped in APIResponse)
          const customer = response.data || response;
          
          if (customer && customer.customerId) {
            const dialogRef = this.dialog.open(CustomerDetailsDialogComponent, {
              width: '1000px',
              data: { customer }
            });

            dialogRef.afterClosed().pipe(takeUntil(this.destroy$)).subscribe(() => {
              // Dialog closed, no action needed
            });
          } else {
            this.toastr.error('Failed to load customer details', 'Error');
          }
        },
        error: (err) => {
          this.toastr.error('Error loading customer details: ' + err.message, 'Error');
        }
      });
  }

  /**
   * View payments for a customer - opens modal with payment history
   */
  viewPayments(customerId: string | undefined, customerName: string | undefined): void {
    if (!customerId) return;

    const dialogRef = this.dialog.open(PaymentDetailsDialogComponent, {
      width: '900px',
      data: { customerId, customerName: customerName || 'Customer' }
    });

    dialogRef.afterClosed().pipe(takeUntil(this.destroy$)).subscribe(() => {
      // Modal closed, no action needed
    });
  }

  /**
   * Send reminder
   */
  sendReminder(customerId: string | undefined, customerName: string | undefined): void {
    if (!customerId) return;
    this.toastr.info(`Sending reminder to ${customerName}...`, 'Reminder');
    // TODO: Implement send reminder functionality
  }

  /**
   * Prompt user for payment amount and optional invoice, then record the payment
   */
  openPaymentPrompt(element: customerOutstanding | undefined): void {
    if (!element || !element.customerId) return;

    const dialogRef = this.dialog.open(PaymentDialogComponent, {
      width: '420px',
      data: { customerId: element.customerId, customerName: element.customerName, companyId: this.companyId }
    });

    dialogRef.afterClosed().pipe(takeUntil(this.destroy$)).subscribe((result: any) => {
      if (result?.ok) {
        this.toastr.success('Payment recorded successfully', 'Payment');
        this.refreshData();
      } else if (result?.error) {
        this.toastr.error(result.error || 'Failed to record payment', 'Payment');
      }
    });
  }



  /**
   * Refresh data
   */
  refreshData(): void {
    this.currentPage = 1;
    this.loadOutstandingCustomers();
    this.toastr.info('Data refreshed', 'Refresh');
  }
}
