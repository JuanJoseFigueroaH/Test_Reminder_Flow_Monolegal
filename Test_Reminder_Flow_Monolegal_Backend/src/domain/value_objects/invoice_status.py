from enum import Enum


class InvoiceStatus(str, Enum):
    PENDIENTE = "pendiente"
    PRIMER_RECORDATORIO = "primerrecordatorio"
    SEGUNDO_RECORDATORIO = "segundorecordatorio"
    DESACTIVADO = "desactivado"

    @classmethod
    def get_next_status(cls, current_status: "InvoiceStatus") -> "InvoiceStatus":
        transitions = {
            cls.PENDIENTE: cls.PRIMER_RECORDATORIO,
            cls.PRIMER_RECORDATORIO: cls.SEGUNDO_RECORDATORIO,
            cls.SEGUNDO_RECORDATORIO: cls.DESACTIVADO,
        }
        return transitions.get(current_status, current_status)

    @classmethod
    def is_actionable(cls, status: "InvoiceStatus") -> bool:
        return status in [cls.PENDIENTE, cls.PRIMER_RECORDATORIO, cls.SEGUNDO_RECORDATORIO]
