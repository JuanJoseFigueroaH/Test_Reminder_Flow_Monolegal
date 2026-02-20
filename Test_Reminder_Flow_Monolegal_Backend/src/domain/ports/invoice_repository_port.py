from abc import ABC, abstractmethod
from typing import List, Optional

from src.domain.entities.invoice import Invoice
from src.domain.value_objects.invoice_status import InvoiceStatus


class InvoiceRepositoryPort(ABC):
    @abstractmethod
    async def get_all(self) -> List[Invoice]:
        pass

    @abstractmethod
    async def get_by_id(self, invoice_id: str) -> Optional[Invoice]:
        pass

    @abstractmethod
    async def get_by_client_id(self, client_id: str) -> List[Invoice]:
        pass

    @abstractmethod
    async def get_by_status(self, statuses: List[InvoiceStatus]) -> List[Invoice]:
        pass

    @abstractmethod
    async def update_status(self, invoice_id: str, new_status: InvoiceStatus) -> Optional[Invoice]:
        pass

    @abstractmethod
    async def create(self, invoice: Invoice) -> Invoice:
        pass

    @abstractmethod
    async def delete(self, invoice_id: str) -> bool:
        pass
