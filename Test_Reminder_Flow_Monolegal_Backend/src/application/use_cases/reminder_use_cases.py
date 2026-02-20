from typing import List
from reactivex import Observable, create, operators as ops

from src.domain.entities.invoice import Invoice
from src.domain.ports.invoice_repository_port import InvoiceRepositoryPort
from src.domain.ports.client_repository_port import ClientRepositoryPort
from src.domain.ports.email_service_port import EmailServicePort
from src.domain.ports.cache_port import CachePort
from src.domain.value_objects.invoice_status import InvoiceStatus
from src.application.commands.process_reminders_command import (
    ProcessRemindersCommand,
    ProcessReminderResult,
    ProcessRemindersResult
)
from src.shared.logger import LoggerFactory

logger = LoggerFactory.get_logger(__name__)


class ReminderUseCases:
    def __init__(
        self,
        invoice_repository: InvoiceRepositoryPort,
        client_repository: ClientRepositoryPort,
        email_service: EmailServicePort,
        cache: CachePort
    ):
        self._invoice_repository = invoice_repository
        self._client_repository = client_repository
        self._email_service = email_service
        self._cache = cache

    async def process_reminders(self, command: ProcessRemindersCommand) -> ProcessRemindersResult:
        logger.info("Use case: Processing reminders for all actionable invoices")
        
        actionable_statuses = [
            InvoiceStatus.PENDIENTE,
            InvoiceStatus.PRIMER_RECORDATORIO,
            InvoiceStatus.SEGUNDO_RECORDATORIO
        ]
        
        invoices = await self._invoice_repository.get_by_status(actionable_statuses)
        logger.info(f"Found {len(invoices)} invoices to process")
        
        results: List[ProcessReminderResult] = []
        
        for invoice in invoices:
            result = await self._process_single_invoice(invoice)
            results.append(result)
        
        await self._cache.clear_pattern("invoices:*")
        
        return ProcessRemindersResult(
            processed_count=len(results),
            results=results
        )

    async def _process_single_invoice(self, invoice: Invoice) -> ProcessReminderResult:
        logger.info(f"Processing invoice {invoice.invoice_number}")
        
        client = await self._client_repository.get_by_id(invoice.client_id)
        client_name = client.name if client else "Cliente"
        client_email = client.email if client else ""
        
        previous_status = invoice.status
        new_status = invoice.get_next_status()
        
        email_sent = False
        if client_email:
            email_sent = await self._email_service.send_reminder_email(
                to_email=client_email,
                client_name=client_name,
                invoice=invoice,
                new_status=new_status
            )
        
        await self._invoice_repository.update_status(invoice.id, new_status)
        
        logger.info(f"Invoice {invoice.invoice_number} processed: {previous_status.value} -> {new_status.value}")
        
        return ProcessReminderResult(
            invoice_id=invoice.id,
            invoice_number=invoice.invoice_number,
            client_name=client_name,
            client_email=client_email,
            previous_status=previous_status.value,
            new_status=new_status.value,
            email_sent=email_sent
        )

    def process_reminders_reactive(self) -> Observable:
        def subscribe(observer, scheduler):
            async def process():
                try:
                    actionable_statuses = [
                        InvoiceStatus.PENDIENTE,
                        InvoiceStatus.PRIMER_RECORDATORIO,
                        InvoiceStatus.SEGUNDO_RECORDATORIO
                    ]
                    
                    invoices = await self._invoice_repository.get_by_status(actionable_statuses)
                    
                    for invoice in invoices:
                        result = await self._process_single_invoice(invoice)
                        observer.on_next(result)
                    
                    await self._cache.clear_pattern("invoices:*")
                    observer.on_completed()
                except Exception as e:
                    observer.on_error(e)
            
            import asyncio
            asyncio.create_task(process())
            return lambda: None
        
        return create(subscribe)
