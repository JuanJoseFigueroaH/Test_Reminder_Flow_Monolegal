using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ReminderFlow.Application.Clients.Commands;
using ReminderFlow.Domain.Entities;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Tests.Clients;

public class CreateClientCommandHandlerTests
{
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<CreateClientCommandHandler>> _loggerMock;
    private readonly CreateClientCommandHandler _handler;

    public CreateClientCommandHandlerTests()
    {
        _clientRepositoryMock = new Mock<IClientRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<CreateClientCommandHandler>>();
        _handler = new CreateClientCommandHandler(
            _clientRepositoryMock.Object,
            _cacheServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCreatedClient()
    {
        var command = new CreateClientCommand("Juan Pérez", "juan@email.com", "+57 300 123 4567");
        var createdClient = new Client
        {
            Id = "507f1f77bcf86cd799439011",
            Name = command.Name,
            Email = command.Email,
            Phone = command.Phone,
            CreatedAt = DateTime.UtcNow
        };

        _clientRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Client>()))
            .ReturnsAsync(createdClient);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.StatusCode.Should().Be(201);
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(command.Name);
        result.Data.Email.Should().Be(command.Email);
        _cacheServiceMock.Verify(x => x.RemoveByPatternAsync("clients:*"), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_InvalidatesCacheAfterCreation()
    {
        var command = new CreateClientCommand("María García", "maria@email.com", "+57 301 234 5678");
        var createdClient = new Client
        {
            Id = "507f1f77bcf86cd799439012",
            Name = command.Name,
            Email = command.Email,
            Phone = command.Phone,
            CreatedAt = DateTime.UtcNow
        };

        _clientRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Client>()))
            .ReturnsAsync(createdClient);

        await _handler.Handle(command, CancellationToken.None);

        _cacheServiceMock.Verify(x => x.RemoveByPatternAsync("clients:*"), Times.Once);
    }
}
