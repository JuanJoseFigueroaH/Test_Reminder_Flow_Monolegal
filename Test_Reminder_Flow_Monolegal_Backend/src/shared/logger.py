import logging
import sys
from typing import Optional


class LoggerFactory:
    _loggers: dict = {}

    @classmethod
    def get_logger(cls, name: str, level: Optional[int] = None) -> logging.Logger:
        if name in cls._loggers:
            return cls._loggers[name]

        logger = logging.getLogger(name)
        logger.setLevel(level or logging.INFO)

        if not logger.handlers:
            handler = logging.StreamHandler(sys.stdout)
            handler.setLevel(level or logging.INFO)
            formatter = logging.Formatter(
                "%(asctime)s - %(name)s - %(levelname)s - %(message)s",
                datefmt="%Y-%m-%d %H:%M:%S"
            )
            handler.setFormatter(formatter)
            logger.addHandler(handler)

        cls._loggers[name] = logger
        return logger
