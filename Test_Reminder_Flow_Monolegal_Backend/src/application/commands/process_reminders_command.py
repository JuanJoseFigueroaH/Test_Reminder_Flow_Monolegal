from dataclasses import dataclass
from typing import List

from src.domain.entities.invoice import Invoice
from src.domain.value_objects.invoice_status import InvoiceStatus


@dataclass
class ProcessRemindersCommand:
    pass


@dataclass
class ProcessReminderResult:
    invoice_id: str
    invoice_number: str
    client_name: str
    client_email: str
    previous_status: str
    new_status: str
    email_sent: bool

    def to_dict(self) -> dict:
        return {
            "invoice_id": self.invoice_id,
            "invoice_number": self.invoice_number,
            "client_name": self.client_name,
            "client_email": self.client_email,
            "previous_status": self.previous_status,
            "new_status": self.new_status,
            "email_sent": self.email_sent
        }


@dataclass
class ProcessRemindersResult:
    processed_count: int
    results: List[ProcessReminderResult]

    def to_dict(self) -> dict:
        return {
            "processed_count": self.processed_count,
            "results": [r.to_dict() for r in self.results]
        }
