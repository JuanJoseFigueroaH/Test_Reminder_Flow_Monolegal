from dependency_injector import containers, providers

from src.infrastructure.repositories.invoice_repository import InvoiceRepository
from src.infrastructure.repositories.client_repository import ClientRepository
from src.infrastructure.cache.redis_cache import RedisCache
from src.infrastructure.email.email_service import EmailService
from src.application.use_cases.invoice_use_cases import InvoiceUseCases
from src.application.use_cases.client_use_cases import ClientUseCases
from src.application.use_cases.reminder_use_cases import ReminderUseCases


class Container(containers.DeclarativeContainer):
    wiring_config = containers.WiringConfiguration(
        modules=[
            "src.api.routes.invoice_routes",
            "src.api.routes.client_routes",
            "src.api.routes.reminder_routes"
        ]
    )

    invoice_repository = providers.Singleton(InvoiceRepository)
    client_repository = providers.Singleton(ClientRepository)
    cache = providers.Singleton(RedisCache)
    email_service = providers.Singleton(EmailService)

    invoice_use_cases = providers.Factory(
        InvoiceUseCases,
        invoice_repository=invoice_repository,
        client_repository=client_repository,
        cache=cache
    )

    client_use_cases = providers.Factory(
        ClientUseCases,
        client_repository=client_repository,
        cache=cache
    )

    reminder_use_cases = providers.Factory(
        ReminderUseCases,
        invoice_repository=invoice_repository,
        client_repository=client_repository,
        email_service=email_service,
        cache=cache
    )
