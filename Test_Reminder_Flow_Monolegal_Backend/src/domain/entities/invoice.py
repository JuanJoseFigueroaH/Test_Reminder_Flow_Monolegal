from dataclasses import dataclass
from datetime import datetime
from typing import Optional

from src.domain.value_objects.invoice_status import InvoiceStatus


@dataclass
class Invoice:
    id: Optional[str]
    client_id: str
    invoice_number: str
    amount: float
    status: InvoiceStatus
    due_date: datetime
    created_at: datetime
    updated_at: datetime
    client_name: Optional[str] = None
    client_email: Optional[str] = None

    def to_dict(self) -> dict:
        return {
            "id": self.id,
            "client_id": self.client_id,
            "invoice_number": self.invoice_number,
            "amount": self.amount,
            "status": self.status.value,
            "due_date": self.due_date.isoformat(),
            "created_at": self.created_at.isoformat(),
            "updated_at": self.updated_at.isoformat(),
            "client_name": self.client_name,
            "client_email": self.client_email,
        }

    @classmethod
    def from_dict(cls, data: dict) -> "Invoice":
        return cls(
            id=str(data.get("_id", data.get("id"))),
            client_id=str(data.get("client_id", "")),
            invoice_number=data.get("invoice_number", ""),
            amount=float(data.get("amount", 0)),
            status=InvoiceStatus(data.get("status", InvoiceStatus.PENDIENTE.value)),
            due_date=cls._parse_datetime(data.get("due_date")),
            created_at=cls._parse_datetime(data.get("created_at")),
            updated_at=cls._parse_datetime(data.get("updated_at")),
            client_name=data.get("client_name"),
            client_email=data.get("client_email"),
        )

    @staticmethod
    def _parse_datetime(value) -> datetime:
        if isinstance(value, datetime):
            return value
        if isinstance(value, str):
            return datetime.fromisoformat(value.replace("Z", "+00:00"))
        return datetime.utcnow()

    def can_process_reminder(self) -> bool:
        return InvoiceStatus.is_actionable(self.status)

    def get_next_status(self) -> InvoiceStatus:
        return InvoiceStatus.get_next_status(self.status)
