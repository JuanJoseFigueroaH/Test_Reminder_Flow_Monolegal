using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ReminderFlow.Application.Clients.Queries;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Domain.Entities;
using ReminderFlow.Domain.Exceptions;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Tests.Clients;

public class GetClientByIdQueryHandlerTests
{
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<GetClientByIdQueryHandler>> _loggerMock;
    private readonly GetClientByIdQueryHandler _handler;

    public GetClientByIdQueryHandlerTests()
    {
        _clientRepositoryMock = new Mock<IClientRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<GetClientByIdQueryHandler>>();
        _handler = new GetClientByIdQueryHandler(
            _clientRepositoryMock.Object,
            _cacheServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenClientExists_ReturnsClient()
    {
        var clientId = "507f1f77bcf86cd799439011";
        var client = new Client
        {
            Id = clientId,
            Name = "Juan Pérez",
            Email = "juan@email.com",
            Phone = "+57 300 123 4567",
            CreatedAt = DateTime.UtcNow
        };

        _cacheServiceMock
            .Setup(x => x.GetAsync<ClientDto>($"clients:{clientId}"))
            .ReturnsAsync((ClientDto?)null);

        _clientRepositoryMock
            .Setup(x => x.GetByIdAsync(clientId))
            .ReturnsAsync(client);

        var result = await _handler.Handle(new GetClientByIdQuery(clientId), CancellationToken.None);

        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(clientId);
    }

    [Fact]
    public async Task Handle_WhenClientNotFound_ThrowsNotFoundException()
    {
        var clientId = "507f1f77bcf86cd799439011";

        _cacheServiceMock
            .Setup(x => x.GetAsync<ClientDto>($"clients:{clientId}"))
            .ReturnsAsync((ClientDto?)null);

        _clientRepositoryMock
            .Setup(x => x.GetByIdAsync(clientId))
            .ReturnsAsync((Client?)null);

        var act = async () => await _handler.Handle(new GetClientByIdQuery(clientId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenCacheHit_ReturnsCachedClient()
    {
        var clientId = "507f1f77bcf86cd799439011";
        var cachedClient = new ClientDto(clientId, "Juan", "juan@email.com", "+57 300", DateTime.UtcNow, null);

        _cacheServiceMock
            .Setup(x => x.GetAsync<ClientDto>($"clients:{clientId}"))
            .ReturnsAsync(cachedClient);

        var result = await _handler.Handle(new GetClientByIdQuery(clientId), CancellationToken.None);

        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        _clientRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Never);
    }
}
