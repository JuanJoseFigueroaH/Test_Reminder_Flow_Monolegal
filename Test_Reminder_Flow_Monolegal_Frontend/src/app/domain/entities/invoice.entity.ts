export type InvoiceStatus = 'pendiente' | 'primerrecordatorio' | 'segundorecordatorio' | 'desactivado';

export interface Invoice {
  id: string;
  client_id: string;
  invoice_number: string;
  amount: number;
  status: InvoiceStatus;
  due_date: string;
  created_at: string;
  updated_at: string;
  client_name?: string;
  client_email?: string;
}

export interface InvoiceListResponse {
  invoices: Invoice[];
  total: number;
}
