import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductImageService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  uploadImage(productcode: string, formFile: File): Observable<any> {
    const formData = new FormData();
    formData.append('formFile', formFile);
    
    return this.http.put(
      `${this.baseUrl}Product/UploadImage?productcode=${productcode}`,
      formData
    );
  }

  uploadMultipleImages(productcode: string, files: File[]): Observable<any> {
    const formData = new FormData();
    files.forEach(file => {
      formData.append('filecollection', file);
    });
    
    return this.http.put(
      `${this.baseUrl}Product/MultiUploadImage?productcode=${productcode}`,
      formData
    );
  }

  uploadMultipleImagesToDb(productcode: string, files: File[]): Observable<any> {
    const formData = new FormData();
    files.forEach(file => {
      formData.append('filecollection', file);
    });
    
    return this.http.put(
      `${this.baseUrl}Product/DBMultiUploadImage?productcode=${productcode}`,
      formData
    );
  }

  getImage(productcode: string): Observable<Blob> {
    return this.http.get(
      `${this.baseUrl}Product/GetImage?productcode=${productcode}`,
      { responseType: 'blob' }
    );
  }

  getMultipleImages(productcode: string): Observable<any> {
    return this.http.get(
      `${this.baseUrl}Product/GetMultiImage?productcode=${productcode}`
    );
  }

  getDbMultipleImages(productcode: string): Observable<any> {
    return this.http.get(
      `${this.baseUrl}Product/GetDBMultiImage?productcode=${productcode}`
    );
  }

  downloadImage(productcode: string): Observable<Blob> {
    return this.http.get(
      `${this.baseUrl}Product/downloadimage?productcode=${productcode}`,
      { responseType: 'blob' }
    );
  }

  downloadDbImage(productcode: string): Observable<Blob> {
    return this.http.get(
      `${this.baseUrl}Product/dbdownloadimage?productcode=${productcode}`,
      { responseType: 'blob' }
    );
  }

  removeImage(productcode: string): Observable<any> {
    return this.http.delete(
      `${this.baseUrl}Product/removeimage?productcode=${productcode}`
    );
  }

  removeMultipleImages(productcode: string): Observable<any> {
    return this.http.delete(
      `${this.baseUrl}Product/multiremoveimage?productcode=${productcode}`
    );
  }
}