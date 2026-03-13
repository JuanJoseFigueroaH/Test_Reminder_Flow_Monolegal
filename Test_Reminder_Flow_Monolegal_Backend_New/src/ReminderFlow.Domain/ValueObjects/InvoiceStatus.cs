namespace ReminderFlow.Domain.ValueObjects;

public enum InvoiceStatus
{
    Pendiente,
    PrimerRecordatorio,
    SegundoRecordatorio,
    Desactivado
}

public static class InvoiceStatusExtensions
{
    public static InvoiceStatus GetNextStatus(this InvoiceStatus status)
    {
        return status switch
        {
            InvoiceStatus.Pendiente => InvoiceStatus.PrimerRecordatorio,
            InvoiceStatus.PrimerRecordatorio => InvoiceStatus.SegundoRecordatorio,
            InvoiceStatus.SegundoRecordatorio => InvoiceStatus.Desactivado,
            InvoiceStatus.Desactivado => InvoiceStatus.Desactivado,
            _ => status
        };
    }

    public static bool IsActionable(this InvoiceStatus status)
    {
        return status is InvoiceStatus.Pendiente 
            or InvoiceStatus.PrimerRecordatorio 
            or InvoiceStatus.SegundoRecordatorio;
    }

    public static string ToDbString(this InvoiceStatus status)
    {
        return status switch
        {
            InvoiceStatus.Pendiente => "pendiente",
            InvoiceStatus.PrimerRecordatorio => "primerrecordatorio",
            InvoiceStatus.SegundoRecordatorio => "segundorecordatorio",
            InvoiceStatus.Desactivado => "desactivado",
            _ => "pendiente"
        };
    }

    public static InvoiceStatus FromDbString(string status)
    {
        return status.ToLower() switch
        {
            "pendiente" => InvoiceStatus.Pendiente,
            "primerrecordatorio" => InvoiceStatus.PrimerRecordatorio,
            "segundorecordatorio" => InvoiceStatus.SegundoRecordatorio,
            "desactivado" => InvoiceStatus.Desactivado,
            _ => InvoiceStatus.Pendiente
        };
    }
}
