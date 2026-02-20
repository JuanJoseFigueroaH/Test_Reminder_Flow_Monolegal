from typing import List, Optional
from bson import ObjectId

from src.domain.entities.client import Client
from src.domain.ports.client_repository_port import ClientRepositoryPort
from src.infrastructure.database.mongodb import MongoDBConnection
from src.shared.logger import LoggerFactory

logger = LoggerFactory.get_logger(__name__)


class ClientRepository(ClientRepositoryPort):
    COLLECTION_NAME = "clients"

    async def _get_collection(self):
        db = await MongoDBConnection.get_database()
        return db[self.COLLECTION_NAME]

    async def get_all(self) -> List[Client]:
        logger.info("Fetching all clients from database")
        collection = await self._get_collection()
        clients = []
        async for doc in collection.find():
            clients.append(Client.from_dict(doc))
        logger.info(f"Found {len(clients)} clients")
        return clients

    async def get_by_id(self, client_id: str) -> Optional[Client]:
        logger.info(f"Fetching client with id: {client_id}")
        collection = await self._get_collection()
        doc = await collection.find_one({"_id": ObjectId(client_id)})
        if doc:
            logger.info(f"Client found: {client_id}")
            return Client.from_dict(doc)
        logger.warning(f"Client not found: {client_id}")
        return None

    async def create(self, client: Client) -> Client:
        logger.info(f"Creating new client: {client.name}")
        collection = await self._get_collection()
        doc = {
            "name": client.name,
            "email": client.email,
            "phone": client.phone
        }
        result = await collection.insert_one(doc)
        client.id = str(result.inserted_id)
        logger.info(f"Client created with id: {client.id}")
        return client

    async def update(self, client: Client) -> Optional[Client]:
        logger.info(f"Updating client: {client.id}")
        collection = await self._get_collection()
        result = await collection.find_one_and_update(
            {"_id": ObjectId(client.id)},
            {
                "$set": {
                    "name": client.name,
                    "email": client.email,
                    "phone": client.phone
                }
            },
            return_document=True
        )
        if result:
            logger.info(f"Client {client.id} updated successfully")
            return Client.from_dict(result)
        logger.warning(f"Failed to update client {client.id}")
        return None

    async def delete(self, client_id: str) -> bool:
        logger.info(f"Deleting client: {client_id}")
        collection = await self._get_collection()
        result = await collection.delete_one({"_id": ObjectId(client_id)})
        success = result.deleted_count > 0
        if success:
            logger.info(f"Client {client_id} deleted successfully")
        else:
            logger.warning(f"Client {client_id} not found for deletion")
        return success
