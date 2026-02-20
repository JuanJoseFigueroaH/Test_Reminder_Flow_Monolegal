import json
from typing import Any, Optional
import redis.asyncio as redis

from src.domain.ports.cache_port import CachePort
from src.shared.config import get_settings
from src.shared.logger import LoggerFactory

logger = LoggerFactory.get_logger(__name__)


class RedisCache(CachePort):
    def __init__(self):
        self._client: Optional[redis.Redis] = None

    async def _get_client(self) -> redis.Redis:
        if self._client is None:
            settings = get_settings()
            logger.info(f"Connecting to Redis at {settings.redis_url}")
            self._client = redis.from_url(settings.redis_url, decode_responses=True)
        return self._client

    async def get(self, key: str) -> Optional[Any]:
        try:
            client = await self._get_client()
            value = await client.get(key)
            if value:
                logger.info(f"Cache hit for key: {key}")
                return json.loads(value)
            logger.info(f"Cache miss for key: {key}")
            return None
        except Exception as e:
            logger.error(f"Redis get error: {str(e)}")
            return None

    async def set(self, key: str, value: Any, ttl: int = 300) -> bool:
        try:
            client = await self._get_client()
            serialized = json.dumps(value, default=str)
            await client.setex(key, ttl, serialized)
            logger.info(f"Cache set for key: {key} with TTL: {ttl}")
            return True
        except Exception as e:
            logger.error(f"Redis set error: {str(e)}")
            return False

    async def delete(self, key: str) -> bool:
        try:
            client = await self._get_client()
            await client.delete(key)
            logger.info(f"Cache deleted for key: {key}")
            return True
        except Exception as e:
            logger.error(f"Redis delete error: {str(e)}")
            return False

    async def clear_pattern(self, pattern: str) -> bool:
        try:
            client = await self._get_client()
            keys = []
            async for key in client.scan_iter(match=pattern):
                keys.append(key)
            if keys:
                await client.delete(*keys)
                logger.info(f"Cache cleared for pattern: {pattern}, {len(keys)} keys deleted")
            return True
        except Exception as e:
            logger.error(f"Redis clear pattern error: {str(e)}")
            return False

    async def close(self) -> None:
        if self._client:
            await self._client.close()
            self._client = None
            logger.info("Redis connection closed")
