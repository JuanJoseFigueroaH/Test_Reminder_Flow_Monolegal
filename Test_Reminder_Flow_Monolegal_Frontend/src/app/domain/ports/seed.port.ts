import { Observable } from 'rxjs';
import { ApiResponse } from '../entities/api-response.entity';

export interface SeedResponse {
  clients_created: number;
  invoices_created: number;
}

export abstract class SeedPort {
  abstract seedData(): Observable<ApiResponse<SeedResponse>>;
}
