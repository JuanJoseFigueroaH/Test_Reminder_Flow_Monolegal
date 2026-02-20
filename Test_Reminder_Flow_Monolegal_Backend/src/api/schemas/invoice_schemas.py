from datetime import datetime
from typing import Optional, List
from pydantic import BaseModel, Field


class InvoiceCreateRequest(BaseModel):
    client_id: str = Field(..., min_length=1)
    invoice_number: str = Field(..., min_length=1)
    amount: float = Field(..., gt=0)
    due_date: Optional[datetime] = None
    status: Optional[str] = Field(default="pendiente")


class InvoiceResponse(BaseModel):
    id: str
    client_id: str
    invoice_number: str
    amount: float
    status: str
    due_date: str
    created_at: str
    updated_at: str
    client_name: Optional[str] = None
    client_email: Optional[str] = None


class InvoiceListResponse(BaseModel):
    invoices: List[InvoiceResponse]
    total: int
