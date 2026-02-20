from fastapi import Request
from fastapi.responses import JSONResponse

from src.domain.exceptions.domain_exceptions import (
    DomainException,
    InvoiceNotFoundException,
    ClientNotFoundException,
    InvalidStatusTransitionException,
    EmailSendingException,
    DatabaseConnectionException,
    CacheException
)
from src.shared.response import ResponseBuilder
from src.shared.logger import LoggerFactory

logger = LoggerFactory.get_logger(__name__)


async def domain_exception_handler(request: Request, exc: DomainException) -> JSONResponse:
    logger.error(f"Domain exception: {exc.code} - {exc.message}")
    
    status_code_map = {
        "INVOICE_NOT_FOUND": 404,
        "CLIENT_NOT_FOUND": 404,
        "INVALID_STATUS_TRANSITION": 400,
        "EMAIL_SENDING_FAILED": 500,
        "DATABASE_CONNECTION_ERROR": 503,
        "CACHE_ERROR": 503,
    }
    
    status_code = status_code_map.get(exc.code, 400)
    response = ResponseBuilder.error(message=exc.message, status_code=status_code)
    
    return JSONResponse(
        status_code=status_code,
        content=response.model_dump()
    )


async def generic_exception_handler(request: Request, exc: Exception) -> JSONResponse:
    logger.error(f"Unhandled exception: {str(exc)}")
    response = ResponseBuilder.internal_error(message="An unexpected error occurred")
    
    return JSONResponse(
        status_code=500,
        content=response.model_dump()
    )


async def validation_exception_handler(request: Request, exc: Exception) -> JSONResponse:
    logger.error(f"Validation exception: {str(exc)}")
    response = ResponseBuilder.error(message=str(exc), status_code=422)
    
    return JSONResponse(
        status_code=422,
        content=response.model_dump()
    )
