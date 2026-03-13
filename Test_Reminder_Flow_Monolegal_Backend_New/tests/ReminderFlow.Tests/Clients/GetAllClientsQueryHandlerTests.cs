using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ReminderFlow.Application.Clients.Queries;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Domain.Entities;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Tests.Clients;

public class GetAllClientsQueryHandlerTests
{
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<GetAllClientsQueryHandler>> _loggerMock;
    private readonly GetAllClientsQueryHandler _handler;

    public GetAllClientsQueryHandlerTests()
    {
        _clientRepositoryMock = new Mock<IClientRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<GetAllClientsQueryHandler>>();
        _handler = new GetAllClientsQueryHandler(
            _clientRepositoryMock.Object,
            _cacheServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenCacheHit_ReturnsCachedClients()
    {
        var cachedClients = new List<ClientDto>
        {
            new("1", "Juan", "juan@email.com", "+57 300", DateTime.UtcNow, null)
        };

        _cacheServiceMock
            .Setup(x => x.GetAsync<IEnumerable<ClientDto>>("clients:all"))
            .ReturnsAsync(cachedClients);

        var result = await _handler.Handle(new GetAllClientsQuery(), CancellationToken.None);

        result.StatusCode.Should().Be(200);
        result.Data.Should().HaveCount(1);
        _clientRepositoryMock.Verify(x => x.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCacheMiss_ReturnsClientsFromRepository()
    {
        var clients = new List<Client>
        {
            new() { Id = "1", Name = "Juan", Email = "juan@email.com", Phone = "+57 300", CreatedAt = DateTime.UtcNow },
            new() { Id = "2", Name = "María", Email = "maria@email.com", Phone = "+57 301", CreatedAt = DateTime.UtcNow }
        };

        _cacheServiceMock
            .Setup(x => x.GetAsync<IEnumerable<ClientDto>>("clients:all"))
            .ReturnsAsync((IEnumerable<ClientDto>?)null);

        _clientRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(clients);

        var result = await _handler.Handle(new GetAllClientsQuery(), CancellationToken.None);

        result.StatusCode.Should().Be(200);
        result.Data.Should().HaveCount(2);
        _cacheServiceMock.Verify(x => x.SetAsync("clients:all", It.IsAny<List<ClientDto>>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoClients_ReturnsEmptyList()
    {
        _cacheServiceMock
            .Setup(x => x.GetAsync<IEnumerable<ClientDto>>("clients:all"))
            .ReturnsAsync((IEnumerable<ClientDto>?)null);

        _clientRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Client>());

        var result = await _handler.Handle(new GetAllClientsQuery(), CancellationToken.None);

        result.StatusCode.Should().Be(200);
        result.Data.Should().BeEmpty();
    }
}
