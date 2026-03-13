using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ReminderFlow.Application.Reminders.Commands;
using ReminderFlow.Domain.Entities;
using ReminderFlow.Domain.Ports;
using ReminderFlow.Domain.ValueObjects;

namespace ReminderFlow.Tests.Reminders;

public class ProcessRemindersCommandHandlerTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<ProcessRemindersCommandHandler>> _loggerMock;
    private readonly ProcessRemindersCommandHandler _handler;

    public ProcessRemindersCommandHandlerTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<ProcessRemindersCommandHandler>>();
        _handler = new ProcessRemindersCommandHandler(
            _invoiceRepositoryMock.Object,
            _clientRepositoryMock.Object,
            _emailServiceMock.Object,
            _cacheServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithActionableInvoices_ProcessesReminders()
    {
        var clientId = "507f1f77bcf86cd799439011";
        var invoices = new List<Invoice>
        {
            new() { Id = "inv1", ClientId = clientId, InvoiceNumber = "INV-001", Amount = 100000m, Status = InvoiceStatus.Pendiente }
        };
        var client = new Client { Id = clientId, Name = "Juan", Email = "juan@email.com" };

        _invoiceRepositoryMock
            .Setup(x => x.GetByStatusAsync(It.IsAny<IEnumerable<InvoiceStatus>>()))
            .ReturnsAsync(invoices);

        _clientRepositoryMock
            .Setup(x => x.GetByIdAsync(clientId))
            .ReturnsAsync(client);

        _emailServiceMock
            .Setup(x => x.SendReminderEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _invoiceRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<Invoice>()))
            .ReturnsAsync((string id, Invoice inv) => inv);

        var result = await _handler.Handle(new ProcessRemindersCommand(), CancellationToken.None);

        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.ProcessedCount.Should().Be(1);
        result.Data.SuccessCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithNoActionableInvoices_ReturnsEmptyResult()
    {
        _invoiceRepositoryMock
            .Setup(x => x.GetByStatusAsync(It.IsAny<IEnumerable<InvoiceStatus>>()))
            .ReturnsAsync(new List<Invoice>());

        var result = await _handler.Handle(new ProcessRemindersCommand(), CancellationToken.None);

        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.ProcessedCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenEmailFails_CountsAsFailure()
    {
        var clientId = "507f1f77bcf86cd799439011";
        var invoices = new List<Invoice>
        {
            new() { Id = "inv1", ClientId = clientId, InvoiceNumber = "INV-001", Amount = 100000m, Status = InvoiceStatus.PrimerRecordatorio }
        };
        var client = new Client { Id = clientId, Name = "Juan", Email = "juan@email.com" };

        _invoiceRepositoryMock
            .Setup(x => x.GetByStatusAsync(It.IsAny<IEnumerable<InvoiceStatus>>()))
            .ReturnsAsync(invoices);

        _clientRepositoryMock
            .Setup(x => x.GetByIdAsync(clientId))
            .ReturnsAsync(client);

        _emailServiceMock
            .Setup(x => x.SendReminderEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new ProcessRemindersCommand(), CancellationToken.None);

        result.Data!.FailedCount.Should().Be(1);
        result.Data.SuccessCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_StatusTransitions_AreCorrect()
    {
        var clientId = "507f1f77bcf86cd799439011";
        var invoices = new List<Invoice>
        {
            new() { Id = "inv1", ClientId = clientId, InvoiceNumber = "INV-001", Amount = 100000m, Status = InvoiceStatus.PrimerRecordatorio }
        };
        var client = new Client { Id = clientId, Name = "Juan", Email = "juan@email.com" };

        _invoiceRepositoryMock
            .Setup(x => x.GetByStatusAsync(It.IsAny<IEnumerable<InvoiceStatus>>()))
            .ReturnsAsync(invoices);

        _clientRepositoryMock
            .Setup(x => x.GetByIdAsync(clientId))
            .ReturnsAsync(client);

        _emailServiceMock
            .Setup(x => x.SendReminderEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        Invoice? updatedInvoice = null;
        _invoiceRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<Invoice>()))
            .Callback<string, Invoice>((id, inv) => updatedInvoice = inv)
            .ReturnsAsync((string id, Invoice inv) => inv);

        await _handler.Handle(new ProcessRemindersCommand(), CancellationToken.None);

        updatedInvoice.Should().NotBeNull();
        updatedInvoice!.Status.Should().Be(InvoiceStatus.SegundoRecordatorio);
    }
}
