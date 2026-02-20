import { Observable } from 'rxjs';
import { Invoice, InvoiceListResponse } from '../entities/invoice.entity';
import { ApiResponse } from '../entities/api-response.entity';

export abstract class InvoicePort {
  abstract getAll(): Observable<ApiResponse<InvoiceListResponse>>;
  abstract getById(id: string): Observable<ApiResponse<Invoice>>;
  abstract getByClient(clientId: string): Observable<ApiResponse<InvoiceListResponse>>;
  abstract getByStatus(statuses: string): Observable<ApiResponse<InvoiceListResponse>>;
  abstract create(invoice: Partial<Invoice>): Observable<ApiResponse<Invoice>>;
  abstract delete(id: string): Observable<ApiResponse<null>>;
}
