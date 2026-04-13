// Removed duplicate loginWithPassword() definition outside the class
import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { LoginResponse, UserCredentials, VerifyLoginOtp, CreatePassword } from '../_model/user.model';
import { UserService } from './user.service';
import { LoggerService } from './logger.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = environment.apiUrl;
  private isAuthenticated = signal(false);
  public isAuthenticated$ = this.isAuthenticated.asReadonly(); // Expose as readonly signal
  private refreshTokenTimeout: any;
  private inactivityTimeout: any;
  private userEmail = new BehaviorSubject<string>('');
  public userEmail$ = this.userEmail.asObservable();
  
  // Session timeout settings (configurable)
  private readonly TOKEN_EXPIRY_MINUTES = 15; // Backend token expiry
  private readonly TOKEN_REFRESH_INTERVAL = 12 * 60 * 1000; // 12 minutes - refresh before expiry
  private readonly INACTIVITY_TIMEOUT_MINUTES = 30; // Session timeout after inactivity
  private inactivityTimer: any;
  
  // Session expiry warning subject
  private sessionExpiryWarning = new BehaviorSubject<{ willExpionIn: number } | null>(null);
  public sessionExpiryWarning$ = this.sessionExpiryWarning.asObservable();

  constructor(
    private http: HttpClient,
    private userService: UserService,
    private logger: LoggerService
  ) {
    this.checkAuthStatus();
  }

  /**
   * Get username from localStorage
   */
  getUsername(): string | null {
    return localStorage.getItem('username');
  }

  /**
   * Get user email from BehaviorSubject (for OTP flow)
   */
  getUserEmail(): string {
    return this.userEmail.getValue();
  }

  /**
   * Set user email (for OTP flow)
   */
  setUserEmail(email: string): void {
    this.userEmail.next(email);
  }

  /**
   * Get user role from localStorage
   */
  getUserRole(): string | null {
    return localStorage.getItem('userrole');
  }

  /**
   * Get company ID from localStorage
   */
  getCompanyId(): string | null {
    return localStorage.getItem('companyid');
  }

  /**
   * Get authentication status
   */
  getAuthStatus(): boolean {
    return this.isAuthenticated();
  }

  /**
   * Check if user is currently authenticated
   */
  private checkAuthStatus(): void {
    const token = localStorage.getItem('token');
    const username = localStorage.getItem('username');
    
    // Check if both token and username exist
    if (!token || !username) {
      this.isAuthenticated.set(false);
      return;
    }
    
    // Validate that token is not expired
    if (this.isTokenExpired(token)) {
      console.warn('AUTH_SERVICE: Token is expired, clearing auth');
      this.logout();
      this.isAuthenticated.set(false);
      return;
    }
    
    this.isAuthenticated.set(true);
  }

  /**
   * Check if JWT token is expired
   */
  private isTokenExpired(token: string): boolean {
    try {
      const parts = token.split('.');
      if (parts.length < 2) return true;
      
      const payload = parts[1];
      const json = decodeURIComponent(
        Array.prototype.map.call(
          atob(payload.replace(/-/g, '+').replace(/_/g, '/')),
          function(c: any) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
          }
        ).join('')
      );
      
      const obj = JSON.parse(json);
      const expirationTime = obj.exp;
      
      if (!expirationTime) {
        return true; // No expiration claim, treat as invalid
      }
      
      // exp is in seconds, convert to milliseconds
      const expirationMs = expirationTime * 1000;
      const currentTimeMs = Date.now();
      
      // Add 5 second buffer to avoid race conditions
      return currentTimeMs > (expirationMs - 5000);
    } catch (ex) {
      console.error('AUTH_SERVICE: Failed to check token expiration', ex);
      return true; // Treat as expired if we can't parse it
    }
  }

  /**
   * Store user login details and credentials
   */
  login(response: LoginResponse, username: string): void {
    console.log('AUTH_SERVICE: Login called with response:', response);
    console.log('AUTH_SERVICE: response.userRole =', response.userRole);
    
    localStorage.setItem('token', response.token);
    localStorage.setItem('refreshToken', response.refreshToken);
    localStorage.setItem('username', username);

    // Determine role: prefer explicit response.userRole, otherwise decode from JWT
    let role = response.userRole;
    if (!role && response.token) {
      role = this.extractRoleFromToken(response.token) || '';
    }
    localStorage.setItem('userrole', role || '');
    
    // Extract and store company ID from token
    if (response.token) {
      const companyId = this.extractCompanyIdFromToken(response.token);
      if (companyId) {
        localStorage.setItem('companyid', companyId);
        console.log('AUTH_SERVICE: Company ID stored:', companyId);
      }
    }
    
    console.log('AUTH_SERVICE: After setItem - userrole:', this.getUserRole());
    
    this.isAuthenticated.set(true);
    
    // Set up automatic token refresh and inactivity tracking
    this.scheduleTokenRefresh();
    this.startInactivityTimer();
  }

  /**
   * Extract role claim from JWT token payload
   */
  private extractRoleFromToken(token: string): string | null {
    try {
      const parts = token.split('.');
      if (parts.length < 2) return null;
      const payload = parts[1];
      // atob supports base64; replace url-safe chars
      const json = decodeURIComponent(Array.prototype.map.call(atob(payload.replace(/-/g, '+').replace(/_/g, '/')), function(c: any) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
      const obj = JSON.parse(json);
      // Common claim keys: role, roles, userRole
      if (obj.role) return obj.role;
      if (obj.userRole) return obj.userRole;
      if (obj.roles) {
        if (Array.isArray(obj.roles) && obj.roles.length > 0) return obj.roles[0];
        if (typeof obj.roles === 'string') return obj.roles;
      }
      // Some tokens use 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      const roleClaimKey = Object.keys(obj).find(k => k.toLowerCase().includes('role'));
      if (roleClaimKey) return obj[roleClaimKey];
      return null;
    } catch (ex) {
      console.error('Failed to parse JWT for role', ex);
      return null;
    }
  }

  /**
   * Extract company ID claim from JWT token payload
   */
  private extractCompanyIdFromToken(token: string): string | null {
    try {
      const parts = token.split('.');
      if (parts.length < 2) return null;
      const payload = parts[1];
      // atob supports base64; replace url-safe chars
      const json = decodeURIComponent(Array.prototype.map.call(atob(payload.replace(/-/g, '+').replace(/_/g, '/')), function(c: any) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
      const obj = JSON.parse(json);
      // Common claim keys: CompanyId, companyId, company
      if (obj.CompanyId) return obj.CompanyId;
      if (obj.companyId) return obj.companyId;
      if (obj.company) return obj.company;
      return null;
    } catch (ex) {
      console.error('Failed to parse JWT for company ID', ex);
      return null;
    }
  }

  /**
   * OTP-based login: Verify OTP and get token
   */
  verifyOtpLogin(email: string, otp: string): Observable<LoginResponse> {
    const data: VerifyLoginOtp = { email, otptext: otp };
    return this.userService.verifyLoginOtp(data).pipe(
      tap(response => {
        if (response && response.token) {
          // After OTP verification, fetch user details to get the actual username
          // We'll do this in the component after calling this method
          // For now, store with email as temporary username
          // The component will fix it after fetching user details
          console.log('AUTH_SERVICE: OTP verified, storing temporary auth');
          this.login(response, email);
        }
      }),
      catchError(error => {
        console.error('OTP verification failed:', error);
        return throwError(() => new Error('Failed to verify OTP'));
      })
    );
  }

  /**
   * Request OTP for login
   */
  requestLoginOtp(email: string): Observable<any> {
    return this.userService.requestLoginOtp({ email }).pipe(
      tap(() => {
        this.setUserEmail(email);
      }),
      catchError(error => {
        console.error('Failed to request OTP:', error);
        return throwError(() => new Error('Failed to request OTP'));
      })
    );
  }

  /**
   * Create password after OTP login (Optional)
   */
  createPassword(newPassword: string, confirmPassword: string): Observable<any> {
    const data: CreatePassword = { newPassword, confirmPassword };
    return this.userService.createPassword(data).pipe(
      catchError(error => {
        console.error('Failed to create password:', error);
        return throwError(() => new Error('Failed to create password'));
      })
    );
  }

  /**
   * Request OTP for forgot password
   */
  requestForgotPasswordOtp(email: string): Observable<any> {
    return this.userService.requestForgotPasswordOtp({ email }).pipe(
      tap(() => {
        this.setUserEmail(email);
      }),
      catchError(error => {
        console.error('Failed to request forgot password OTP:', error);
        return throwError(() => new Error('Failed to request OTP'));
      })
    );
  }

  /**
   * Reset password with OTP
   */
  resetPasswordWithOtp(data: { email: string; otp: string; newPassword: string; confirmPassword: string }): Observable<any> {
    this.logger.info('AUTH_SERVICE', 'Attempting password reset with OTP', {
          email: data.email,
      otp: '***REDACTED***',
      newPassword: '***REDACTED***',
      confirmPassword: '***REDACTED***'
    });
    return this.userService.resetPasswordWithOtp(data).pipe(
      tap(response => {
        console.log('✅ AUTH_SERVICE - Password reset response:', response);
        this.logger.info('AUTH_SERVICE', 'Password reset successful', {
              username: data.email,
          response
        });
      }),
      catchError(error => {
        console.error('❌ AUTH_SERVICE - Password reset error:', error);
        this.logger.error('AUTH_SERVICE', 'Password reset failed', {
              email: data.email,
          error: error.message
        }, error);
        return throwError(() => new Error('Failed to reset password'));
      })
    );
  }

  /**
   * Logout user and clear credentials
   */
  logout(): void {
    console.log('AUTH_SERVICE: Logging out user...');
    
    // Clear all storage
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('username');
    localStorage.removeItem('userrole');
    localStorage.removeItem('companyid');
    this.isAuthenticated.set(false);
    this.userEmail.next('');
    this.sessionExpiryWarning.next(null);
    
    // Clear all timers
    if (this.refreshTokenTimeout) {
      clearTimeout(this.refreshTokenTimeout);
      this.refreshTokenTimeout = null;
    }
    
    if (this.inactivityTimer) {
      clearTimeout(this.inactivityTimer);
      this.inactivityTimer = null;
    }
    
    console.log('AUTH_SERVICE: User logged out successfully');
  }

  /**
   * Refresh the authentication token
   */
  refreshToken(): Observable<LoginResponse> {
    const refreshToken = localStorage.getItem('refreshToken');
    
    if (!refreshToken) {
      this.logout();
      return throwError(() => new Error('No refresh token available'));
    }

    return this.http.post<LoginResponse>(
      `${this.baseUrl}Authorize/GenerateRefreshToken`,
      { refreshToken }
    ).pipe(
      tap(response => {
        const username = this.getUsername();
        if (username && response) {
          this.login(response, username);
        }
      }),
      catchError(error => {
        // If refresh fails, logout user
        this.logout();
        return throwError(() => new Error('Token refresh failed'));
      })
    );
  }

  /**
   * Schedule automatic token refresh before expiry
   * Tokens expire after 15 minutes, refresh after 12 minutes for safety
   */
  private scheduleTokenRefresh(): void {
    if (this.refreshTokenTimeout) {
      clearTimeout(this.refreshTokenTimeout);
    }
    
    // Refresh after 12 minutes, leaving 3-minute safety buffer before 15-minute expiry
    this.refreshTokenTimeout = setTimeout(() => {
      this.refreshToken().subscribe({
        next: (response) => {
          console.log('AUTH_SERVICE: Token refreshed successfully');
          // Reset inactivity timer on successful refresh
          this.resetInactivityTimer();
        },
        error: () => {
          console.error('AUTH_SERVICE: Token refresh failed, logging out');
          this.logout();
        }
      });
    }, this.TOKEN_REFRESH_INTERVAL);
    
    console.log('AUTH_SERVICE: Token refresh scheduled in', this.TOKEN_REFRESH_INTERVAL / 1000 / 60, 'minutes');
  }
  
  /**
   * Start inactivity timer for auto-logout after 30 minutes of no activity
   */
  private startInactivityTimer(): void {
    this.resetInactivityTimer();
  }
  
  /**
   * Reset inactivity timer - called on user activity
   */
  resetInactivityTimer(): void {
    if (this.inactivityTimer) {
      clearTimeout(this.inactivityTimer);
    }
    
    const inactivityMs = this.INACTIVITY_TIMEOUT_MINUTES * 60 * 1000;
    
    this.inactivityTimer = setTimeout(() => {
      if (this.isAuthenticated()) {
        console.warn('AUTH_SERVICE: User inactive for', this.INACTIVITY_TIMEOUT_MINUTES, 'minutes. Logging out...');
        this.logout();
      }
    }, inactivityMs);
  }
  
  /**
   * Get configured inactivity timeout in minutes
   */
  getInactivityTimeoutMinutes(): number {
    return this.INACTIVITY_TIMEOUT_MINUTES;
  }
  
  /**
   * Get configured token refresh interval in minutes
   */
  getTokenRefreshIntervalMinutes(): number {
    return this.TOKEN_REFRESH_INTERVAL / 1000 / 60;
  }

  /**
   * Check if token exists and is valid
   */
  isTokenValid(): boolean {
    const token = localStorage.getItem('token');
    return !!token;
  }

  /**
   * Get token from localStorage
   */
  getToken(): string | null {
    return localStorage.getItem('token');
  }
}

