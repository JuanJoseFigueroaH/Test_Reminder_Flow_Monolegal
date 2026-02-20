import pytest
from src.domain.value_objects.invoice_status import InvoiceStatus


class TestInvoiceStatus:
    def test_get_next_status_from_primer_recordatorio(self):
        current = InvoiceStatus.PRIMER_RECORDATORIO
        next_status = InvoiceStatus.get_next_status(current)
        assert next_status == InvoiceStatus.SEGUNDO_RECORDATORIO

    def test_get_next_status_from_segundo_recordatorio(self):
        current = InvoiceStatus.SEGUNDO_RECORDATORIO
        next_status = InvoiceStatus.get_next_status(current)
        assert next_status == InvoiceStatus.DESACTIVADO

    def test_get_next_status_from_desactivado_stays_same(self):
        current = InvoiceStatus.DESACTIVADO
        next_status = InvoiceStatus.get_next_status(current)
        assert next_status == InvoiceStatus.DESACTIVADO

    def test_is_actionable_primer_recordatorio(self):
        assert InvoiceStatus.is_actionable(InvoiceStatus.PRIMER_RECORDATORIO) is True

    def test_is_actionable_segundo_recordatorio(self):
        assert InvoiceStatus.is_actionable(InvoiceStatus.SEGUNDO_RECORDATORIO) is True

    def test_is_actionable_desactivado(self):
        assert InvoiceStatus.is_actionable(InvoiceStatus.DESACTIVADO) is False

    def test_status_values(self):
        assert InvoiceStatus.PRIMER_RECORDATORIO.value == "primerrecordatorio"
        assert InvoiceStatus.SEGUNDO_RECORDATORIO.value == "segundorecordatorio"
        assert InvoiceStatus.DESACTIVADO.value == "desactivado"
