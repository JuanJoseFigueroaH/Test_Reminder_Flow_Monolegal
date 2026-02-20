import pytest
from unittest.mock import AsyncMock, patch
from fastapi.testclient import TestClient
from datetime import datetime

from src.main import app
from src.domain.entities.invoice import Invoice
from src.domain.value_objects.invoice_status import InvoiceStatus


class TestInvoiceRoutes:
    @pytest.fixture
    def client(self):
        return TestClient(app)

    @pytest.fixture
    def mock_invoice_data(self):
        now = datetime.utcnow()
        return Invoice(
            id="507f1f77bcf86cd799439012",
            client_id="507f1f77bcf86cd799439011",
            invoice_number="INV-001",
            amount=150000.00,
            status=InvoiceStatus.PRIMER_RECORDATORIO,
            due_date=now,
            created_at=now,
            updated_at=now,
            client_name="Test Client",
            client_email="test@email.com"
        )

    def test_health_check(self, client):
        response = client.get("/health")
        assert response.status_code == 200
        assert response.json()["status"] == "healthy"
