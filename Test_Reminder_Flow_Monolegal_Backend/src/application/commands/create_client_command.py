from dataclasses import dataclass
from typing import Optional


@dataclass
class CreateClientCommand:
    name: str
    email: str
    phone: Optional[str] = None
