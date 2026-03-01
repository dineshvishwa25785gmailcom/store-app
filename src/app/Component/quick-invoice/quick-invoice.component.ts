import { Component, QueryList, ViewChildren, ElementRef, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../../material.module';
import { jsPDF } from 'jspdf';
import { QuickInvoiceService } from '../../_service/quick-invoice.service';
import { ProductService } from '../../_service/product.service';
import { CustomerService } from '../../_service/customer.service';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';
import { customer } from '../../_model/customer.model';
import { ProductDTO } from '../../_model/product.model';
import { QuickInvoiceItem } from '../../_model/quick-invoice.model';
import { MatDialog } from '@angular/material/dialog';
import { InvoiceListDialogComponent } from '../../invoice-list-dialog/invoice-list-dialog.component';


interface InvoiceItem extends QuickInvoiceItem {
  product: string;
  quantity: number;
  rate: number;
  total: number;
}

@Component({
  selector: 'app-quick-invoice',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './quick-invoice.component.html',
  styleUrl: './quick-invoice.component.css'
})
export class QuickInvoiceComponent implements OnInit, OnDestroy {
  items: InvoiceItem[] = [];
  today = new Date();
  customerName: string = '';
  customerId: string = '';

  customers: customer[] = [];
  filteredCustomers: customer[] = [];
  customerSearchSubject = new Subject<string>();

  products: ProductDTO[] = [];
  filteredProducts: ProductDTO[] = [];
  productSearchSubject = new Subject<string>();

  isLoading = false;
  invoices: any[] = [];
  selectedInvoiceId: string = '';
  
  @ViewChildren('productInput') productInputs!: QueryList<ElementRef>;
  private destroy$ = new Subject<void>();

  showInvoiceList = false;
  hoveredInvoiceId: string | null = null;
  hoveredInvoiceItems: any[] = [];

  constructor(
    private quickInvoiceService: QuickInvoiceService,
    private productService: ProductService,
    private customerService: CustomerService,
    private toastr: ToastrService,private dialog: MatDialog
  ) {}

  ngOnInit() {
    this.loadCustomers();
    this.loadProducts();
    this.loadInvoices();
    this.setupCustomerSearch();
    this.setupProductSearch();
  }

toggleInvoiceList() {
  this.dialog.open(InvoiceListDialogComponent, {
    width: '800px',
    data: { 
      invoices: this.invoices,
      performInvoiceAction: this.performInvoiceAction.bind(this)
    }
  });
}


  hoverInvoice(inv: any) {
    this.hoveredInvoiceId = inv.quickInvoiceId ?? inv.QuickInvoiceId ?? (inv.id?.toString ? inv.id.toString() : null);
    const items = inv.items ?? inv.Items ?? null;
    if (items && Array.isArray(items)) {
      this.hoveredInvoiceItems = items;
    } else {
      const id = this.hoveredInvoiceId;
      if (id) {
        this.quickInvoiceService.getInvoiceById(id).pipe(takeUntil(this.destroy$)).subscribe({
          next: (resp: any) => {
            if (Array.isArray(resp)) {
              this.invoices = resp;
            } else if (resp && Array.isArray(resp.data)) {
              this.invoices = resp.data;
            } else if (resp && Array.isArray(resp.items)) {
              this.invoices = resp.items;
            } else {
              this.invoices = resp ? [resp] : [];
            }
          },
          error: (err) => {
            console.error('Error loading invoice details:', err);
            this.toastr.error('Failed to load invoice details');
          }
        });
      }
    }
  }

  clearHover() {
    this.hoveredInvoiceId = null;
    this.hoveredInvoiceItems = [];
  }

  onInvoiceSelectionChange(event: any) {
    const invoiceId = event.value;
    if (invoiceId) {
      this.loadInvoice(invoiceId);
    }
  }

  performInvoiceAction(id: string, action: 'edit' | 'print' | 'delete' | 'share' | 'download') {
    if (!id) return;
    this.quickInvoiceService.getInvoiceById(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        const dto = res?.data ?? res;
        this.selectedInvoiceId = dto?.quickInvoiceId ?? dto?.QuickInvoiceId ?? id;
        this.customerName = dto?.customerName ?? dto?.CustomerName ?? '';
        this.customerId = dto?.customerId ?? dto?.CustomerId ?? '';
        this.items = (dto?.items ?? dto?.Items ?? []).map((item: any) => ({
          product: item.productName ?? item.ProductName ?? item.product ?? '',
          productId: item.productId ?? item.ProductId ?? '',
          productName: item.productName ?? item.ProductName ?? item.product ?? '',
          quantity: item.quantity ?? item.Quantity ?? 0,
          rate: item.rate ?? item.Rate ?? 0,
          total: item.total ?? item.Total ?? 0
        }));

        if (action === 'print') {
          setTimeout(() => this.printInvoice(), 200);
        } else if (action === 'download') {
          setTimeout(() => this.downloadPDF(), 200);
        } else if (action === 'share') {
          setTimeout(() => this.shareToWhatsApp(), 200);
        } else if (action === 'delete') {
          this.deleteInvoiceRecord(id);
        }
      },
      error: (err) => {
        console.error('Error loading invoice for action:', err);
        this.toastr.error('Failed to load invoice');
      }
    });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // ---------------------------
  // ALL OTHER METHODS
  // ---------------------------

 loadCustomers() {
  this.isLoading = true;
  this.customerService.Getall()
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (data: customer[]) => {
        this.customers = data;
        this.filteredCustomers = data;   // ✅ ensure filtered list is populated
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading customers:', err);
        this.toastr.error('Failed to load customers');
        this.isLoading = false;
      }
    });
}
 loadProducts() {
    this.productService.getAllProducts()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data: any) => {
          this.products = data;
          this.filteredProducts = data;
        },
        error: (err) => {
          console.error('Error loading products:', err);
          this.toastr.error('Failed to load products');
        }
      });
  }

  loadInvoices() {
    this.quickInvoiceService.getAllInvoices()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data: any) => {
          if (Array.isArray(data)) {
            this.invoices = data;
          } else if (data && Array.isArray(data.data)) {
            this.invoices = data.data;
          } else if (data && Array.isArray(data.items)) {
            this.invoices = data.items;
          } else {
            this.invoices = data ? [data] : [];
          }
        },
        error: (err) => {
          console.error('Error loading invoices:', err);
          this.toastr.error('Failed to load invoices');
        }
      });
  }

  setupCustomerSearch() {
    this.customerSearchSubject
      .pipe(debounceTime(300), takeUntil(this.destroy$))
      .subscribe((searchTerm) => {
        if (!searchTerm.trim()) {
          this.filteredCustomers = this.customers;
        } else {
          const term = searchTerm.toLowerCase();
          this.filteredCustomers = this.customers.filter(c =>
            c.name.toLowerCase().includes(term) ||
            c.email.toLowerCase().includes(term) ||
            c.phone.includes(term)
          );
        }
      });
  }

  setupProductSearch() {
    this.productSearchSubject
      .pipe(debounceTime(300), takeUntil(this.destroy$))
      .subscribe((searchTerm) => {
        if (!searchTerm.trim()) {
          this.filteredProducts = this.products;
        } else {
          const term = searchTerm.toLowerCase();
          this.filteredProducts = this.products.filter(p =>
            p.productName?.toLowerCase().includes(term)
          );
        }
      });
  }

  onCustomerSearch(event: any) {
    const searchTerm = event.target.value;
    this.customerSearchSubject.next(searchTerm);
  }

  selectCustomer(customer: customer) {
    this.customerName = customer.name ?? '';
    this.customerId = customer.uniqueKeyID ?? '';
  }

  onProductSearch(event: any, item: InvoiceItem) {
    const searchTerm = event.target.value;
    this.productSearchSubject.next(searchTerm);
  }

  selectProduct(product: ProductDTO, item: InvoiceItem) {
    item.product = product.productName ?? '';
    item.productId = product.uniqueKeyID ?? '';
    item.rate = product.rateWithTax ?? product.rateWithoutTax ?? 0;
    this.calculateTotal(item);
  }
addProduct() {
    this.items.push({
      product: '',
      productId: undefined,
      quantity: 1,
      rate: 0,
      total: 0
    });

    setTimeout(() => {
      const inputs = this.productInputs.toArray();
      if (inputs.length > 0) {
        inputs[inputs.length - 1].nativeElement.focus();
      }
    }, 100);
  }

// ---------------------------
  // Calculations
  // ---------------------------

  calculateTotal(item: InvoiceItem) {
    if (item.quantity && item.rate) {
      item.total = Math.round(item.quantity * item.rate * 100) / 100;
    } else {
      item.total = 0;
    }
  }

  deleteItem(index: number) {
    this.items.splice(index, 1);
  }

  getGrandTotal(): number {
    return Math.round(this.items.reduce((sum, item) => sum + item.total, 0) * 100) / 100;
  }

  isValidForPrint(): boolean {
    return this.items.length > 0 && this.items.every(item =>
      item.product && item.product.trim() &&
      item.quantity != null && item.quantity > 0 &&
      item.rate != null && item.rate > 0
    );
  }


// ---------------------------
  // Invoice Actions
  // ---------------------------

  saveInvoice() {
    if (!this.customerName.trim()) {
      this.toastr.warning('Please enter or select a customer');
      return;
    }

    if (!this.isValidForPrint()) {
      this.toastr.warning('Please fill all product, quantity and rate fields with valid values');
      return;
    }

    const invoice = {
      quickInvoiceId: this.selectedInvoiceId || undefined,
      customerId: this.customerId || undefined,
      customerName: this.customerName,
      grandTotal: this.getGrandTotal(),
      items: this.items.map(item => ({
        productId: item.productId ?? undefined,
        productName: item.product,
        quantity: item.quantity,
        rate: item.rate,
        total: item.total
      }))
    };

    const saveOperation = this.selectedInvoiceId
      ? this.quickInvoiceService.updateInvoice(invoice)
      : this.quickInvoiceService.createInvoice(invoice);

    saveOperation.pipe(takeUntil(this.destroy$)).subscribe({
      next: (response) => {
        this.toastr.success(response.message || 'Invoice saved successfully');
        this.selectedInvoiceId = response.data?.quickInvoiceId ?? '';
        this.loadInvoices();
      },
      error: (err) => {
        console.error('Error saving invoice:', err);
        this.toastr.error('Failed to save invoice');
      }
    });
  }

 

loadInvoice(selectedInvoiceId: string) {
  if (!selectedInvoiceId) return;

  this.quickInvoiceService.getInvoiceById(selectedInvoiceId)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (invoice: any) => {
        const dto = invoice?.data ?? invoice;

        this.selectedInvoiceId = dto?.quickInvoiceId ?? '';
        this.customerName = dto?.customerName ?? '';
        this.customerId = dto?.customerId ?? '';

        this.items = (dto?.items ?? []).map((item: any) => ({
          product: item.productName ?? item.product ?? '',
          productId: item.productId ?? '',
          productName: item.productName ?? item.product ?? '',
          quantity: item.quantity ?? 0,
          rate: item.rate ?? 0,
          total: item.total ?? 0
        }));

        this.toastr.success('Invoice loaded successfully');
      },
      error: (err) => {
        console.error('Error loading invoice:', err);
        this.toastr.error('Failed to load invoice');
      }
    });
}
  newInvoice() {
  this.selectedInvoiceId = '';
  this.customerName = '';
  this.customerId = '';
  this.items = [];

  this.toastr.info('Ready to create a new invoice');
}
 deleteInvoiceRecord(invoiceId: string) {
  if (!invoiceId) {
    this.toastr.warning('No invoice selected to delete');
    return;
  }

  this.quickInvoiceService.deleteInvoice(invoiceId)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (response: any) => {
        this.toastr.success(response.message || 'Invoice deleted successfully');
        // Clear current selection if it was the one deleted
        if (this.selectedInvoiceId === invoiceId) {
          this.selectedInvoiceId = '';
          this.customerName = '';
          this.customerId = '';
          this.items = [];
        }
        this.loadInvoices(); // refresh list
      },
      error: (err) => {
        console.error('Error deleting invoice:', err);
        this.toastr.error('Failed to delete invoice');
      }
    });
}
  printInvoice() {
  if (!this.isValidForPrint()) {
    this.toastr.warning('Invoice is not valid for printing');
    return;
  }

  const printContents = document.getElementById('print-section')?.innerHTML;
  if (!printContents) {
    this.toastr.error('Print section not found');
    return;
  }

  const popupWin = window.open('', '_blank', 'width=800,height=600');
  if (popupWin) {
    popupWin.document.open();
    popupWin.document.write(`
      <html>
        <head>
          <title>Invoice</title>
          <style>
            body { font-family: Arial, sans-serif; margin: 20px; }
            table { width: 100%; border-collapse: collapse; margin-top: 20px; }
            th, td { border: 1px solid #333; padding: 8px; text-align: left; }
            th { background-color: #f2f2f2; }
            .grand-total { margin-top: 20px; font-weight: bold; }
          </style>
        </head>
        <body onload="window.print();window.close()">
          ${printContents}
        </body>
      </html>
    `);
    popupWin.document.close();
  }
}
 async shareToWhatsApp() {
  if (!this.isValidForPrint()) {
    alert('Please fill in quantity and rate fields with valid values before sharing.');
    return;
  }

  try {
    // Generate PDF
    const doc = new jsPDF();
    const dateStr = new Date().toLocaleDateString('en-GB');

    // Outer border shadow
    doc.setFillColor(200, 200, 200);
    doc.roundedRect(17, 12, 176, 0, 3, 3, 'S');

    const tableStartY = 52;
    const rowHeight = 10;
    const totalHeight = tableStartY + rowHeight + (this.items.length * rowHeight) + rowHeight + 8;

    doc.setDrawColor(245, 245, 245);
    doc.setLineWidth(0.5);
    doc.roundedRect(15, 10, 180, totalHeight, 3, 3, 'S');

    doc.setFontSize(22);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(102, 126, 234);
    doc.text('QUICK INVOICE', 105, 22, { align: 'center' });

    doc.setDrawColor(200, 200, 200);
    doc.setLineWidth(0.3);
    doc.line(20, 28, 190, 28);

    doc.setFontSize(11);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(0, 0, 0);
    doc.text(`Date: ${dateStr}`, 20, 38);
    doc.text(`Customer: ${this.customerName || 'N/A'}`, 120, 38);

    doc.line(20, 44, 190, 44);

    const startY = tableStartY;
    const colX = [20, 40, 90, 120, 150];

    // Header row
    doc.setFillColor(102, 126, 234);
    doc.rect(20, startY, 170, rowHeight, 'F');
    doc.setDrawColor(80, 80, 80);
    doc.setLineWidth(0.3);
    doc.rect(20, startY, 170, rowHeight);

    colX.slice(1).forEach(x => {
      doc.line(x, startY, x, startY + rowHeight);
    });

    doc.setFontSize(11);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(255, 255, 255);
    doc.text('Sl.No', 30, startY + 6.5, { align: 'center' });
    doc.text('Product Name', 65, startY + 6.5, { align: 'center' });
    doc.text('Qty', 105, startY + 6.5, { align: 'center' });
    doc.text('Rate', 135, startY + 6.5, { align: 'center' });
    doc.text('Total', 170, startY + 6.5, { align: 'center' });

    // Items
    doc.setTextColor(0, 0, 0);
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(10);

    let yPos = startY + rowHeight;
    this.items.forEach((item, index) => {
      if (index % 2 === 0) {
        doc.setFillColor(245, 245, 245);
        doc.rect(20, yPos, 170, rowHeight, 'F');
      }

      doc.setDrawColor(200, 200, 200);
      doc.rect(20, yPos, 170, rowHeight);

      colX.slice(1).forEach(x => {
        doc.line(x, yPos, x, yPos + rowHeight);
      });

      doc.text((index + 1).toString(), 30, yPos + 6.5, { align: 'center' });
      doc.text(item.product || '', 65, yPos + 6.5, { align: 'center' });
      doc.text(item.quantity?.toString() || '0', 105, yPos + 6.5, { align: 'center' });
      doc.text((item.rate?.toFixed(2) || '0.00'), 135, yPos + 6.5, { align: 'center' });
      doc.text((item.total.toFixed(2)), 170, yPos + 6.5, { align: 'center' });

      yPos += rowHeight;
    });

    // Grand total row
    doc.setFillColor(240, 240, 255);
    doc.rect(20, yPos, 170, rowHeight, 'F');
    doc.setDrawColor(80, 80, 80);
    doc.setLineWidth(0.5);
    doc.rect(20, yPos, 170, rowHeight);
    doc.line(150, yPos, 150, yPos + rowHeight);

    doc.setFont('helvetica', 'bold');
    doc.setFontSize(12);
    doc.text('GRAND TOTAL:', 85, yPos + 7, { align: 'center' });
    doc.text(`Rs. ${this.getGrandTotal().toFixed(2)}`, 170, yPos + 7, { align: 'center' });

    // Footer
    doc.setFontSize(8);
    doc.setFont('helvetica', 'italic');
    doc.setTextColor(100, 100, 100);
    doc.text('Thank you for your business!', 105, 280, { align: 'center' });

    // Get PDF as blob
    const pdfBlob = doc.output('blob');
    const fileName = `invoice-${dateStr.replace(/\//g, '-')}.pdf`;
    const file = new File([pdfBlob], fileName, { type: 'application/pdf' });

    // Share if supported
    if (navigator.share && navigator.canShare && navigator.canShare({ files: [file] })) {
      await navigator.share({
        files: [file],
        title: 'Quick Invoice',
        text: `Invoice for ${this.customerName || 'Customer'} - Total: Rs.${this.getGrandTotal().toFixed(2)}`
      });
    } else {
      // Fallback: download the PDF
      alert('Sharing is not supported on this device. The PDF will be downloaded instead.');
      doc.save(fileName);
    }
  } catch (error) {
    alert('Unable to share. Please try downloading the PDF instead.');
  }
}
  downloadPDF() {
  if (!this.isValidForPrint()) {
    this.toastr.warning('Invoice is not valid for download');
    return;
  }

  try {
    const doc = new jsPDF();
    const dateStr = new Date().toLocaleDateString('en-GB');

    // Outer border
    doc.setFillColor(200, 200, 200);
    doc.roundedRect(17, 12, 176, 0, 3, 3, 'S');

    const tableStartY = 52;
    const rowHeight = 10;
    const totalHeight = tableStartY + rowHeight + (this.items.length * rowHeight) + rowHeight + 8;

    doc.setDrawColor(245, 245, 245);
    doc.setLineWidth(0.5);
    doc.roundedRect(15, 10, 180, totalHeight, 3, 3, 'S');

    doc.setFontSize(22);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(102, 126, 234);
    doc.text('QUICK INVOICE', 105, 22, { align: 'center' });

    doc.setDrawColor(200, 200, 200);
    doc.setLineWidth(0.3);
    doc.line(20, 28, 190, 28);

    doc.setFontSize(11);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(0, 0, 0);
    doc.text(`Date: ${dateStr}`, 20, 38);
    doc.text(`Customer: ${this.customerName || 'N/A'}`, 120, 38);

    doc.line(20, 44, 190, 44);

    const startY = tableStartY;
    const colX = [20, 40, 90, 120, 150];

    // Header row
    doc.setFillColor(102, 126, 234);
    doc.rect(20, startY, 170, rowHeight, 'F');
    doc.setDrawColor(80, 80, 80);
    doc.setLineWidth(0.3);
    doc.rect(20, startY, 170, rowHeight);

    colX.slice(1).forEach(x => {
      doc.line(x, startY, x, startY + rowHeight);
    });

    doc.setFontSize(11);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(255, 255, 255);
    doc.text('Sl.No', 30, startY + 6.5, { align: 'center' });
    doc.text('Product Name', 65, startY + 6.5, { align: 'center' });
    doc.text('Qty', 105, startY + 6.5, { align: 'center' });
    doc.text('Rate', 135, startY + 6.5, { align: 'center' });
    doc.text('Total', 170, startY + 6.5, { align: 'center' });

    // Items
    doc.setTextColor(0, 0, 0);
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(10);

    let yPos = startY + rowHeight;
    this.items.forEach((item, index) => {
      if (index % 2 === 0) {
        doc.setFillColor(245, 245, 245);
        doc.rect(20, yPos, 170, rowHeight, 'F');
      }

      doc.setDrawColor(200, 200, 200);
      doc.rect(20, yPos, 170, rowHeight);

      colX.slice(1).forEach(x => {
        doc.line(x, yPos, x, yPos + rowHeight);
      });

      doc.text((index + 1).toString(), 30, yPos + 6.5, { align: 'center' });
      doc.text(item.product || '', 65, yPos + 6.5, { align: 'center' });
      doc.text(item.quantity?.toString() || '0', 105, yPos + 6.5, { align: 'center' });
      doc.text((item.rate?.toFixed(2) || '0.00'), 135, yPos + 6.5, { align: 'center' });
      doc.text((item.total.toFixed(2)), 170, yPos + 6.5, { align: 'center' });

      yPos += rowHeight;
    });

    // Grand total row
    doc.setFillColor(240, 240, 255);
    doc.rect(20, yPos, 170, rowHeight, 'F');
    doc.setDrawColor(80, 80, 80);
    doc.setLineWidth(0.5);
    doc.rect(20, yPos, 170, rowHeight);
    doc.line(150, yPos, 150, yPos + rowHeight);

    doc.setFont('helvetica', 'bold');
    doc.setFontSize(12);
    doc.text('GRAND TOTAL:', 85, yPos + 7, { align: 'center' });
    doc.text(`Rs. ${this.getGrandTotal().toFixed(2)}`, 170, yPos + 7, { align: 'center' });

    // Footer
    doc.setFontSize(8);
    doc.setFont('helvetica', 'italic');
    doc.setTextColor(100, 100, 100);
    doc.text('Thank you for your business!', 105, 280, { align: 'center' });

    // Save PDF
    const fileName = `invoice-${dateStr.replace(/\//g, '-')}.pdf`;
    doc.save(fileName);
  } catch (error) {
    this.toastr.error('Unable to generate PDF');
    console.error('PDF generation error:', error);
  }
}
}