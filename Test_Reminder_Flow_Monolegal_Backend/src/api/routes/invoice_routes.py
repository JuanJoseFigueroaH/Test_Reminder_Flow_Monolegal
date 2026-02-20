from typing import List
from fastapi import APIRouter, Depends
from dependency_injector.wiring import inject, Provide

from src.infrastructure.container import Container
from src.application.use_cases.invoice_use_cases import InvoiceUseCases
from src.application.commands.create_invoice_command import CreateInvoiceCommand
from src.api.schemas.invoice_schemas import InvoiceCreateRequest, InvoiceResponse
from src.shared.response import ApiResponse, ResponseBuilder
from src.shared.logger import LoggerFactory

logger = LoggerFactory.get_logger(__name__)

router = APIRouter(prefix="/invoices", tags=["Invoices"])


@router.get("", response_model=ApiResponse)
@inject
async def get_all_invoices(
    invoice_use_cases: InvoiceUseCases = Depends(Provide[Container.invoice_use_cases])
) -> ApiResponse:
    logger.info("API: Get all invoices request")
    invoices = await invoice_use_cases.get_all_invoices()
    data = {
        "invoices": [i.to_dict() for i in invoices],
        "total": len(invoices)
    }
    return ResponseBuilder.success(data=data, message="Invoices retrieved successfully")


@router.get("/{invoice_id}", response_model=ApiResponse)
@inject
async def get_invoice_by_id(
    invoice_id: str,
    invoice_use_cases: InvoiceUseCases = Depends(Provide[Container.invoice_use_cases])
) -> ApiResponse:
    logger.info(f"API: Get invoice by id request: {invoice_id}")
    invoice = await invoice_use_cases.get_invoice_by_id(invoice_id)
    return ResponseBuilder.success(data=invoice.to_dict(), message="Invoice retrieved successfully")


@router.get("/client/{client_id}", response_model=ApiResponse)
@inject
async def get_invoices_by_client(
    client_id: str,
    invoice_use_cases: InvoiceUseCases = Depends(Provide[Container.invoice_use_cases])
) -> ApiResponse:
    logger.info(f"API: Get invoices by client request: {client_id}")
    invoices = await invoice_use_cases.get_invoices_by_client(client_id)
    data = {
        "invoices": [i.to_dict() for i in invoices],
        "total": len(invoices)
    }
    return ResponseBuilder.success(data=data, message="Invoices retrieved successfully")


@router.get("/status/{statuses}", response_model=ApiResponse)
@inject
async def get_invoices_by_status(
    statuses: str,
    invoice_use_cases: InvoiceUseCases = Depends(Provide[Container.invoice_use_cases])
) -> ApiResponse:
    logger.info(f"API: Get invoices by status request: {statuses}")
    status_list = statuses.split(",")
    invoices = await invoice_use_cases.get_invoices_by_status(status_list)
    data = {
        "invoices": [i.to_dict() for i in invoices],
        "total": len(invoices)
    }
    return ResponseBuilder.success(data=data, message="Invoices retrieved successfully")


@router.post("", response_model=ApiResponse)
@inject
async def create_invoice(
    request: InvoiceCreateRequest,
    invoice_use_cases: InvoiceUseCases = Depends(Provide[Container.invoice_use_cases])
) -> ApiResponse:
    from datetime import datetime
    logger.info(f"API: Create invoice request: {request.invoice_number}")
    command = CreateInvoiceCommand(
        client_id=request.client_id,
        invoice_number=request.invoice_number,
        amount=request.amount,
        due_date=request.due_date or datetime.utcnow(),
        status=request.status or "pendiente"
    )
    invoice = await invoice_use_cases.create_invoice(command)
    return ResponseBuilder.created(data=invoice.to_dict(), message="Invoice created successfully")


@router.delete("/{invoice_id}", response_model=ApiResponse)
@inject
async def delete_invoice(
    invoice_id: str,
    invoice_use_cases: InvoiceUseCases = Depends(Provide[Container.invoice_use_cases])
) -> ApiResponse:
    logger.info(f"API: Delete invoice request: {invoice_id}")
    result = await invoice_use_cases.delete_invoice(invoice_id)
    if result:
        return ResponseBuilder.success(message="Invoice deleted successfully")
    return ResponseBuilder.not_found(message="Invoice not found")
