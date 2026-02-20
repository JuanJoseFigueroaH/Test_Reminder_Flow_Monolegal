import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ClientPort } from '../../domain/ports/client.port';
import { Client, ClientListResponse } from '../../domain/entities/client.entity';
import { ApiResponse } from '../../domain/entities/api-response.entity';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ClientAdapter extends ClientPort {
  private readonly baseUrl = `${environment.apiUrl}/clients`;

  constructor(private http: HttpClient) {
    super();
  }

  getAll(): Observable<ApiResponse<ClientListResponse>> {
    return this.http.get<ApiResponse<ClientListResponse>>(this.baseUrl);
  }

  getById(id: string): Observable<ApiResponse<Client>> {
    return this.http.get<ApiResponse<Client>>(`${this.baseUrl}/${id}`);
  }

  create(client: Partial<Client>): Observable<ApiResponse<Client>> {
    return this.http.post<ApiResponse<Client>>(this.baseUrl, client);
  }

  update(id: string, client: Partial<Client>): Observable<ApiResponse<Client>> {
    return this.http.put<ApiResponse<Client>>(`${this.baseUrl}/${id}`, client);
  }

  delete(id: string): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.baseUrl}/${id}`);
  }
}
