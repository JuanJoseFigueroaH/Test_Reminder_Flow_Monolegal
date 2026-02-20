import { Observable } from 'rxjs';
import { ProcessRemindersResponse } from '../entities/reminder.entity';
import { ApiResponse } from '../entities/api-response.entity';

export abstract class ReminderPort {
  abstract processReminders(): Observable<ApiResponse<ProcessRemindersResponse>>;
}
