import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../material.module';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

interface DialogData {
  customer: any;
}

@Component({
  selector: 'app-customer-details-dialog',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './customer-details-dialog.component.html',
  styleUrls: ['./customer-details-dialog.component.css']
})
export class CustomerDetailsDialogComponent implements OnInit {
  customer: any;

  constructor(
    private dialogRef: MatDialogRef<CustomerDetailsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {
    this.customer = data.customer;
  }

  ngOnInit(): void {
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
   * Get health score color
   */
  getHealthScoreClass(score: number | undefined): string {
    if (!score) return 'health-neutral';
    if (score >= 80) return 'health-excellent';
    if (score >= 60) return 'health-good';
    if (score >= 40) return 'health-fair';
    return 'health-poor';
  }

  /**
   * Get risk level color
   */
  getRiskLevelClass(riskLevel: string | undefined): string {
    if (!riskLevel) return 'risk-default';
    const level = riskLevel.toLowerCase();
    if (level.includes('low')) return 'risk-low';
    if (level.includes('medium')) return 'risk-medium';
    if (level.includes('high')) return 'risk-high';
    return 'risk-default';
  }

  /**
   * Close dialog
   */
  closeDialog(): void {
    this.dialogRef.close();
  }
}
