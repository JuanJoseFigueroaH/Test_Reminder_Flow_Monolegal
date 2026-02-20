from abc import ABC, abstractmethod

from src.domain.entities.invoice import Invoice
from src.domain.value_objects.invoice_status import InvoiceStatus


class EmailServicePort(ABC):
    @abstractmethod
    async def send_reminder_email(
        self,
        to_email: str,
        client_name: str,
        invoice: Invoice,
        new_status: InvoiceStatus
    ) -> bool:
        pass
