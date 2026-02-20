import asyncio
from apscheduler.schedulers.background import BackgroundScheduler
from apscheduler.triggers.interval import IntervalTrigger

from src.application.commands.process_reminders_command import ProcessRemindersCommand
from src.shared.logger import LoggerFactory

logger = LoggerFactory.get_logger(__name__)


class ReminderScheduler:
    _instance = None
    _scheduler: BackgroundScheduler = None
    _reminder_use_cases = None
    _loop = None

    @classmethod
    def get_instance(cls):
        if cls._instance is None:
            cls._instance = cls()
        return cls._instance

    def __init__(self):
        if ReminderScheduler._scheduler is None:
            ReminderScheduler._scheduler = BackgroundScheduler()

    def set_reminder_use_cases(self, reminder_use_cases):
        ReminderScheduler._reminder_use_cases = reminder_use_cases

    def set_event_loop(self, loop):
        ReminderScheduler._loop = loop

    def _process_reminders_job(self):
        logger.info("Scheduler: Starting automatic reminder processing...")
        try:
            if ReminderScheduler._reminder_use_cases and ReminderScheduler._loop:
                command = ProcessRemindersCommand()
                future = asyncio.run_coroutine_threadsafe(
                    ReminderScheduler._reminder_use_cases.process_reminders(command),
                    ReminderScheduler._loop
                )
                result = future.result(timeout=60)
                logger.info(f"Scheduler: Processed {result.processed_count} reminders automatically")
            else:
                logger.warning("Scheduler: Reminder use cases or event loop not configured")
        except Exception as e:
            logger.error(f"Scheduler: Error processing reminders: {str(e)}")

    def start(self):
        if not self._scheduler.running:
            self._scheduler.add_job(
                self._process_reminders_job,
                trigger=IntervalTrigger(minutes=2),
                id="process_reminders",
                name="Process Invoice Reminders",
                replace_existing=True
            )
            self._scheduler.start()
            logger.info("Scheduler: Started - Processing reminders every 2 minutes")

    def stop(self):
        if self._scheduler.running:
            self._scheduler.shutdown(wait=False)
            logger.info("Scheduler: Stopped")

    @property
    def is_running(self) -> bool:
        return self._scheduler.running if self._scheduler else False
