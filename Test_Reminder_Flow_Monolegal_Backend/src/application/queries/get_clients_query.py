from dataclasses import dataclass


@dataclass
class GetAllClientsQuery:
    pass


@dataclass
class GetClientByIdQuery:
    client_id: str
