using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ReminderFlow.Application.Invoices.Commands;
using ReminderFlow.Domain.Exceptions;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Tests.Invoices;

public class DeleteInvoiceCommandHandlerTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<DeleteInvoiceCommandHandler>> _loggerMock;
    private readonly DeleteInvoiceCommandHandler _handler;

    public DeleteInvoiceCommandHandlerTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<DeleteInvoiceCommandHandler>>();
        _handler = new DeleteInvoiceCommandHandler(
            _invoiceRepositoryMock.Object,
            _cacheServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenInvoiceExists_DeletesSuccessfully()
    {
        var invoiceId = "507f1f77bcf86cd799439011";

        _invoiceRepositoryMock
            .Setup(x => x.DeleteAsync(invoiceId))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new DeleteInvoiceCommand(invoiceId), CancellationToken.None);

        result.StatusCode.Should().Be(204);
        _cacheServiceMock.Verify(x => x.RemoveByPatternAsync("invoices:*"), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenInvoiceNotFound_ThrowsNotFoundException()
    {
        var invoiceId = "507f1f77bcf86cd799439011";

        _invoiceRepositoryMock
            .Setup(x => x.DeleteAsync(invoiceId))
            .ReturnsAsync(false);

        var act = async () => await _handler.Handle(new DeleteInvoiceCommand(invoiceId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
