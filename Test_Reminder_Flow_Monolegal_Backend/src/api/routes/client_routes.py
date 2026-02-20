from fastapi import APIRouter, Depends
from dependency_injector.wiring import inject, Provide

from src.infrastructure.container import Container
from src.application.use_cases.client_use_cases import ClientUseCases
from src.application.commands.create_client_command import CreateClientCommand
from src.api.schemas.client_schemas import ClientCreateRequest, ClientUpdateRequest
from src.shared.response import ApiResponse, ResponseBuilder
from src.shared.logger import LoggerFactory

logger = LoggerFactory.get_logger(__name__)

router = APIRouter(prefix="/clients", tags=["Clients"])


@router.get("", response_model=ApiResponse)
@inject
async def get_all_clients(
    client_use_cases: ClientUseCases = Depends(Provide[Container.client_use_cases])
) -> ApiResponse:
    logger.info("API: Get all clients request")
    clients = await client_use_cases.get_all_clients()
    data = {
        "clients": [c.to_dict() for c in clients],
        "total": len(clients)
    }
    return ResponseBuilder.success(data=data, message="Clients retrieved successfully")


@router.get("/{client_id}", response_model=ApiResponse)
@inject
async def get_client_by_id(
    client_id: str,
    client_use_cases: ClientUseCases = Depends(Provide[Container.client_use_cases])
) -> ApiResponse:
    logger.info(f"API: Get client by id request: {client_id}")
    client = await client_use_cases.get_client_by_id(client_id)
    return ResponseBuilder.success(data=client.to_dict(), message="Client retrieved successfully")


@router.post("", response_model=ApiResponse)
@inject
async def create_client(
    request: ClientCreateRequest,
    client_use_cases: ClientUseCases = Depends(Provide[Container.client_use_cases])
) -> ApiResponse:
    logger.info(f"API: Create client request: {request.name}")
    command = CreateClientCommand(
        name=request.name,
        email=request.email,
        phone=request.phone
    )
    client = await client_use_cases.create_client(command)
    return ResponseBuilder.created(data=client.to_dict(), message="Client created successfully")


@router.put("/{client_id}", response_model=ApiResponse)
@inject
async def update_client(
    client_id: str,
    request: ClientUpdateRequest,
    client_use_cases: ClientUseCases = Depends(Provide[Container.client_use_cases])
) -> ApiResponse:
    logger.info(f"API: Update client request: {client_id}")
    client = await client_use_cases.update_client(
        client_id=client_id,
        name=request.name,
        email=request.email,
        phone=request.phone
    )
    return ResponseBuilder.success(data=client.to_dict(), message="Client updated successfully")


@router.delete("/{client_id}", response_model=ApiResponse)
@inject
async def delete_client(
    client_id: str,
    client_use_cases: ClientUseCases = Depends(Provide[Container.client_use_cases])
) -> ApiResponse:
    logger.info(f"API: Delete client request: {client_id}")
    result = await client_use_cases.delete_client(client_id)
    if result:
        return ResponseBuilder.success(message="Client deleted successfully")
    return ResponseBuilder.not_found(message="Client not found")
