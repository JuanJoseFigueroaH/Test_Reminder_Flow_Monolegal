from typing import List, Optional

from src.domain.entities.client import Client
from src.domain.ports.client_repository_port import ClientRepositoryPort
from src.domain.ports.cache_port import CachePort
from src.domain.exceptions.domain_exceptions import ClientNotFoundException
from src.application.commands.create_client_command import CreateClientCommand
from src.shared.logger import LoggerFactory

logger = LoggerFactory.get_logger(__name__)


class ClientUseCases:
    CACHE_KEY_ALL = "clients:all"
    CACHE_KEY_BY_ID = "clients:id:{}"

    def __init__(
        self,
        client_repository: ClientRepositoryPort,
        cache: CachePort
    ):
        self._client_repository = client_repository
        self._cache = cache

    async def get_all_clients(self) -> List[Client]:
        logger.info("Use case: Getting all clients")
        cached = await self._cache.get(self.CACHE_KEY_ALL)
        if cached:
            return [Client.from_dict(c) for c in cached]
        
        clients = await self._client_repository.get_all()
        await self._cache.set(self.CACHE_KEY_ALL, [c.to_dict() for c in clients])
        return clients

    async def get_client_by_id(self, client_id: str) -> Client:
        logger.info(f"Use case: Getting client by id {client_id}")
        cache_key = self.CACHE_KEY_BY_ID.format(client_id)
        cached = await self._cache.get(cache_key)
        if cached:
            return Client.from_dict(cached)
        
        client = await self._client_repository.get_by_id(client_id)
        if not client:
            raise ClientNotFoundException(client_id)
        
        await self._cache.set(cache_key, client.to_dict())
        return client

    async def create_client(self, command: CreateClientCommand) -> Client:
        logger.info(f"Use case: Creating client {command.name}")
        client = Client(
            id=None,
            name=command.name,
            email=command.email,
            phone=command.phone
        )
        created = await self._client_repository.create(client)
        await self._invalidate_cache()
        return created

    async def update_client(self, client_id: str, name: str, email: str, phone: Optional[str]) -> Client:
        logger.info(f"Use case: Updating client {client_id}")
        existing = await self._client_repository.get_by_id(client_id)
        if not existing:
            raise ClientNotFoundException(client_id)
        
        existing.name = name
        existing.email = email
        existing.phone = phone
        
        updated = await self._client_repository.update(existing)
        if not updated:
            raise ClientNotFoundException(client_id)
        
        await self._invalidate_cache()
        return updated

    async def delete_client(self, client_id: str) -> bool:
        logger.info(f"Use case: Deleting client {client_id}")
        result = await self._client_repository.delete(client_id)
        if result:
            await self._invalidate_cache()
        return result

    async def _invalidate_cache(self) -> None:
        await self._cache.clear_pattern("clients:*")
