export interface ReminderResult {
  invoice_id: string;
  invoice_number: string;
  client_name: string;
  client_email: string;
  previous_status: string;
  new_status: string;
  email_sent: boolean;
}

export interface ProcessRemindersResponse {
  processed_count: number;
  results: ReminderResult[];
}
