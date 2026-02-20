from datetime import datetime
from typing import List, Optional
from reactivex import Observable, operators as ops
from reactivex import create

from src.domain.entities.invoice import Invoice
from src.domain.ports.invoice_repository_port import InvoiceRepositoryPort
from src.domain.ports.client_repository_port import ClientRepositoryPort
from src.domain.ports.cache_port import CachePort
from src.domain.value_objects.invoice_status import InvoiceStatus
from src.domain.exceptions.domain_exceptions import InvoiceNotFoundException
from src.application.commands.create_invoice_command import CreateInvoiceCommand
from src.shared.logger import LoggerFactory

logger = LoggerFactory.get_logger(__name__)


class InvoiceUseCases:
    CACHE_KEY_ALL = "invoices:all"
    CACHE_KEY_BY_ID = "invoices:id:{}"
    CACHE_KEY_BY_CLIENT = "invoices:client:{}"

    def __init__(
        self,
        invoice_repository: InvoiceRepositoryPort,
        client_repository: ClientRepositoryPort,
        cache: CachePort
    ):
        self._invoice_repository = invoice_repository
        self._client_repository = client_repository
        self._cache = cache

    async def get_all_invoices(self) -> List[Invoice]:
        logger.info("Use case: Getting all invoices")
        cached = await self._cache.get(self.CACHE_KEY_ALL)
        if cached:
            return [Invoice.from_dict(i) for i in cached]
        
        invoices = await self._invoice_repository.get_all()
        invoices_with_client = await self._enrich_invoices_with_client_data(invoices)
        await self._cache.set(self.CACHE_KEY_ALL, [i.to_dict() for i in invoices_with_client])
        return invoices_with_client

    async def get_invoice_by_id(self, invoice_id: str) -> Invoice:
        logger.info(f"Use case: Getting invoice by id {invoice_id}")
        cache_key = self.CACHE_KEY_BY_ID.format(invoice_id)
        cached = await self._cache.get(cache_key)
        if cached:
            return Invoice.from_dict(cached)
        
        invoice = await self._invoice_repository.get_by_id(invoice_id)
        if not invoice:
            raise InvoiceNotFoundException(invoice_id)
        
        invoices_enriched = await self._enrich_invoices_with_client_data([invoice])
        invoice = invoices_enriched[0]
        await self._cache.set(cache_key, invoice.to_dict())
        return invoice

    async def get_invoices_by_client(self, client_id: str) -> List[Invoice]:
        logger.info(f"Use case: Getting invoices for client {client_id}")
        cache_key = self.CACHE_KEY_BY_CLIENT.format(client_id)
        cached = await self._cache.get(cache_key)
        if cached:
            return [Invoice.from_dict(i) for i in cached]
        
        invoices = await self._invoice_repository.get_by_client_id(client_id)
        invoices_with_client = await self._enrich_invoices_with_client_data(invoices)
        await self._cache.set(cache_key, [i.to_dict() for i in invoices_with_client])
        return invoices_with_client

    async def get_invoices_by_status(self, statuses: List[str]) -> List[Invoice]:
        logger.info(f"Use case: Getting invoices by statuses {statuses}")
        status_enums = [InvoiceStatus(s) for s in statuses]
        invoices = await self._invoice_repository.get_by_status(status_enums)
        return await self._enrich_invoices_with_client_data(invoices)

    async def create_invoice(self, command: CreateInvoiceCommand) -> Invoice:
        logger.info(f"Use case: Creating invoice {command.invoice_number}")
        invoice = Invoice(
            id=None,
            client_id=command.client_id,
            invoice_number=command.invoice_number,
            amount=command.amount,
            status=InvoiceStatus(command.status),
            due_date=command.due_date,
            created_at=datetime.utcnow(),
            updated_at=datetime.utcnow()
        )
        created = await self._invoice_repository.create(invoice)
        await self._invalidate_cache()
        invoices_enriched = await self._enrich_invoices_with_client_data([created])
        return invoices_enriched[0]

    async def update_invoice_status(self, invoice_id: str, new_status: InvoiceStatus) -> Invoice:
        logger.info(f"Use case: Updating invoice {invoice_id} status to {new_status.value}")
        invoice = await self._invoice_repository.update_status(invoice_id, new_status)
        if not invoice:
            raise InvoiceNotFoundException(invoice_id)
        await self._invalidate_cache()
        invoices_enriched = await self._enrich_invoices_with_client_data([invoice])
        return invoices_enriched[0]

    async def delete_invoice(self, invoice_id: str) -> bool:
        logger.info(f"Use case: Deleting invoice {invoice_id}")
        result = await self._invoice_repository.delete(invoice_id)
        if result:
            await self._invalidate_cache()
        return result

    async def _enrich_invoices_with_client_data(self, invoices: List[Invoice]) -> List[Invoice]:
        for invoice in invoices:
            client = await self._client_repository.get_by_id(invoice.client_id)
            if client:
                invoice.client_name = client.name
                invoice.client_email = client.email
        return invoices

    async def _invalidate_cache(self) -> None:
        await self._cache.clear_pattern("invoices:*")

    def get_all_invoices_reactive(self) -> Observable:
        def subscribe(observer, scheduler):
            async def fetch():
                try:
                    invoices = await self.get_all_invoices()
                    for invoice in invoices:
                        observer.on_next(invoice)
                    observer.on_completed()
                except Exception as e:
                    observer.on_error(e)
            
            import asyncio
            asyncio.create_task(fetch())
            return lambda: None
        
        return create(subscribe)
