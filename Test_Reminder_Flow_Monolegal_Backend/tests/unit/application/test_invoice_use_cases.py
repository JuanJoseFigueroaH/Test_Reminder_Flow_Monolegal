import pytest
from datetime import datetime
from unittest.mock import AsyncMock

from src.application.use_cases.invoice_use_cases import InvoiceUseCases
from src.application.commands.create_invoice_command import CreateInvoiceCommand
from src.domain.entities.invoice import Invoice
from src.domain.entities.client import Client
from src.domain.value_objects.invoice_status import InvoiceStatus
from src.domain.exceptions.domain_exceptions import InvoiceNotFoundException


class TestInvoiceUseCases:
    @pytest.fixture
    def use_cases(self, mock_invoice_repository, mock_client_repository, mock_cache):
        return InvoiceUseCases(
            invoice_repository=mock_invoice_repository,
            client_repository=mock_client_repository,
            cache=mock_cache
        )

    @pytest.mark.asyncio
    async def test_get_all_invoices_from_cache(self, use_cases, mock_cache, mock_invoice):
        mock_cache.get.return_value = [mock_invoice.to_dict()]
        
        result = await use_cases.get_all_invoices()
        
        assert len(result) == 1
        assert result[0].invoice_number == mock_invoice.invoice_number
        mock_cache.get.assert_called_once()

    @pytest.mark.asyncio
    async def test_get_all_invoices_from_repository(self, use_cases, mock_invoice_repository, mock_client_repository, mock_cache, mock_invoice, mock_client):
        mock_cache.get.return_value = None
        mock_invoice_repository.get_all.return_value = [mock_invoice]
        mock_client_repository.get_by_id.return_value = mock_client
        
        result = await use_cases.get_all_invoices()
        
        assert len(result) == 1
        mock_invoice_repository.get_all.assert_called_once()
        mock_cache.set.assert_called_once()

    @pytest.mark.asyncio
    async def test_get_invoice_by_id_success(self, use_cases, mock_invoice_repository, mock_client_repository, mock_cache, mock_invoice, mock_client):
        mock_cache.get.return_value = None
        mock_invoice_repository.get_by_id.return_value = mock_invoice
        mock_client_repository.get_by_id.return_value = mock_client
        
        result = await use_cases.get_invoice_by_id(mock_invoice.id)
        
        assert result.id == mock_invoice.id
        mock_invoice_repository.get_by_id.assert_called_once_with(mock_invoice.id)

    @pytest.mark.asyncio
    async def test_get_invoice_by_id_not_found(self, use_cases, mock_invoice_repository, mock_cache):
        mock_cache.get.return_value = None
        mock_invoice_repository.get_by_id.return_value = None
        
        with pytest.raises(InvoiceNotFoundException):
            await use_cases.get_invoice_by_id("nonexistent_id")

    @pytest.mark.asyncio
    async def test_create_invoice(self, use_cases, mock_invoice_repository, mock_client_repository, mock_cache, mock_client):
        now = datetime.utcnow()
        command = CreateInvoiceCommand(
            client_id=mock_client.id,
            invoice_number="INV-NEW",
            amount=200000.00,
            due_date=now,
            status="primerrecordatorio"
        )
        
        created_invoice = Invoice(
            id="new_id",
            client_id=mock_client.id,
            invoice_number="INV-NEW",
            amount=200000.00,
            status=InvoiceStatus.PRIMER_RECORDATORIO,
            due_date=now,
            created_at=now,
            updated_at=now
        )
        
        mock_invoice_repository.create.return_value = created_invoice
        mock_client_repository.get_by_id.return_value = mock_client
        
        result = await use_cases.create_invoice(command)
        
        assert result.invoice_number == "INV-NEW"
        mock_invoice_repository.create.assert_called_once()
        mock_cache.clear_pattern.assert_called()

    @pytest.mark.asyncio
    async def test_delete_invoice_success(self, use_cases, mock_invoice_repository, mock_cache):
        mock_invoice_repository.delete.return_value = True
        
        result = await use_cases.delete_invoice("invoice_id")
        
        assert result is True
        mock_invoice_repository.delete.assert_called_once_with("invoice_id")
        mock_cache.clear_pattern.assert_called()

    @pytest.mark.asyncio
    async def test_delete_invoice_not_found(self, use_cases, mock_invoice_repository, mock_cache):
        mock_invoice_repository.delete.return_value = False
        
        result = await use_cases.delete_invoice("nonexistent_id")
        
        assert result is False
