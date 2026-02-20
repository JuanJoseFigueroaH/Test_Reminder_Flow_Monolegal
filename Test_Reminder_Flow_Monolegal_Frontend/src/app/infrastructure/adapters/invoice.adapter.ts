import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { InvoicePort } from '../../domain/ports/invoice.port';
import { Invoice, InvoiceListResponse } from '../../domain/entities/invoice.entity';
import { ApiResponse } from '../../domain/entities/api-response.entity';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class InvoiceAdapter extends InvoicePort {
  private readonly baseUrl = `${environment.apiUrl}/invoices`;

  constructor(private http: HttpClient) {
    super();
  }

  getAll(): Observable<ApiResponse<InvoiceListResponse>> {
    return this.http.get<ApiResponse<InvoiceListResponse>>(this.baseUrl);
  }

  getById(id: string): Observable<ApiResponse<Invoice>> {
    return this.http.get<ApiResponse<Invoice>>(`${this.baseUrl}/${id}`);
  }

  getByClient(clientId: string): Observable<ApiResponse<InvoiceListResponse>> {
    return this.http.get<ApiResponse<InvoiceListResponse>>(`${this.baseUrl}/client/${clientId}`);
  }

  getByStatus(statuses: string): Observable<ApiResponse<InvoiceListResponse>> {
    return this.http.get<ApiResponse<InvoiceListResponse>>(`${this.baseUrl}/status/${statuses}`);
  }

  create(invoice: Partial<Invoice>): Observable<ApiResponse<Invoice>> {
    return this.http.post<ApiResponse<Invoice>>(this.baseUrl, invoice);
  }

  delete(id: string): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.baseUrl}/${id}`);
  }
}
