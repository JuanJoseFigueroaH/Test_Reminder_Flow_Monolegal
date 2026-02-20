from typing import List
from pydantic import BaseModel


class ReminderResultResponse(BaseModel):
    invoice_id: str
    invoice_number: str
    client_name: str
    client_email: str
    previous_status: str
    new_status: str
    email_sent: bool


class ProcessRemindersResponse(BaseModel):
    processed_count: int
    results: List[ReminderResultResponse]
