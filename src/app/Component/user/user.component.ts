import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../material.module';
import { Users } from '../../_model/user.model';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { UserService } from '../../_service/user.service';
import { ToastrService } from 'ngx-toastr';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { UserupdateComponent } from '../userupdate/userupdate.component';
import { LoggerService } from '../../_service/logger.service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatDialogModule
  ],
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css'],
})
export class UserComponent implements OnInit, AfterViewInit {
  userlist: Users[] = [];
  displayedColumns: string[] = [
    'username',
    'name',
    'email',
    'phone',
    'role',
    'status',
    'action',
  ];
  datasource!: MatTableDataSource<Users>;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private service: UserService,
    private toastr: ToastrService,
    private dialog: MatDialog,
    private logger: LoggerService
  ) {}

  ngOnInit(): void {
    console.log('UserComponent initialized');
    this.logger.logComponentLifecycle('UserComponent', 'ngOnInit');
    this.Loadusers();
  }

  ngAfterViewInit(): void {
    console.log('UserComponent ngAfterViewInit fired');
    if (this.datasource) {
      this.datasource.paginator = this.paginator;
      this.datasource.sort = this.sort;
    }
  }

Loadusers() {
  console.log('Calling API: /User/GetAll');
  this.service.getAllUsers().subscribe({
    next: (users: Users[]) => {
      this.userlist = users || [];
      this.datasource = new MatTableDataSource<Users>(this.userlist);
      this.datasource.paginator = this.paginator;
      this.datasource.sort = this.sort;
      this.logger.info('UserComponent', `Loaded ${this.userlist.length} users`);
      this.toastr.success(`Loaded ${this.userlist.length} users`, 'Success');
    },
    error: (error) => {
      console.error('Error loading users:', error);
      this.datasource = new MatTableDataSource<Users>([]);
      this.toastr.error('Failed to load users', 'Error');
    }
  });
}

  updaterole(code: string) {
    console.log('Updating role for:', code);
    this.Openpopup(code, 'role');
  }

  updatestatus(code: string) {
    console.log('Updating status for:', code);
    this.Openpopup(code, 'status');
  }

  Openpopup(username: string, type: string) {
    console.log(`Opening popup for ${username}, type: ${type}`);
    this.dialog
      .open(UserupdateComponent, {
        width: '30%',
        enterAnimationDuration: '1000ms',
        exitAnimationDuration: '1000ms',
        data: { username, type },
      })
      .afterClosed()
      .subscribe((item) => {
        console.log('Popup closed, reloading users');
        this.Loadusers();
      });
  }
}