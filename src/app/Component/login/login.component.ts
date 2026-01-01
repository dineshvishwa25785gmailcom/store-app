import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MaterialModule } from '../../material.module';
import { Router, RouterLink } from '@angular/router';
import { UserService } from '../../_service/user.service';
import { ToastrService } from 'ngx-toastr';
import { loginresp, usercred } from '../../_model/user.model';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [MatButtonModule,ReactiveFormsModule, MaterialModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  _response!: loginresp;
  _loginform!: FormGroup;

  constructor(
    private builder: FormBuilder,
    private service: UserService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  ngOnInit(): void {

    localStorage.clear(); // Clear local storage on initialization
    this.service._menulist.set([]); // Reset menu list
    // Initialize the login form
    this._loginform = this.builder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });

    // Clear local storage and reset menu list on component initialization
    //localStorage.clear();
    //this.service._menulist.set([]);
  }

  proceedlogin(): void {
    if (this._loginform.valid) {
      const credentials: usercred = {
        userName: this._loginform.value.username as string,
        password: this._loginform.value.password as string
      };

      this.service.Proceedlogin(credentials).subscribe({
        next: (response) => {
          this._response = response;
// && this._response?.userRole
          if (this._response?.token) {
            // Store user details in local storage
            localStorage.setItem('token', this._response.token);
            localStorage.setItem('username', credentials.userName);
            localStorage.setItem('userrole', this._response.userRole);

            // Load menu items based on user role
            this.service.Loadmenubyrole(this._response.userRole).subscribe({
              next: (menuItems) => {
                this.service._menulist.set(menuItems);
                this.router.navigateByUrl('/'); // Navigate to the home page
              },
              error: (menuError) => {
                console.error('Menu loading error:', menuError);
                this.toastr.error('Failed to load menu items.', 'Error');
              }
            });
          } else {
            this.toastr.error('Invalid login response from the server.', 'Login Failed');
          }
        },
        error: (err) => {
          console.error('Login error:', err);
          this.toastr.error(err.error?.title || 'Failed to login', 'Error');
        }
      });
    } else {
      this.toastr.error('Please fill in all required fields.', 'Validation Error');
    }
  }
}