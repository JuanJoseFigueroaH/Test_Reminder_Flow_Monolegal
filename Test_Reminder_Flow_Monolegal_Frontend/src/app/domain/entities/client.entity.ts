export interface Client {
  id: string;
  name: string;
  email: string;
  phone?: string;
}

export interface ClientListResponse {
  clients: Client[];
  total: number;
}
