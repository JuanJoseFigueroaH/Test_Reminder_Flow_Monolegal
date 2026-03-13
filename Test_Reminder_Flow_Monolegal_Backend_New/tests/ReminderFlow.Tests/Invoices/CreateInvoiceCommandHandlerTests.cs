using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ReminderFlow.Application.Invoices.Commands;
using ReminderFlow.Domain.Entities;
using ReminderFlow.Domain.Exceptions;
using ReminderFlow.Domain.Ports;
using ReminderFlow.Domain.ValueObjects;

namespace ReminderFlow.Tests.Invoices;

public class CreateInvoiceCommandHandlerTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<CreateInvoiceCommandHandler>> _loggerMock;
    private readonly CreateInvoiceCommandHandler _handler;

    public CreateInvoiceCommandHandlerTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<CreateInvoiceCommandHandler>>();
        _handler = new CreateInvoiceCommandHandler(
            _invoiceRepositoryMock.Object,
            _clientRepositoryMock.Object,
            _cacheServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCreatedInvoice()
    {
        var clientId = "507f1f77bcf86cd799439011";
        var command = new CreateInvoiceCommand(clientId, "INV-001", 150000m, null, null);
        
        var client = new Client { Id = clientId, Name = "Juan", Email = "juan@email.com" };
        var createdInvoice = new Invoice
        {
            Id = "507f1f77bcf86cd799439012",
            ClientId = clientId,
            InvoiceNumber = command.InvoiceNumber,
            Amount = command.Amount,
            Status = InvoiceStatus.Pendiente,
            CreatedAt = DateTime.UtcNow
        };

        _clientRepositoryMock
            .Setup(x => x.GetByIdAsync(clientId))
            .ReturnsAsync(client);

        _invoiceRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(createdInvoice);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.StatusCode.Should().Be(201);
        result.Data.Should().NotBeNull();
        result.Data!.InvoiceNumber.Should().Be(command.InvoiceNumber);
        result.Data.Status.Should().Be("pendiente");
    }

    [Fact]
    public async Task Handle_ClientNotFound_ThrowsNotFoundException()
    {
        var clientId = "507f1f77bcf86cd799439011";
        var command = new CreateInvoiceCommand(clientId, "INV-001", 150000m, null, null);

        _clientRepositoryMock
            .Setup(x => x.GetByIdAsync(clientId))
            .ReturnsAsync((Client?)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ValidCommand_InvalidatesCache()
    {
        var clientId = "507f1f77bcf86cd799439011";
        var command = new CreateInvoiceCommand(clientId, "INV-002", 200000m, DateTime.UtcNow.AddDays(30), "pendiente");
        
        var client = new Client { Id = clientId, Name = "María", Email = "maria@email.com" };
        var createdInvoice = new Invoice
        {
            Id = "507f1f77bcf86cd799439013",
            ClientId = clientId,
            InvoiceNumber = command.InvoiceNumber,
            Amount = command.Amount,
            Status = InvoiceStatus.Pendiente,
            CreatedAt = DateTime.UtcNow
        };

        _clientRepositoryMock
            .Setup(x => x.GetByIdAsync(clientId))
            .ReturnsAsync(client);

        _invoiceRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(createdInvoice);

        await _handler.Handle(command, CancellationToken.None);

        _cacheServiceMock.Verify(x => x.RemoveByPatternAsync("invoices:*"), Times.Once);
    }
}
