import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './authentication.service';
import { catchError, tap, throwError } from 'rxjs';
import { LoggerService } from './logger.service';

/**
 * Token Interceptor Function
 * Automatically adds JWT token to all API requests
 * Handles token refresh on 401 errors
 * Skips token insertion for auth-related endpoints
 */
export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const logger = inject(LoggerService);

  // Skip token insertion for public endpoints
  if (shouldSkipTokenInsertion(req)) {
    logger.logApiRequest(req.method, req.url);
    return next(req).pipe(
      tap(response => {
        if (response.type === 4) { // 4 = HttpResponse
          logger.logApiResponse(req.method, req.url, response.status, response.body);
        }
      }),
      catchError(error => {
        logger.logApiError(req.method, req.url, error.status, error);
        return throwError(() => error);
      })
    );
  }

  // Get JWT token from localStorage
  const token = authService.getToken();

  // Add Authorization header if token exists
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    logger.debug('TOKEN_INTERCEPTOR', 'Authorization header added', {
      endpoint: req.url,
      hasToken: !!token
    });
  } else {
    logger.warn('TOKEN_INTERCEPTOR', 'No token found for protected endpoint', {
      endpoint: req.url
    });
  }

  // Log the request
  logger.logApiRequest(req.method, req.url);

  return next(req).pipe(
    tap(response => {
      if (response.type === 4) { // 4 = HttpResponse
        logger.logApiResponse(req.method, req.url, response.status, response.body);
      }
    }),
    catchError((error: any) => {
      logger.logApiError(req.method, req.url, error.status, error);

      if (error instanceof HttpErrorResponse) {
        switch (error.status) {
          case 401:
            // Token expired or invalid - logout user
            logger.warn('TOKEN_INTERCEPTOR', 'Unauthorized (401) - Token invalid or expired', {
              endpoint: req.url
            });
            authService.logout();
            return throwError(() => new Error('Your session has expired. Please login again.'));

          case 403:
            // Forbidden - user doesn't have permission
            logger.warn('TOKEN_INTERCEPTOR', 'Forbidden (403) - Insufficient permissions', {
              endpoint: req.url
            });
            return throwError(() => new Error('You do not have permission to access this resource.'));

          default:
            return throwError(() => error);
        }
      }
      return throwError(() => error);
    })
  );
};

/**
 * Determine if token should be skipped for this request
 * Public endpoints don't require authentication
 */
function shouldSkipTokenInsertion(request: any): boolean {
  const publicEndpoints = [
    'GenerateToken',                    // Password login
    'initialregistration',              // Email registration
    'requestloginotp',                  // Request login OTP
    'verifyloginotp',                   // Verify login OTP
    'confirmregisteration',             // Confirm registration
    'resendregistrationotp',            // Resend registration OTP
    'requestforgotpasswordotp',         // Request forgot password OTP
    'resetpasswordwithotp',             // Reset password with OTP
    'GenerateRefreshToken',             // Refresh token
    'userregistration',                 // Legacy registration endpoint
  ];

  return publicEndpoints.some(endpoint => request.url.includes(endpoint));
}
