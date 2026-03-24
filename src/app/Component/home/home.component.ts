import { Component, OnInit, Inject } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../material.module';
import { UserService } from '../../_service/user.service';
import { AuthService } from '../../_service/authentication.service';
import { Company } from '../../_model/company.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  company?: Company;

  constructor(
    private service: UserService,
    private authService: AuthService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    const companyId = this.authService.getCompanyId();
    if (companyId) {
      this.service.getCompanyById(companyId).subscribe({
        next: (company) => {
          this.company = company;
        },
        error: (err) => {
          console.error('Failed to load company details:', err);
        }
      });
    }
  }

  openCompanyDetails(): void {
    if (this.company) {
      this.dialog.open(CompanyDetailsDialog, {
        width: '400px',
        data: this.company
      });
    }
  }
}

@Component({
  selector: 'company-details-dialog',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  template: `
    <h1 mat-dialog-title>{{data.name}}</h1>
    <div mat-dialog-content>
      <p><strong>ID:</strong> {{data.companyId}}</p>
      <p><strong>Status:</strong> {{data.status}}</p>
      <p><strong>Email:</strong> {{data.emailId}}</p>
      <p><strong>Mobile:</strong> {{data.mobileNo}}</p>
      <p><strong>Address:</strong> {{data.addressDetails}}, {{data.stateName}}, {{data.countryName}}</p>
      <p><strong>GST:</strong> {{data.gstNumber}}</p>
      <p><strong>Bank:</strong> {{data.bankName}} ({{data.ifsc}})</p>
    </div>
    <div mat-dialog-actions>
      <button mat-button mat-dialog-close>Close</button>
    </div>
  `
})
export class CompanyDetailsDialog {
  constructor(@Inject(MAT_DIALOG_DATA) public data: Company) {}
}