using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ReminderFlow.Application.Clients.Commands;
using ReminderFlow.Domain.Exceptions;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Tests.Clients;

public class DeleteClientCommandHandlerTests
{
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<DeleteClientCommandHandler>> _loggerMock;
    private readonly DeleteClientCommandHandler _handler;

    public DeleteClientCommandHandlerTests()
    {
        _clientRepositoryMock = new Mock<IClientRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<DeleteClientCommandHandler>>();
        _handler = new DeleteClientCommandHandler(
            _clientRepositoryMock.Object,
            _cacheServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenClientExists_DeletesSuccessfully()
    {
        var clientId = "507f1f77bcf86cd799439011";

        _clientRepositoryMock
            .Setup(x => x.DeleteAsync(clientId))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new DeleteClientCommand(clientId), CancellationToken.None);

        result.StatusCode.Should().Be(204);
        _cacheServiceMock.Verify(x => x.RemoveByPatternAsync("clients:*"), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenClientNotFound_ThrowsNotFoundException()
    {
        var clientId = "507f1f77bcf86cd799439011";

        _clientRepositoryMock
            .Setup(x => x.DeleteAsync(clientId))
            .ReturnsAsync(false);

        var act = async () => await _handler.Handle(new DeleteClientCommand(clientId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
