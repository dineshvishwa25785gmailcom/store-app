import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { customer } from '../_model/customer.model';
import { Observable } from 'rxjs';
import { CustomerApiResult } from '../_model/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  constructor(private http: HttpClient) { }

  baseUrl = environment.apiUrl;

  Getall() {
    return this.http.get<customer[]>(this.baseUrl + 'Customer/GetAll');

  }

  Getbycode(code:string) {

    console.log('API Response:');
    console.log('API Response:', this.http.get<customer>(this.baseUrl + 'Customer/GetByUniqueKeyID?code='+code)); // 👈 This logs the full response

    return this.http.get<customer>(this.baseUrl + 'Customer/GetByUniqueKeyID?code='+code);
  }

Createcustomer(_data: customer): Observable<CustomerApiResult> {
  return this.http.post<CustomerApiResult>(this.baseUrl + 'Customer/create', _data);
}

Updatecustomer(_data: customer): Observable<CustomerApiResult> {
  return this.http.put<CustomerApiResult>(this.baseUrl + 'Customer/Update?code=' + _data.uniqueKeyID, _data);
}




  
  Deletecustomer(code: string) {
    return this.http.delete(this.baseUrl + 'Customer/Remove?code=' + code);
  }

  ExportCustomersToExcel() {
    return this.http.get(this.baseUrl + 'Customer/Exportexcel', {
      observe: 'response',
      responseType: 'blob'
    });
  }

}
