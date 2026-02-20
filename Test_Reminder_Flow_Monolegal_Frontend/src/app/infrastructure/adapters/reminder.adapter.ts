import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ReminderPort } from '../../domain/ports/reminder.port';
import { ProcessRemindersResponse } from '../../domain/entities/reminder.entity';
import { ApiResponse } from '../../domain/entities/api-response.entity';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReminderAdapter extends ReminderPort {
  private readonly baseUrl = `${environment.apiUrl}/reminders`;

  constructor(private http: HttpClient) {
    super();
  }

  processReminders(): Observable<ApiResponse<ProcessRemindersResponse>> {
    return this.http.post<ApiResponse<ProcessRemindersResponse>>(`${this.baseUrl}/process`, {});
  }
}
