from motor.motor_asyncio import AsyncIOMotorClient, AsyncIOMotorDatabase
from typing import Optional

from src.shared.config import get_settings
from src.shared.logger import LoggerFactory

logger = LoggerFactory.get_logger(__name__)


class MongoDBConnection:
    _client: Optional[AsyncIOMotorClient] = None
    _database: Optional[AsyncIOMotorDatabase] = None

    @classmethod
    async def connect(cls) -> AsyncIOMotorDatabase:
        if cls._database is None:
            settings = get_settings()
            logger.info(f"Connecting to MongoDB at {settings.mongodb_url}")
            cls._client = AsyncIOMotorClient(settings.mongodb_url)
            cls._database = cls._client[settings.mongodb_database]
            logger.info("MongoDB connection established successfully")
        return cls._database

    @classmethod
    async def disconnect(cls) -> None:
        if cls._client is not None:
            logger.info("Closing MongoDB connection")
            cls._client.close()
            cls._client = None
            cls._database = None
            logger.info("MongoDB connection closed")

    @classmethod
    async def get_database(cls) -> AsyncIOMotorDatabase:
        if cls._database is None:
            await cls.connect()
        return cls._database
