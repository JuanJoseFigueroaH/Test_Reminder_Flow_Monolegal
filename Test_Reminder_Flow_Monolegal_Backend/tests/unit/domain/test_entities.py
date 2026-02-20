import pytest
from datetime import datetime
from src.domain.entities.client import Client
from src.domain.entities.invoice import Invoice
from src.domain.value_objects.invoice_status import InvoiceStatus


class TestClient:
    def test_client_creation(self):
        client = Client(
            id="123",
            name="Test Client",
            email="test@email.com",
            phone="+57 300 123 4567"
        )
        assert client.id == "123"
        assert client.name == "Test Client"
        assert client.email == "test@email.com"
        assert client.phone == "+57 300 123 4567"

    def test_client_to_dict(self):
        client = Client(
            id="123",
            name="Test Client",
            email="test@email.com",
            phone="+57 300 123 4567"
        )
        result = client.to_dict()
        assert result["id"] == "123"
        assert result["name"] == "Test Client"
        assert result["email"] == "test@email.com"
        assert result["phone"] == "+57 300 123 4567"

    def test_client_from_dict(self):
        data = {
            "_id": "123",
            "name": "Test Client",
            "email": "test@email.com",
            "phone": "+57 300 123 4567"
        }
        client = Client.from_dict(data)
        assert client.id == "123"
        assert client.name == "Test Client"
        assert client.email == "test@email.com"


class TestInvoice:
    def test_invoice_creation(self):
        now = datetime.utcnow()
        invoice = Invoice(
            id="456",
            client_id="123",
            invoice_number="INV-001",
            amount=150000.00,
            status=InvoiceStatus.PRIMER_RECORDATORIO,
            due_date=now,
            created_at=now,
            updated_at=now
        )
        assert invoice.id == "456"
        assert invoice.client_id == "123"
        assert invoice.invoice_number == "INV-001"
        assert invoice.amount == 150000.00
        assert invoice.status == InvoiceStatus.PRIMER_RECORDATORIO

    def test_invoice_can_process_reminder_primer(self):
        now = datetime.utcnow()
        invoice = Invoice(
            id="456",
            client_id="123",
            invoice_number="INV-001",
            amount=150000.00,
            status=InvoiceStatus.PRIMER_RECORDATORIO,
            due_date=now,
            created_at=now,
            updated_at=now
        )
        assert invoice.can_process_reminder() is True

    def test_invoice_can_process_reminder_segundo(self):
        now = datetime.utcnow()
        invoice = Invoice(
            id="456",
            client_id="123",
            invoice_number="INV-001",
            amount=150000.00,
            status=InvoiceStatus.SEGUNDO_RECORDATORIO,
            due_date=now,
            created_at=now,
            updated_at=now
        )
        assert invoice.can_process_reminder() is True

    def test_invoice_cannot_process_reminder_desactivado(self):
        now = datetime.utcnow()
        invoice = Invoice(
            id="456",
            client_id="123",
            invoice_number="INV-001",
            amount=150000.00,
            status=InvoiceStatus.DESACTIVADO,
            due_date=now,
            created_at=now,
            updated_at=now
        )
        assert invoice.can_process_reminder() is False

    def test_invoice_get_next_status(self):
        now = datetime.utcnow()
        invoice = Invoice(
            id="456",
            client_id="123",
            invoice_number="INV-001",
            amount=150000.00,
            status=InvoiceStatus.PRIMER_RECORDATORIO,
            due_date=now,
            created_at=now,
            updated_at=now
        )
        assert invoice.get_next_status() == InvoiceStatus.SEGUNDO_RECORDATORIO

    def test_invoice_to_dict(self):
        now = datetime.utcnow()
        invoice = Invoice(
            id="456",
            client_id="123",
            invoice_number="INV-001",
            amount=150000.00,
            status=InvoiceStatus.PRIMER_RECORDATORIO,
            due_date=now,
            created_at=now,
            updated_at=now
        )
        result = invoice.to_dict()
        assert result["id"] == "456"
        assert result["client_id"] == "123"
        assert result["invoice_number"] == "INV-001"
        assert result["status"] == "primerrecordatorio"
