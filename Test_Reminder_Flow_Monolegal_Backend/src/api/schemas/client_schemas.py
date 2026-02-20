from typing import Optional, List
from pydantic import BaseModel, Field, EmailStr


class ClientCreateRequest(BaseModel):
    name: str = Field(..., min_length=1)
    email: EmailStr
    phone: Optional[str] = None


class ClientUpdateRequest(BaseModel):
    name: str = Field(..., min_length=1)
    email: EmailStr
    phone: Optional[str] = None


class ClientResponse(BaseModel):
    id: str
    name: str
    email: str
    phone: Optional[str] = None


class ClientListResponse(BaseModel):
    clients: List[ClientResponse]
    total: int
