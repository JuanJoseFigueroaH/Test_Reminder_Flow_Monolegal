import pytest
import asyncio
from unittest.mock import AsyncMock, MagicMock
from datetime import datetime

from src.domain.entities.client import Client
from src.domain.entities.invoice import Invoice
from src.domain.value_objects.invoice_status import InvoiceStatus


@pytest.fixture(scope="session")
def event_loop():
    loop = asyncio.get_event_loop_policy().new_event_loop()
    yield loop
    loop.close()


@pytest.fixture
def mock_client():
    return Client(
        id="507f1f77bcf86cd799439011",
        name="Test Client",
        email="test@email.com",
        phone="+57 300 123 4567"
    )


@pytest.fixture
def mock_invoice(mock_client):
    return Invoice(
        id="507f1f77bcf86cd799439012",
        client_id=mock_client.id,
        invoice_number="INV-001",
        amount=150000.00,
        status=InvoiceStatus.PRIMER_RECORDATORIO,
        due_date=datetime.utcnow(),
        created_at=datetime.utcnow(),
        updated_at=datetime.utcnow(),
        client_name=mock_client.name,
        client_email=mock_client.email
    )


@pytest.fixture
def mock_invoice_repository():
    repository = AsyncMock()
    return repository


@pytest.fixture
def mock_client_repository():
    repository = AsyncMock()
    return repository


@pytest.fixture
def mock_cache():
    cache = AsyncMock()
    cache.get.return_value = None
    cache.set.return_value = True
    cache.delete.return_value = True
    cache.clear_pattern.return_value = True
    return cache


@pytest.fixture
def mock_email_service():
    service = AsyncMock()
    service.send_reminder_email.return_value = True
    return service
