import { Component, QueryList, ViewChildren, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../material.module';

interface InvoiceItem {
  quantity: number | null;
  rate: number | null;
  total: number;
}

@Component({
  selector: 'app-quick-invoice',
  standalone: true,
  imports: [CommonModule, FormsModule, MaterialModule],
  templateUrl: './quick-invoice.component.html',
  styleUrl: './quick-invoice.component.css'
})
export class QuickInvoiceComponent {
  items: InvoiceItem[] = [];
  today = new Date();
  @ViewChildren('quantityInput') quantityInputs!: QueryList<ElementRef>;
  
  addProduct() {
    this.items.push({
      quantity: null,
      rate: null,
      total: 0
    });
    setTimeout(() => {
      const inputs = this.quantityInputs.toArray();
      if (inputs.length > 0) {
        inputs[inputs.length - 1].nativeElement.focus();
      }
    });
  }

  calculateTotal(item: InvoiceItem) {
    if (item.quantity && item.rate) {
      item.total = item.quantity * item.rate;
    } else {
      item.total = 0;
    }
  }

  deleteItem(index: number) {
    this.items.splice(index, 1);
  }

  getGrandTotal(): number {
    return this.items.reduce((sum, item) => sum + item.total, 0);
  }

  isValidForPrint(): boolean {
    return this.items.length > 0 && this.items.every(item => 
      item.quantity != null && item.quantity > 0 && 
      item.rate != null && item.rate > 0
    );
  }

  printInvoice() {
    if (!this.isValidForPrint()) {
      alert('Please fill in all quantity and rate fields with valid values before printing.');
      return;
    }

    const printContent = document.getElementById('invoice-print-area');
    if (!printContent) return;

    const windowPrint = window.open('', '', 'width=800,height=600');
    if (!windowPrint) return;

    windowPrint.document.write(`
      <html>
        <head>
          <title>Quick Invoice</title>
          <style>
            body { font-family: Arial, sans-serif; padding: 20px; }
            h1 { text-align: center; color: #333; }
            table { width: 100%; border-collapse: collapse; margin-top: 20px; }
            th, td { border: 1px solid #ddd; padding: 12px; text-align: left; }
            th { background-color: #667eea; color: white; }
            .total-row { font-weight: bold; background-color: #f5f5f5; }
            .grand-total { font-size: 18px; text-align: right; margin-top: 20px; }
          </style>
        </head>
        <body>
          ${printContent.innerHTML}
        </body>
      </html>
    `);

    windowPrint.document.close();
    windowPrint.focus();
    setTimeout(() => {
      windowPrint.print();
      windowPrint.close();
    }, 250);
  }
}
