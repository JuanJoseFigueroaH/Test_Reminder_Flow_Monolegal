using MediatR;
using Microsoft.Extensions.Logging;
using ReminderFlow.Application.Common;
using ReminderFlow.Domain.Exceptions;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Application.Clients.Commands;

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, ApiResponse>
{
    private readonly IClientRepository _clientRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<DeleteClientCommandHandler> _logger;

    public DeleteClientCommandHandler(
        IClientRepository clientRepository,
        ICacheService cacheService,
        ILogger<DeleteClientCommandHandler> logger)
    {
        _clientRepository = clientRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Eliminando cliente con ID: {Id}", request.Id);

        var deleted = await _clientRepository.DeleteAsync(request.Id);
        if (!deleted)
        {
            throw new NotFoundException("Cliente", request.Id);
        }

        await _cacheService.RemoveByPatternAsync("clients:*");

        _logger.LogInformation("Cliente eliminado exitosamente: {Id}", request.Id);

        return ApiResponse.NoContent("Cliente eliminado exitosamente");
    }
}
