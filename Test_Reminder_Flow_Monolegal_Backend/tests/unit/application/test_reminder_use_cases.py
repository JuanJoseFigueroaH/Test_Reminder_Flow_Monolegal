import pytest
from datetime import datetime
from unittest.mock import AsyncMock

from src.application.use_cases.reminder_use_cases import ReminderUseCases
from src.application.commands.process_reminders_command import ProcessRemindersCommand
from src.domain.entities.invoice import Invoice
from src.domain.entities.client import Client
from src.domain.value_objects.invoice_status import InvoiceStatus


class TestReminderUseCases:
    @pytest.fixture
    def use_cases(self, mock_invoice_repository, mock_client_repository, mock_email_service, mock_cache):
        return ReminderUseCases(
            invoice_repository=mock_invoice_repository,
            client_repository=mock_client_repository,
            email_service=mock_email_service,
            cache=mock_cache
        )

    @pytest.fixture
    def invoices_to_process(self, mock_client):
        now = datetime.utcnow()
        return [
            Invoice(
                id="inv1",
                client_id=mock_client.id,
                invoice_number="INV-001",
                amount=150000.00,
                status=InvoiceStatus.PRIMER_RECORDATORIO,
                due_date=now,
                created_at=now,
                updated_at=now
            ),
            Invoice(
                id="inv2",
                client_id=mock_client.id,
                invoice_number="INV-002",
                amount=250000.00,
                status=InvoiceStatus.SEGUNDO_RECORDATORIO,
                due_date=now,
                created_at=now,
                updated_at=now
            )
        ]

    @pytest.mark.asyncio
    async def test_process_reminders_success(
        self, 
        use_cases, 
        mock_invoice_repository, 
        mock_client_repository, 
        mock_email_service, 
        mock_cache,
        mock_client,
        invoices_to_process
    ):
        mock_invoice_repository.get_by_status.return_value = invoices_to_process
        mock_client_repository.get_by_id.return_value = mock_client
        mock_email_service.send_reminder_email.return_value = True
        mock_invoice_repository.update_status.return_value = invoices_to_process[0]
        
        command = ProcessRemindersCommand()
        result = await use_cases.process_reminders(command)
        
        assert result.processed_count == 2
        assert len(result.results) == 2
        assert mock_email_service.send_reminder_email.call_count == 2
        assert mock_invoice_repository.update_status.call_count == 2
        mock_cache.clear_pattern.assert_called()

    @pytest.mark.asyncio
    async def test_process_reminders_status_transitions(
        self, 
        use_cases, 
        mock_invoice_repository, 
        mock_client_repository, 
        mock_email_service, 
        mock_cache,
        mock_client,
        invoices_to_process
    ):
        mock_invoice_repository.get_by_status.return_value = invoices_to_process
        mock_client_repository.get_by_id.return_value = mock_client
        mock_email_service.send_reminder_email.return_value = True
        mock_invoice_repository.update_status.return_value = invoices_to_process[0]
        
        command = ProcessRemindersCommand()
        result = await use_cases.process_reminders(command)
        
        assert result.results[0].previous_status == "primerrecordatorio"
        assert result.results[0].new_status == "segundorecordatorio"
        assert result.results[1].previous_status == "segundorecordatorio"
        assert result.results[1].new_status == "desactivado"

    @pytest.mark.asyncio
    async def test_process_reminders_no_invoices(
        self, 
        use_cases, 
        mock_invoice_repository, 
        mock_cache
    ):
        mock_invoice_repository.get_by_status.return_value = []
        
        command = ProcessRemindersCommand()
        result = await use_cases.process_reminders(command)
        
        assert result.processed_count == 0
        assert len(result.results) == 0

    @pytest.mark.asyncio
    async def test_process_reminders_email_failure(
        self, 
        use_cases, 
        mock_invoice_repository, 
        mock_client_repository, 
        mock_email_service, 
        mock_cache,
        mock_client,
        invoices_to_process
    ):
        mock_invoice_repository.get_by_status.return_value = [invoices_to_process[0]]
        mock_client_repository.get_by_id.return_value = mock_client
        mock_email_service.send_reminder_email.return_value = False
        mock_invoice_repository.update_status.return_value = invoices_to_process[0]
        
        command = ProcessRemindersCommand()
        result = await use_cases.process_reminders(command)
        
        assert result.processed_count == 1
        assert result.results[0].email_sent is False
        mock_invoice_repository.update_status.assert_called_once()
