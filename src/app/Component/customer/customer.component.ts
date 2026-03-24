import { Component, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { MaterialModule } from '../../material.module';
import { Router, RouterLink } from '@angular/router';
import { customer } from '../../_model/customer.model';
import { UserService } from '../../_service/user.service';
import { CustomerService } from '../../_service/customer.service';
import { AuthService } from '../../_service/authentication.service';
import { MatTableDataSource } from '@angular/material/table';
import { MenuPermission } from '../../_model/user.model';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-customer',
  standalone: true,
  imports: [MaterialModule, RouterLink],
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.css'],
})
export class CustomerComponent implements OnInit, OnDestroy {
  customerlist!: customer[];
  displayedColumns: string[] = [
    'name',
    'email',
    'phone',
    'company',
    'addressDetails',
    'status',
    'action',
  ];
  datasource = new MatTableDataSource<customer>();
  _response: any;
  private destroy$ = new Subject<void>();

  companyMap: { [companyId: string]: string } = {};

  _permission: MenuPermission = {
    code: '',
    name: '',
    haveview: false,
    haveadd: false,
    haveedit: false,
    havedelete: false,
    userrole: '',
    menucode: '',
  };

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private service: CustomerService,
    private userservice: UserService,
    private toastr: ToastrService,
    private router: Router
    , private authService: AuthService
  ) {
    // Constructor logic here if needed
  }
  ngOnInit(): void {
    this.loadAccess();
    this.loadCompanies();
    this.loadCustomer();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.datasource.filter = filterValue.trim().toLowerCase();
  }

  getActiveCount(): number {
    return this.customerlist?.filter(c => c.isActive)?.length || 0;
  }

  getInactiveCount(): number {
    return this.customerlist?.filter(c => !c.isActive)?.length || 0;
  }

  loadAccess(): void {
    const role = this.authService.getUserRole() as string;
    if (!role) {
      return;
    }
    this.userservice.getMenuPermission(role, 'customer')
      .pipe(takeUntil(this.destroy$))
      .subscribe((item) => {
        this._permission = item;
      });
  }

  loadCustomer(): void {
    this.service.Getall()
      .pipe(takeUntil(this.destroy$))
      .subscribe((item) => {
        this.customerlist = item;
        this.datasource = new MatTableDataSource<customer>(this.customerlist);
        this.datasource.paginator = this.paginator;
        this.datasource.sort = this.sort;
      });
  }

  loadCompanies(): void {
    this.userservice.getActiveCompanies()
      .pipe(takeUntil(this.destroy$))
      .subscribe((companies) => {
        if (!companies) return;
        companies.forEach(c => {
          if (c && c.companyId) this.companyMap[c.companyId] = c.name || '';
        });
      });
  }

  functionEdit(uniqueKeyID: string): void {
    if (this._permission.haveedit) {
      this.router.navigateByUrl('/customer/edit/' + uniqueKeyID);
    } else {
      this.toastr.warning('User not having edit access', 'warning');
    }
  }

  functionDelete(uniqueKeyID: string): void {
    if (this._permission.havedelete) {
      if (confirm('Are you sure?')) {
        this.service.Deletecustomer(uniqueKeyID)
          .pipe(takeUntil(this.destroy$))
          .subscribe((item) => {
            this._response = item;
            if (this._response.result === 'pass') {
              this.toastr.success('Deleted successfully', 'Success');
              this.loadCustomer();
            } else {
              this.toastr.error('Due to:' + this._response.message, 'Failed');
            }
          });
      }
    } else {
      this.toastr.warning('User not having delete access', 'warning');
    }
  }
}
