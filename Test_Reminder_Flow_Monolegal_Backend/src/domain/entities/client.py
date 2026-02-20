from dataclasses import dataclass
from typing import Optional


@dataclass
class Client:
    id: Optional[str]
    name: str
    email: str
    phone: Optional[str] = None

    def to_dict(self) -> dict:
        return {
            "id": self.id,
            "name": self.name,
            "email": self.email,
            "phone": self.phone,
        }

    @classmethod
    def from_dict(cls, data: dict) -> "Client":
        return cls(
            id=str(data.get("_id", data.get("id"))),
            name=data.get("name", ""),
            email=data.get("email", ""),
            phone=data.get("phone"),
        )
