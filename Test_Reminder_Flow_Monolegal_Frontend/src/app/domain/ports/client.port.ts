import { Observable } from 'rxjs';
import { Client, ClientListResponse } from '../entities/client.entity';
import { ApiResponse } from '../entities/api-response.entity';

export abstract class ClientPort {
  abstract getAll(): Observable<ApiResponse<ClientListResponse>>;
  abstract getById(id: string): Observable<ApiResponse<Client>>;
  abstract create(client: Partial<Client>): Observable<ApiResponse<Client>>;
  abstract update(id: string, client: Partial<Client>): Observable<ApiResponse<Client>>;
  abstract delete(id: string): Observable<ApiResponse<null>>;
}
