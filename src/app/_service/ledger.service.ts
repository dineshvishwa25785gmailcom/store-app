import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable, throwError, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import {
  ledgerApiResponse,
  ledgerSummary,
  ageDistribution,
  customerOutstanding,
  customerLedger,
  ageingSummary,
  dispute,
  paymentEntryRequest,
  arAnalysis,
  maintenanceStatus
} from '../_model/ledger.model';

@Injectable({
  providedIn: 'root'
})
export class LedgerService {
  private baseUrl = environment.apiUrl;
  private useMockData = false; // Set to true for development without backend

  constructor(private http: HttpClient) {}

  /**
   * Handle service errors with fallback to mock data
   */
  private handleError(error: any, operation: string, MockDataGenerator?: () => ledgerApiResponse): Observable<never | ledgerApiResponse> {
    console.error(`${operation} failed:`, error);
    
    // If it's a 404 and we have mock data generator, use it
    if (error?.status === 404 && MockDataGenerator) {
      console.warn(`${operation}: Using mock data (endpoint not found)`);
      return new Observable(observer => {
        observer.next(MockDataGenerator());
        observer.complete();
      });
    }
    
    const errorMessage = error?.error?.errorMessage || error?.message || `${operation} failed`;
    return throwError(() => new Error(errorMessage));
  }

  /**
   * Get invoice numbers for a company.
   * Tries a dedicated endpoint first; falls back to extracting from outstanding invoices.
   */
  getCompanyInvoiceNumbers(companyId: string): Observable<string[]> {
    // Try a dedicated endpoint that may or may not exist
    return this.http.get<any>(`${this.baseUrl}CustomerLedger/invoices/company?companyId=${companyId}`)
      .pipe(
        map(resp => {
          // Accept either array of strings or array of objects with invoiceNumber
          const data = resp?.data || [];
          if (Array.isArray(data)) {
            return data.map((d: any) => (typeof d === 'string' ? d : d.invoiceNumber)).filter(Boolean);
          }
          return [] as string[];
        }),
        catchError(err => {
          // Fallback: attempt to extract invoice numbers from outstanding report
          console.warn('Invoices endpoint not available, falling back to outstanding invoices');
          return this.getCustomersOutstanding(companyId, 1, 1000).pipe(
            map((resp) => {
              const customers = Array.isArray(resp?.data) ? resp.data : [];
              const invoices: string[] = [];
              customers.forEach((c: any) => {
                const outs = Array.isArray(c.outstandingInvoices) ? c.outstandingInvoices : [];
                outs.forEach((inv: any) => {
                  if (inv && inv.invoiceNumber) invoices.push(inv.invoiceNumber);
                });
              });
              // unique
              return Array.from(new Set(invoices));
            }),
            catchError(() => of([]))
          );
        })
      );
  }

  /**
   * Generate mock company summary for testing
   */
  private getMockCompanySummary(): ledgerApiResponse {
    return {
      result: 'pass',
      errorMessage: null,
      data: [
        {
          customerId: 'CUST001',
          customerName: 'ABC Corporation',
          totalInvoiced: 50000,
          totalPaid: 30000,
          balance: 20000,
          daysOutstanding: 45,
          lastPaymentDate: '2026-03-15'
        },
        {
          customerId: 'CUST002',
          customerName: 'XYZ Industries',
          totalInvoiced: 75000,
          totalPaid: 50000,
          balance: 25000,
          daysOutstanding: 60,
          lastPaymentDate: '2026-03-10'
        },
        {
          customerId: 'CUST003',
          customerName: 'Tech Solutions Ltd',
          totalInvoiced: 35000,
          totalPaid: 35000,
          balance: 0,
          daysOutstanding: 0,
          lastPaymentDate: '2026-03-25'
        }
      ]
    };
  }

  /**
   * Generate mock ageing report for testing
   */
  private getMockAgeingReport(): ledgerApiResponse {
    return {
      result: 'pass',
      errorMessage: null,
      data: [
        {
          customerId: 'CUST001',
          customerName: 'ABC Corporation',
          totalOutstanding: 20000,
          bucket_0_30: 5000,
          bucket_30_60: 3000,
          bucket_60_90: 2000,
          bucket_90_plus: 10000,
          healthScore: 65
        },
        {
          customerId: 'CUST002',
          customerName: 'XYZ Industries',
          totalOutstanding: 25000,
          bucket_0_30: 8000,
          bucket_30_60: 5000,
          bucket_60_90: 7000,
          bucket_90_plus: 5000,
          healthScore: 45
        },
        {
          customerId: 'CUST003',
          customerName: 'Tech Solutions Ltd',
          totalOutstanding: 0,
          bucket_0_30: 0,
          bucket_30_60: 0,
          bucket_60_90: 0,
          bucket_90_plus: 0,
          healthScore: 100
        }
      ]
    };
  }

  /**
   * Generate mock DSO analysis for testing
   */
  private getMockDSOAnalysis(): ledgerApiResponse {
    return {
      result: 'pass',
      errorMessage: null,
      data: {
        companyId: 'COMP1',
        totalAR: 45000,
        totalInvoiced: 160000,
        dso: 102.19,
        collectionRate: 0.72,
        averageDaysOverdue: 35,
        oldestInvoice: '2026-01-15'
      }
    };
  }

  // ===== CORE READ ENDPOINTS =====

  /**
   * Get company-level AR summary
   * Maps from: GET /api/ledger/outstanding/report/company
   * Falls back to mock data if endpoint not found
   */
  getCompanySummary(companyId: string): Observable<ledgerApiResponse> {
    return this.http.get<any>(
      `${this.baseUrl}CustomerLedger/outstanding/report/company`
    ).pipe(
      map(resp => {
        // Backend returns paged data: { data: [customers], ... }
        const customers = Array.isArray(resp?.data) ? resp.data : [];
        const totalAR = customers.reduce((s: number, c: any) => s + (c.totalOutstanding || 0), 0);
        const totalInvoiced = customers.reduce((s: number, c: any) => s + ((c.totalInvoiced) || 0), 0);
        const totalPaid = customers.reduce((s: number, c: any) => s + ((c.totalPaid) || 0), 0);
        const totalDue = customers.reduce((s: number, c: any) => {
          const invoices = Array.isArray(c.outstandingInvoices) ? c.outstandingInvoices : [];
          return s + invoices.reduce((si: number, inv: any) => si + (inv.daysOverdue > 0 ? (inv.outstanding || 0) : 0), 0);
        }, 0);
        const avgDays = customers.length ? Math.round(customers.reduce((s: number, c: any) => s + ((c.daysOutstanding) || 0), 0) / customers.length) : 0;
        const largest = customers.length ? customers.reduce((max: any, c: any) => (c.totalOutstanding > (max.totalOutstanding || 0) ? c : max), customers[0]).customerName : '';

        const summary: ledgerSummary = {
          totalAR,
          totalDue,
          daysOutstanding: avgDays,
          collectionRate: totalInvoiced > 0 ? (totalPaid / totalInvoiced) : 1,
          largestCustomer: largest || '',
          currency: 'INR'
        };

        return { result: 'pass', errorMessage: null, data: summary } as ledgerApiResponse;
      }),
      catchError(err => {
        if (err.status === 404) {
          console.warn('Outstanding report endpoint not found, using mock data');
          return of(this.getMockCompanySummary());
        }
        return this.handleError(err, 'Get company summary', () => this.getMockCompanySummary());
      })
    );
  }

  /**
   * Get ageing distribution by buckets (0-30, 30-60, 60-90, 90+)
   * Maps from: GET /api/ledger/ageing/report/company
   * Falls back to mock data if endpoint not found
   */
  getAgeDistribution(companyId: string): Observable<ledgerApiResponse> {
    return this.http.get<any>(
      `${this.baseUrl}CustomerLedger/ageing/report/company`
    ).pipe(
      map(resp => {
        const first = Array.isArray(resp?.data) && resp.data.length ? resp.data[0] : null;
        const grand = first?.grandTotal || 0;
        const bucket0 = first?.current_0_30 || 0;
        const bucket30 = first?.overdue_31_60 || 0;
        const bucket60 = first?.overdue_61_90 || 0;
        const bucket90 = first?.overdue_90Plus || 0;

        const toBucket = (daysLabel: string, amount: number) => ({ days: daysLabel, amount, percentage: grand > 0 ? Math.round((amount / grand) * 100) : 0 });

        const distribution: ageDistribution = {
          bucket_0_30: toBucket('0-30', bucket0),
          bucket_30_60: toBucket('30-60', bucket30),
          bucket_60_90: toBucket('60-90', bucket60),
          bucket_90_plus: toBucket('90+', bucket90)
        };

        return { result: 'pass', errorMessage: null, data: distribution } as ledgerApiResponse;
      }),
      catchError(err => {
        if (err.status === 404) {
          console.warn('Ageing report endpoint not found, using mock data');
          return of(this.getMockAgeingReport());
        }
        return this.handleError(err, 'Get age distribution', () => this.getMockAgeingReport());
      })
    );
  }

  /**
   * Get list of customers with outstanding AR
   * Maps from: GET /api/ledger/outstanding/report/company
   */
  getCustomersOutstanding(companyId: string, page: number = 1, pageSize: number = 10): Observable<ledgerApiResponse> {
    return this.http.get<any>(
      `${this.baseUrl}CustomerLedger/outstanding/report/company?pageNumber=${page}&pageSize=${pageSize}`
    ).pipe(
      map(resp => {
        // Map API response to customerOutstanding shape
        const customers = Array.isArray(resp?.data) ? resp.data.map((c: any) => ({
          customerId: c.customerId,
          customerName: c.customerName,
          totalInvoiced: (c.outstandingInvoices || []).reduce((sum: number, inv: any) => sum + (inv.originalAmount || 0), 0),
          totalPaid: (c.outstandingInvoices || []).reduce((sum: number, inv: any) => sum + (inv.amountPaid || 0), 0),
          balance: c.totalOutstanding || 0,
          daysOutstanding: c.averageDaysToPay || 0,
          lastPaymentDate: c.lastPaymentDate === '0001-01-01T00:00:00' ? null : c.lastPaymentDate
        })) : [];
        return { result: 'pass', errorMessage: null, data: customers, currentPage: resp?.currentPage, totalPages: resp?.totalPages, totalRecords: resp?.totalRecords } as ledgerApiResponse;
      }),
      catchError(err => {
        if (err.status === 404) {
          console.warn('Outstanding report endpoint not found, using mock data');
          return of(this.getMockCompanySummary());
        }
        return this.handleError(err, 'Get outstanding customers', () => this.getMockCompanySummary());
      })
    );
  }

  /**
   * Get customer ledger details
   * GET /api/ledger/customer/{customerId}
   */
  getCustomerLedger(customerId: string): Observable<ledgerApiResponse> {
    return this.http.get<ledgerApiResponse>(
      `${this.baseUrl}CustomerLedger/customer/${customerId}`
    ).pipe(catchError(err => this.handleError(err, 'Get customer ledger')));
  }

  /**
   * Get transaction history/statement for customer
   * GET /api/ledger/customer/{customerId}/statement?pageNumber=1&pageSize=20
   */
  getCustomerHistory(customerId: string, page: number = 1, pageSize: number = 20): Observable<ledgerApiResponse> {
    return this.http.get<ledgerApiResponse>(
      `${this.baseUrl}CustomerLedger/customer/${customerId}/statement?pageNumber=${page}&pageSize=${pageSize}`
    ).pipe(catchError(err => this.handleError(err, 'Get customer history')));
  }

  /**
   * Get ageing report for customer
   * GET /api/ledger/ageing/{customerId}
   */
  getCustomerAgeingSummary(customerId: string, months: number = 6): Observable<ledgerApiResponse> {
    return this.http.get<ledgerApiResponse>(
      `${this.baseUrl}CustomerLedger/ageing/${customerId}`
    ).pipe(catchError(err => this.handleError(err, 'Get ageing summary')));
  }

  // ===== ANALYSIS & REPORTING =====

  /**
   * Get AR metrics and DSO analysis
   * GET /api/ledger/analytics/dso
   */
  getARAnalysis(companyId: string): Observable<ledgerApiResponse> {
    return this.http.get<ledgerApiResponse>(
      `${this.baseUrl}CustomerLedger/analytics/dso`
    ).pipe(
      catchError(err => {
        if (err.status === 404) {
          console.warn('DSO analysis endpoint not found, using mock data');
          return of(this.getMockDSOAnalysis());
        }
        return this.handleError(err, 'Get AR analysis', () => this.getMockDSOAnalysis());
      })
    );
  }

  /**
   * Analyze overdue AR by due date
   * NOTE: This endpoint doesn't exist yet - fallback to outstanding report
   */
  getDueDateAnalysis(companyId: string): Observable<ledgerApiResponse> {
    return this.http.get<ledgerApiResponse>(
      `${this.baseUrl}CustomerLedger/outstanding/report/company`
    ).pipe(catchError(err => this.handleError(err, 'Get due date analysis')));
  }

  /**
   * Get customer payment trend analysis
   * GET /api/ledger/customer/{customerId}/statement (use this as proxy)
   */
  getCustomerTrendAnalysis(customerId: string): Observable<ledgerApiResponse> {
    return this.http.get<ledgerApiResponse>(
      `${this.baseUrl}CustomerLedger/customer/${customerId}/statement?pageSize=100`
    ).pipe(catchError(err => this.handleError(err, 'Get trend analysis')));
  }

  /**
   * Get month-end AR report
   * NOTE: This endpoint doesn't exist - fallback to ageing report
   */
  getMonthEndReport(companyId: string): Observable<ledgerApiResponse> {
    return this.http.get<ledgerApiResponse>(
      `${this.baseUrl}CustomerLedger/ageing/report/company`
    ).pipe(catchError(err => this.handleError(err, 'Get month-end report')));
  }

  /**
   * Get executive summary for directors
   * NOTE: This endpoint doesn't exist - fallback to combined data
   * Consider creating a new endpoint in backend: GET /api/ledger/summary/executive
   */
  getDirectorSummary(companyId: string): Observable<ledgerApiResponse> {
    return this.http.get<ledgerApiResponse>(
      `${this.baseUrl}CustomerLedger/analytics/dso`
    ).pipe(catchError(err => this.handleError(err, 'Get director summary')));
  }

  // ===== MAINTENANCE OPERATIONS (MANUAL TRIGGER) =====

  /**
   * Manually trigger ageing bucket update (Admins only)
   * Returns immediately with update status
   */
  triggerAgeingUpdate(): Observable<ledgerApiResponse> {
    return this.http.post<ledgerApiResponse>(
      `${this.baseUrl}CustomerLedger/maintenance/update-ageing`,
      {}
    ).pipe(catchError(err => this.handleError(err, 'Trigger ageing update')));
  }

  /**
   * Manually trigger outstanding refresh (Admins only)
   * Returns immediately with refresh status
   */
  triggerOutstandingRefresh(): Observable<ledgerApiResponse> {
    return this.http.post<ledgerApiResponse>(
      `${this.baseUrl}CustomerLedger/maintenance/refresh-outstanding`,
      {}
    ).pipe(catchError(err => this.handleError(err, 'Trigger outstanding refresh')));
  }

  /**
   * Manually trigger ageing snapshot creation (Admins only)
   * Returns immediately with snapshot status
   */
  triggerSnapshotCreation(): Observable<ledgerApiResponse> {
    return this.http.post<ledgerApiResponse>(
      `${this.baseUrl}CustomerLedger/maintenance/create-snapshot`,
      {}
    ).pipe(catchError(err => this.handleError(err, 'Trigger snapshot creation')));
  }

  // ===== DISPUTES =====

  /**
   * Get disputes for company
   * NOTE: Disputes endpoint not implemented in controller yet
   */
  getDisputes(companyId: string): Observable<ledgerApiResponse> {
    return new Observable(observer => {
      observer.next({ result: 'success', errorMessage: null, data: [] });
      observer.complete();
    });
  }

  /**
   * Create new dispute
   * NOTE: Disputes endpoint not implemented in controller yet
   */
  createDispute(dispute: dispute): Observable<ledgerApiResponse> {
    return new Observable(observer => {
      observer.next({ result: 'fail', errorMessage: 'Disputes feature not yet implemented', data: null });
      observer.complete();
    });
  }

  /**
   * Resolve dispute
   * NOTE: Disputes endpoint not implemented in controller yet
   */
  resolveDispute(disputeId: string, resolution: string): Observable<ledgerApiResponse> {
    return new Observable(observer => {
      observer.next({ result: 'fail', errorMessage: 'Disputes feature not yet implemented', data: null });
      observer.complete();
    });
  }

  // ===== PAYMENTS =====

  /**
   * Record customer payment
   * POST /api/ledger/payment
   */
  recordPayment(payment: any): Observable<ledgerApiResponse> {
    return this.http.post<ledgerApiResponse>(
      `${this.baseUrl}CustomerLedger/payment`,
      payment
    ).pipe(catchError(err => this.handleError(err, 'Record payment')));
  }

  /**
   * Get all payments for a customer
   * GET /api/ledger/payments/customer/{customerId}
   */
  getPaymentsByCustomerId(customerId: string): Observable<ledgerApiResponse> {
    return this.http.get<ledgerApiResponse>(
      `${this.baseUrl}CustomerLedger/payments/customer/${customerId}`
    ).pipe(catchError(err => this.handleError(err, 'Get customer payments')));
  }

  /**
   * Reverse a payment transaction
   * POST /api/CustomerLedger/{paymentId}/reverse
   */
  reversePayment(paymentId: number, reason: string): Observable<ledgerApiResponse> {
    return this.http.post<ledgerApiResponse>(
      `${this.baseUrl}CustomerLedger/${paymentId}/reverse`,
      { reason }
    ).pipe(catchError(err => this.handleError(err, 'Reverse payment')));
  }

  /**
   * Delete/Inactivate a payment by payment number
   * DELETE /api/ledger/payment/{paymentNumber}
   */
  deletePayment(paymentNumber: string): Observable<ledgerApiResponse> {
    return this.http.delete<ledgerApiResponse>(
      `${this.baseUrl}ledger/payment/${paymentNumber}`
    ).pipe(catchError(err => this.handleError(err, 'Delete payment')));
  }
}
