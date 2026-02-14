import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MaterialModule } from '../../material.module';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../../_service/user.service';
import { LoggerService } from '../../_service/logger.service';
import { UserProfileService } from '../../_service/user-profile.service';
import { AuthService } from '../../_service/authentication.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, MaterialModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  profileForm!: FormGroup;
  isLoading = false;
  currentUsername = '';
  showPassword = false;
  userEmailDisplay = '';
  // Dialog is used to show non-editable info

  constructor(
    private builder: FormBuilder,
    private service: UserService,
    private toastr: ToastrService,
    private logger: LoggerService,
    private router: Router,
    private dialog: MatDialog,
    private authService: AuthService,
    private profileService: UserProfileService
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    this.logger.logComponentLifecycle('ProfileComponent', 'ngOnInit');
    this.loadCurrentUserData();
  }

  initializeForm(): void {
    this.profileForm = this.builder.group({
      username: [{ value: '', disabled: true }, Validators.required],
      newUsername: [''],
      name: ['', Validators.required],
      phone: ['', Validators.required],
      address: ['']
    });
  }

  loadCurrentUserData(): void {
    this.currentUsername = this.authService.getUsername() || '';
    if (this.currentUsername) {
      this.profileForm.patchValue({
        username: this.currentUsername,
        name: this.profileService.getName() || '',
        phone: this.profileService.getPhone() || '',
        address: this.profileService.getAddress() || ''
      });
      this.userEmailDisplay = this.profileService.getEmail() || this.authService.getUsername() || '';
    } else {
      this.toastr.error('Unable to load user information', 'Error');
      this.router.navigateByUrl('/');
    }
  }

  updateProfile(): void {
    if (this.profileForm.invalid) {
      this.logger.logFormValidation('ProfileForm', false, this.profileForm.errors);
      this.toastr.error('Please fill in all required fields', 'Validation Error');
      return;
    }

    this.isLoading = true;
    const profileData = {
      username: this.currentUsername,
      newUsername: this.profileForm.value.newUsername || null,
      name: this.profileForm.value.name,
      phone: this.profileForm.value.phone,
      address: this.profileForm.value.address
    };

    this.logger.info('ProfileComponent', 'Updating user profile', profileData);

    this.service.updateProfile(profileData).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.logger.logApiResponse('POST', '/api/User/updateprofile', 200, response);

        if (response.result === 'pass') {
          this.logger.logAuthEvent('Profile updated successfully', { user: this.currentUsername });
          
          // Update profile storage with new values
          this.profileService.setName(this.profileForm.value.name);
          this.profileService.setPhone(this.profileForm.value.phone);
          this.profileService.setAddress(this.profileForm.value.address);

          this.toastr.success('Profile updated successfully', 'Success');
          this.router.navigateByUrl('/');
        } else {
          this.logger.error('ProfileComponent', 'Profile update failed', response);
          this.toastr.error(response.message || 'Failed to update profile', 'Update Failed');
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.logger.logApiError('POST', '/api/User/updateprofile', error?.status || 500, error);
        
        if (error?.status === 0) {
          this.toastr.error('Network error. Please check your connection and try again.', 'Network Error');
        } else if (error?.status >= 500) {
          this.toastr.error('Server error. Please try again later.', 'Server Error');
        } else {
          this.toastr.error('Failed to update profile. Please try again.', 'Update Failed');
        }
      }
    });
  }

  cancelUpdate(): void {
    this.router.navigateByUrl('/');
  }

  openInfoDialog(templateRef: any): void {
    this.dialog.open(templateRef, { width: '420px' });
  }
}
