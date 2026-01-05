import { Component, QueryList, ViewChildren, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../material.module';
import jsPDF from 'jspdf';

interface InvoiceItem {
  product: string;
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
  customerName: string = '';
  @ViewChildren('productInput') productInputs!: QueryList<ElementRef>;
  
  addProduct() {
    this.items.push({
      product: '',
      quantity: null,
      rate: null,
      total: 0
    });
    setTimeout(() => {
      const inputs = this.productInputs.toArray();
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
      alert('Please fill in quantity and rate fields with valid values before printing.');
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

  downloadPDF() {
    if (!this.isValidForPrint()) {
      alert('Please fill in quantity and rate fields with valid values before downloading.');
      return;
    }

    const doc = new jsPDF();
    
    // Outer border shadow for entire content
    doc.setFillColor(200, 200, 200);
    doc.roundedRect(17, 12, 176, 0, 3, 3, 'S');
    
    // Calculate total height needed
    const tableStartY = 52;
    const rowHeight = 10;
    const totalHeight = tableStartY + rowHeight + (this.items.length * rowHeight) + rowHeight + 8;
    
    // Outer border for entire content
    doc.setDrawColor(245, 245, 245);
    doc.setLineWidth(0.5);
    doc.roundedRect(15, 10, 180, totalHeight, 3, 3, 'S');
    
    // Title (no border)
    doc.setFontSize(22);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(102, 126, 234);
    doc.text('QUICK INVOICE', 105, 22, { align: 'center' });
    
    // Separator line after title
    doc.setDrawColor(200, 200, 200);
    doc.setLineWidth(0.3);
    doc.line(20, 28, 190, 28);
    
    // Date (no border)
    const dateStr = new Date().toLocaleDateString('en-GB');
    doc.setFontSize(11);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(0, 0, 0);
    doc.text(`Date: ${dateStr}`, 20, 38);
    
    // Customer Name
    doc.text(`Customer: ${this.customerName || 'N/A'}`, 120, 38);
    
    // Separator line after date
    doc.line(20, 44, 190, 44);
    
    // Table setup
    const startY = tableStartY;
    const colX = [20, 40, 90, 120, 150];
    
    // Draw table header
    doc.setFillColor(102, 126, 234);
    doc.rect(20, startY, 170, rowHeight, 'F');
    
    // Header borders
    doc.setDrawColor(80, 80, 80);
    doc.setLineWidth(0.3);
    doc.rect(20, startY, 170, rowHeight);
    
    // Vertical lines in header
    colX.slice(1).forEach(x => {
      doc.line(x, startY, x, startY + rowHeight);
    });
    
    // Header text
    doc.setFontSize(11);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(255, 255, 255);
    doc.text('Sl.No', 30, startY + 6.5, { align: 'center' });
    doc.text('Product Name', 65, startY + 6.5, { align: 'center' });
    doc.text('Qty', 105, startY + 6.5, { align: 'center' });
    doc.text('Rate', 135, startY + 6.5, { align: 'center' });
    doc.text('Total', 170, startY + 6.5, { align: 'center' });
    
    // Reset for data rows
    doc.setTextColor(0, 0, 0);
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(10);
    
    // Table data rows
    let yPos = startY + rowHeight;
    this.items.forEach((item, index) => {
      // Alternate row colors
      if (index % 2 === 0) {
        doc.setFillColor(245, 245, 245);
        doc.rect(20, yPos, 170, rowHeight, 'F');
      }
      
      // Row border
      doc.setDrawColor(200, 200, 200);
      doc.rect(20, yPos, 170, rowHeight);
      
      // Vertical lines
      colX.slice(1).forEach(x => {
        doc.line(x, yPos, x, yPos + rowHeight);
      });
      
      // Data
      doc.text((index + 1).toString(), 30, yPos + 6.5, { align: 'center' });
      doc.text(item.product || '', 65, yPos + 6.5, { align: 'center' });
      doc.text(item.quantity?.toString() || '0', 105, yPos + 6.5, { align: 'center' });
      doc.text((item.rate?.toFixed(2) || '0.00'), 135, yPos + 6.5, { align: 'center' });
      doc.text((item.total.toFixed(2)), 170, yPos + 6.5, { align: 'center' });
      
      yPos += rowHeight;
    });
    
    // Grand Total row
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
    doc.save(`invoice-${dateStr.replace(/\//g, '-')}.pdf`);
  }
}
