using FluentAssertions;
using ReminderFlow.Domain.ValueObjects;

namespace ReminderFlow.Tests.Domain;

public class InvoiceStatusTests
{
    [Theory]
    [InlineData(InvoiceStatus.Pendiente, InvoiceStatus.PrimerRecordatorio)]
    [InlineData(InvoiceStatus.PrimerRecordatorio, InvoiceStatus.SegundoRecordatorio)]
    [InlineData(InvoiceStatus.SegundoRecordatorio, InvoiceStatus.Desactivado)]
    [InlineData(InvoiceStatus.Desactivado, InvoiceStatus.Desactivado)]
    public void GetNextStatus_ReturnsCorrectNextStatus(InvoiceStatus current, InvoiceStatus expected)
    {
        var result = current.GetNextStatus();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(InvoiceStatus.Pendiente, true)]
    [InlineData(InvoiceStatus.PrimerRecordatorio, true)]
    [InlineData(InvoiceStatus.SegundoRecordatorio, true)]
    [InlineData(InvoiceStatus.Desactivado, false)]
    public void IsActionable_ReturnsCorrectValue(InvoiceStatus status, bool expected)
    {
        var result = status.IsActionable();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(InvoiceStatus.Pendiente, "pendiente")]
    [InlineData(InvoiceStatus.PrimerRecordatorio, "primerrecordatorio")]
    [InlineData(InvoiceStatus.SegundoRecordatorio, "segundorecordatorio")]
    [InlineData(InvoiceStatus.Desactivado, "desactivado")]
    public void ToDbString_ReturnsCorrectString(InvoiceStatus status, string expected)
    {
        var result = status.ToDbString();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("pendiente", InvoiceStatus.Pendiente)]
    [InlineData("primerrecordatorio", InvoiceStatus.PrimerRecordatorio)]
    [InlineData("segundorecordatorio", InvoiceStatus.SegundoRecordatorio)]
    [InlineData("desactivado", InvoiceStatus.Desactivado)]
    [InlineData("PENDIENTE", InvoiceStatus.Pendiente)]
    [InlineData("unknown", InvoiceStatus.Pendiente)]
    public void FromDbString_ReturnsCorrectStatus(string input, InvoiceStatus expected)
    {
        var result = InvoiceStatusExtensions.FromDbString(input);
        result.Should().Be(expected);
    }
}
