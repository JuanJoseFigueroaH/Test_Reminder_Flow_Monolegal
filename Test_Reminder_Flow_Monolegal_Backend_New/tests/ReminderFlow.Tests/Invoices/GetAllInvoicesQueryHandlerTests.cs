using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Application.Invoices.Queries;
using ReminderFlow.Domain.Entities;
using ReminderFlow.Domain.Ports;
using ReminderFlow.Domain.ValueObjects;

namespace ReminderFlow.Tests.Invoices;

public class GetAllInvoicesQueryHandlerTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<GetAllInvoicesQueryHandler>> _loggerMock;
    private readonly GetAllInvoicesQueryHandler _handler;

    public GetAllInvoicesQueryHandlerTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<GetAllInvoicesQueryHandler>>();
        _handler = new GetAllInvoicesQueryHandler(
            _invoiceRepositoryMock.Object,
            _cacheServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenCacheHit_ReturnsCachedInvoices()
    {
        var cachedInvoices = new List<InvoiceDto>
        {
            new("1", "client1", "INV-001", 100000m, null, "pendiente", DateTime.UtcNow, null)
        };

        _cacheServiceMock
            .Setup(x => x.GetAsync<IEnumerable<InvoiceDto>>("invoices:all"))
            .ReturnsAsync(cachedInvoices);

        var result = await _handler.Handle(new GetAllInvoicesQuery(), CancellationToken.None);

        result.StatusCode.Should().Be(200);
        result.Data.Should().HaveCount(1);
        _invoiceRepositoryMock.Verify(x => x.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCacheMiss_ReturnsInvoicesFromRepository()
    {
        var invoices = new List<Invoice>
        {
            new() { Id = "1", ClientId = "c1", InvoiceNumber = "INV-001", Amount = 100000m, Status = InvoiceStatus.Pendiente, CreatedAt = DateTime.UtcNow },
            new() { Id = "2", ClientId = "c2", InvoiceNumber = "INV-002", Amount = 200000m, Status = InvoiceStatus.PrimerRecordatorio, CreatedAt = DateTime.UtcNow }
        };

        _cacheServiceMock
            .Setup(x => x.GetAsync<IEnumerable<InvoiceDto>>("invoices:all"))
            .ReturnsAsync((IEnumerable<InvoiceDto>?)null);

        _invoiceRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(invoices);

        var result = await _handler.Handle(new GetAllInvoicesQuery(), CancellationToken.None);

        result.StatusCode.Should().Be(200);
        result.Data.Should().HaveCount(2);
        _cacheServiceMock.Verify(x => x.SetAsync("invoices:all", It.IsAny<List<InvoiceDto>>(), It.IsAny<TimeSpan>()), Times.Once);
    }
}
