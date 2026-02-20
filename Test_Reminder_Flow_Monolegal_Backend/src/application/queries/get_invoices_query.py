from dataclasses import dataclass
from typing import List, Optional


@dataclass
class GetAllInvoicesQuery:
    pass


@dataclass
class GetInvoiceByIdQuery:
    invoice_id: str


@dataclass
class GetInvoicesByClientQuery:
    client_id: str


@dataclass
class GetInvoicesByStatusQuery:
    statuses: List[str]
