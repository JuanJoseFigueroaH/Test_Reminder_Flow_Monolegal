from contextlib import asynccontextmanager
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from src.infrastructure.container import Container
from src.infrastructure.database.mongodb import MongoDBConnection
from src.infrastructure.scheduler.reminder_scheduler import ReminderScheduler
from src.api.routes import invoice_routes, client_routes, reminder_routes
from src.api.exception_handlers import domain_exception_handler, generic_exception_handler
from src.domain.exceptions.domain_exceptions import DomainException
from src.shared.logger import LoggerFactory

logger = LoggerFactory.get_logger(__name__)

container = Container()
scheduler = ReminderScheduler.get_instance()


@asynccontextmanager
async def lifespan(app: FastAPI):
    import asyncio
    logger.info("Starting application...")
    await MongoDBConnection.connect()
    
    reminder_use_cases = container.reminder_use_cases()
    scheduler.set_reminder_use_cases(reminder_use_cases)
    scheduler.set_event_loop(asyncio.get_running_loop())
    scheduler.start()
    
    yield
    
    logger.info("Shutting down application...")
    scheduler.stop()
    await MongoDBConnection.disconnect()


def create_app() -> FastAPI:
    container = Container()
    
    app = FastAPI(
        title="Reminder Flow API",
        description="API for managing invoice reminders and client notifications",
        version="1.0.0",
        lifespan=lifespan,
        docs_url="/docs",
        redoc_url="/redoc"
    )
    
    app.container = container
    
    app.add_middleware(
        CORSMiddleware,
        allow_origins=["*"],
        allow_credentials=True,
        allow_methods=["*"],
        allow_headers=["*"],
    )
    
    app.add_exception_handler(DomainException, domain_exception_handler)
    app.add_exception_handler(Exception, generic_exception_handler)
    
    app.include_router(invoice_routes.router, prefix="/api/v1")
    app.include_router(client_routes.router, prefix="/api/v1")
    app.include_router(reminder_routes.router, prefix="/api/v1")
    
    logger.info("Application created successfully")
    return app


app = create_app()


@app.get("/health", tags=["Health"])
async def health_check():
    return {"status": "healthy", "message": "API is running"}
