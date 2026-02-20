from typing import Any, Generic, Optional, TypeVar
from pydantic import BaseModel

T = TypeVar("T")


class ApiResponse(BaseModel, Generic[T]):
    status_code: int
    message: str
    data: Optional[T] = None

    class Config:
        arbitrary_types_allowed = True


class ResponseBuilder:
    @staticmethod
    def success(data: Any = None, message: str = "Operation successful", status_code: int = 200) -> ApiResponse:
        return ApiResponse(
            status_code=status_code,
            message=message,
            data=data
        )

    @staticmethod
    def created(data: Any = None, message: str = "Resource created successfully") -> ApiResponse:
        return ApiResponse(
            status_code=201,
            message=message,
            data=data
        )

    @staticmethod
    def error(message: str, status_code: int = 400, data: Any = None) -> ApiResponse:
        return ApiResponse(
            status_code=status_code,
            message=message,
            data=data
        )

    @staticmethod
    def not_found(message: str = "Resource not found") -> ApiResponse:
        return ApiResponse(
            status_code=404,
            message=message,
            data=None
        )

    @staticmethod
    def internal_error(message: str = "Internal server error") -> ApiResponse:
        return ApiResponse(
            status_code=500,
            message=message,
            data=None
        )
