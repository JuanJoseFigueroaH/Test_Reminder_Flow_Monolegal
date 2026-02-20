from datetime import datetime
from typing import List, Optional
from bson import ObjectId

from src.domain.entities.invoice import Invoice
from src.domain.ports.invoice_repository_port import InvoiceRepositoryPort
from src.domain.value_objects.invoice_status import InvoiceStatus
from src.infrastructure.database.mongodb import MongoDBConnection
from src.shared.logger import LoggerFactory

logger = LoggerFactory.get_logger(__name__)


class InvoiceRepository(InvoiceRepositoryPort):
    COLLECTION_NAME = "invoices"

    async def _get_collection(self):
        db = await MongoDBConnection.get_database()
        return db[self.COLLECTION_NAME]

    async def get_all(self) -> List[Invoice]:
        logger.info("Fetching all invoices from database")
        collection = await self._get_collection()
        invoices = []
        async for doc in collection.find():
            invoices.append(Invoice.from_dict(doc))
        logger.info(f"Found {len(invoices)} invoices")
        return invoices

    async def get_by_id(self, invoice_id: str) -> Optional[Invoice]:
        logger.info(f"Fetching invoice with id: {invoice_id}")
        collection = await self._get_collection()
        doc = await collection.find_one({"_id": ObjectId(invoice_id)})
        if doc:
            logger.info(f"Invoice found: {invoice_id}")
            return Invoice.from_dict(doc)
        logger.warning(f"Invoice not found: {invoice_id}")
        return None

    async def get_by_client_id(self, client_id: str) -> List[Invoice]:
        logger.info(f"Fetching invoices for client: {client_id}")
        collection = await self._get_collection()
        invoices = []
        async for doc in collection.find({"client_id": client_id}):
            invoices.append(Invoice.from_dict(doc))
        logger.info(f"Found {len(invoices)} invoices for client {client_id}")
        return invoices

    async def get_by_status(self, statuses: List[InvoiceStatus]) -> List[Invoice]:
        status_values = [s.value for s in statuses]
        logger.info(f"Fetching invoices with statuses: {status_values}")
        collection = await self._get_collection()
        invoices = []
        async for doc in collection.find({"status": {"$in": status_values}}):
            invoices.append(Invoice.from_dict(doc))
        logger.info(f"Found {len(invoices)} invoices with specified statuses")
        return invoices

    async def update_status(self, invoice_id: str, new_status: InvoiceStatus) -> Optional[Invoice]:
        logger.info(f"Updating invoice {invoice_id} status to {new_status.value}")
        collection = await self._get_collection()
        result = await collection.find_one_and_update(
            {"_id": ObjectId(invoice_id)},
            {
                "$set": {
                    "status": new_status.value,
                    "updated_at": datetime.utcnow()
                }
            },
            return_document=True
        )
        if result:
            logger.info(f"Invoice {invoice_id} status updated successfully")
            return Invoice.from_dict(result)
        logger.warning(f"Failed to update invoice {invoice_id}")
        return None

    async def create(self, invoice: Invoice) -> Invoice:
        logger.info(f"Creating new invoice: {invoice.invoice_number}")
        collection = await self._get_collection()
        doc = {
            "client_id": invoice.client_id,
            "invoice_number": invoice.invoice_number,
            "amount": invoice.amount,
            "status": invoice.status.value,
            "due_date": invoice.due_date,
            "created_at": datetime.utcnow(),
            "updated_at": datetime.utcnow()
        }
        result = await collection.insert_one(doc)
        invoice.id = str(result.inserted_id)
        logger.info(f"Invoice created with id: {invoice.id}")
        return invoice

    async def delete(self, invoice_id: str) -> bool:
        logger.info(f"Deleting invoice: {invoice_id}")
        collection = await self._get_collection()
        result = await collection.delete_one({"_id": ObjectId(invoice_id)})
        success = result.deleted_count > 0
        if success:
            logger.info(f"Invoice {invoice_id} deleted successfully")
        else:
            logger.warning(f"Invoice {invoice_id} not found for deletion")
        return success
