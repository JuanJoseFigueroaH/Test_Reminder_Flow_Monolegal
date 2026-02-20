class DomainException(Exception):
    def __init__(self, message: str, code: str = "DOMAIN_ERROR"):
        self.message = message
        self.code = code
        super().__init__(self.message)


class InvoiceNotFoundException(DomainException):
    def __init__(self, invoice_id: str):
        super().__init__(
            message=f"Invoice with id {invoice_id} not found",
            code="INVOICE_NOT_FOUND"
        )


class ClientNotFoundException(DomainException):
    def __init__(self, client_id: str):
        super().__init__(
            message=f"Client with id {client_id} not found",
            code="CLIENT_NOT_FOUND"
        )


class InvalidStatusTransitionException(DomainException):
    def __init__(self, current_status: str, target_status: str):
        super().__init__(
            message=f"Invalid status transition from {current_status} to {target_status}",
            code="INVALID_STATUS_TRANSITION"
        )


class EmailSendingException(DomainException):
    def __init__(self, email: str, reason: str):
        super().__init__(
            message=f"Failed to send email to {email}: {reason}",
            code="EMAIL_SENDING_FAILED"
        )


class DatabaseConnectionException(DomainException):
    def __init__(self, reason: str):
        super().__init__(
            message=f"Database connection error: {reason}",
            code="DATABASE_CONNECTION_ERROR"
        )


class CacheException(DomainException):
    def __init__(self, reason: str):
        super().__init__(
            message=f"Cache error: {reason}",
            code="CACHE_ERROR"
        )
