import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { MaterialModule } from '../../material.module';
import { ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MasterService } from '../../_service/master.service';
import { ToastrService } from 'ngx-toastr';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { PreviewDialogComponent } from './preview-dialog.component';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

interface Invoice {
  invNum: string;
  invoiceNumber: string;
  invDate: string;
  cuName: string;
  coName: string;
  totalAmt: number;
}

@Component({
  selector: 'app-listinvoice',
  standalone: true,
  imports: [MaterialModule, ReactiveFormsModule, RouterLink],
  templateUrl: './listinvoice.component.html',
  styleUrls: ['./listinvoice.component.css'],
})
export class ListinvoiceComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = [
    'invNum',
    'invDate',
    'cuName',
    'coName',
    'totalAmt',
    'actions',
  ];
  dataSource = new MatTableDataSource<Invoice>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  private destroy$ = new Subject<void>();

  constructor(
    private service: MasterService,
    private alert: ToastrService,
    private router: Router,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.LoadInvoice();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  LoadInvoice() {
    this.service.GetAllInvoice()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          let data: any = res;
          if (Array.isArray(data)) {
            // Direct array
          } else if (data && typeof data === 'object') {
            if (Array.isArray(data.data)) {
              data = data.data;
            } else if (Array.isArray(data.result)) {
              data = data.result;
            } else if (Array.isArray(data.invoices)) {
              data = data.invoices;
            } else {
              for (let key in data) {
                if (Array.isArray(data[key])) {
                  data = data[key];
                  break;
                }
              }
            }
          }

          if (Array.isArray(data)) {
            this.dataSource.data = data;
          } else {
            this.alert.error('Invalid response format', 'Error');
          }
        },
        error: (err) => {
          this.alert.error('Failed to load invoices.', 'Error');
        },
      });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  invoiceremove(invoiceno: string) {
    const dialogConfirm = confirm(`Do you want to remove this Invoice: ${invoiceno}?`);
    if (dialogConfirm) {
      this.service.RemoveInvoice(invoiceno)
        .pipe(takeUntil(this.destroy$))
        .subscribe((res: any) => {
          if (res.Result === 'pass' || res.result === 'pass') {
            this.alert.success('Removed Successfully.', 'Remove Invoice');
            this.LoadInvoice();
          } else {
            this.alert.error('Failed to Remove.', 'Invoice');
          }
        });
    }
  }

  Editinvoice(invoiceno: string) {
    this.router.navigate(['/editinvoice', invoiceno]);
  }

  PrintInvoice(invoiceno: string) {
    this.service.GenerateInvoicePDF(invoiceno)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          if (res.body && res.body.size > 0) {
            const blob: Blob = res.body as Blob;
            const url = window.URL.createObjectURL(blob);
            window.open(url, '_blank');
            setTimeout(() => window.URL.revokeObjectURL(url), 1000);
          } else {
            this.alert.error('PDF file is empty', 'Error');
          }
        },
        error: (err) => {
          this.alert.error(`Failed to print invoice ${invoiceno}`, 'Error');
        },
      });
  }

  DownloadInvoice(invoiceno: string) {
    this.service.GenerateInvoicePDF(invoiceno)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          if (res.body && res.body.size > 0) {
            const blob: Blob = res.body as Blob;
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.download = `Invoice_${invoiceno.replace('/', '_')}.pdf`;
            a.href = url;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            window.URL.revokeObjectURL(url);
          } else {
            this.alert.error('PDF file is empty', 'Error');
          }
        },
        error: (err) => {
          this.alert.error(`Failed to download invoice ${invoiceno}`, 'Error');
        },
      });
  }

  PreviewInvoice(invoiceno: string) {
    this.service.GenerateInvoicePDF(invoiceno)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          if (res.body && res.body.size > 0) {
            const blob = new Blob([res.body], { type: 'application/pdf' });
            const url = URL.createObjectURL(blob);
            const isMobile = window.innerWidth < 768;
            
            const dialogRef = this.dialog.open(PreviewDialogComponent, {
              width: isMobile ? '100vw' : '80%',
              height: isMobile ? '100vh' : '80%',
              maxWidth: isMobile ? '100vw' : 'none',
              data: { pdfurl: url, invoiceno },
            });
            
            dialogRef.afterClosed().subscribe(() => {
              URL.revokeObjectURL(url);
            });
          } else {
            this.alert.error('PDF file is empty', 'Error');
          }
        },
        error: (err) => {
          this.alert.error(`Failed to preview invoice ${invoiceno}`, 'Error');
        },
      });
  }
}