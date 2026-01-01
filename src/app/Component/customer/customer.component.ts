import { Component, ViewChild } from '@angular/core';
import { MaterialModule } from '../../material.module';
import { Router, RouterLink } from '@angular/router';
import { customer } from '../../_model/customer.model';
import { UserService } from '../../_service/user.service';
import { CustomerService } from '../../_service/customer.service';
import { MatTableDataSource } from '@angular/material/table';
import { menupermission } from '../../_model/user.model';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-customer',
  standalone: true,
  imports: [MaterialModule, RouterLink],
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.css'],
})
export class CustomerComponent {
  customerlist!: customer[];
  displayedColumns: string[] = [
    'name',
    'email',
    'phone',
    'addressDetails',
    'status',
    'action',
  ];
  datasource = new MatTableDataSource<customer>();
  _response: any;

  _permission: menupermission = {
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
  ) {
    // Constructor logic here if needed
  }
  ngOnInit(): void {
    this.Setaccess();
    this.LoadCustomer();
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

  Setaccess() {
    let role = localStorage.getItem('userrole') as string;
    this.userservice.Getmenupermission(role, 'customer').subscribe((item) => {
      this._permission = item;
      console.log(this._permission);
    });
  }

  LoadCustomer() {
    this.service.Getall().subscribe((item) => {
      this.customerlist = item;
      this.datasource = new MatTableDataSource<customer>(this.customerlist);
      this.datasource.paginator = this.paginator;
      this.datasource.sort = this.sort;
      console.log(this.customerlist);

      // console.log(this.customerlist);
      // Perform any additional operations with the customer list if needed
    });
  }

  functionedit(uniqueKeyID: string) {
    if (this._permission.haveedit) {
      console.log('API 1:'); // 👈 This logs the full response
console.log(uniqueKeyID); // 👈 This logs the full response



      this.router.navigateByUrl('/customer/edit/' + uniqueKeyID);
    } else {
      this.toastr.warning('User not having edit access', 'warning');
    }
  }

  functiondelete(uniqueKeyID: string) {
    if (this._permission.havedelete) {
      if (confirm('Are you sure?')) {
        this.service.Deletecustomer(uniqueKeyID).subscribe((item) => {
          this._response = item;
          if (this._response.result === 'pass') {
            this.toastr.success('Deleted successfully', 'Success');
            this.LoadCustomer();
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
