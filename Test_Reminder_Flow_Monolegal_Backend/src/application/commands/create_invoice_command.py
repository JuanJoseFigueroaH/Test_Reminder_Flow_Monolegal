from dataclasses import dataclass
from datetime import datetime
from typing import Optional


@dataclass
class CreateInvoiceCommand:
    client_id: str
    invoice_number: str
    amount: float
    due_date: datetime
    status: Optional[str] = "primerrecordatorio"
