from fastapi import APIRouter, Depends
from dependency_injector.wiring import inject, Provide

from src.infrastructure.container import Container
from src.application.use_cases.reminder_use_cases import ReminderUseCases
from src.application.commands.process_reminders_command import ProcessRemindersCommand
from src.shared.response import ApiResponse, ResponseBuilder
from src.shared.logger import LoggerFactory

logger = LoggerFactory.get_logger(__name__)

router = APIRouter(prefix="/reminders", tags=["Reminders"])


@router.post("/process", response_model=ApiResponse)
@inject
async def process_reminders(
    reminder_use_cases: ReminderUseCases = Depends(Provide[Container.reminder_use_cases])
) -> ApiResponse:
    logger.info("API: Process reminders request")
    command = ProcessRemindersCommand()
    result = await reminder_use_cases.process_reminders(command)
    return ResponseBuilder.success(
        data=result.to_dict(),
        message=f"Processed {result.processed_count} reminders successfully"
    )
