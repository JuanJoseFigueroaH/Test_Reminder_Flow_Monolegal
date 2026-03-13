using MediatR;
using Microsoft.Extensions.Logging;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Application.Mappers;
using ReminderFlow.Domain.Exceptions;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Application.Clients.Commands;

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, ApiResponse<ClientDto>>
{
    private readonly IClientRepository _clientRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<UpdateClientCommandHandler> _logger;

    public UpdateClientCommandHandler(
        IClientRepository clientRepository,
        ICacheService cacheService,
        ILogger<UpdateClientCommandHandler> logger)
    {
        _clientRepository = clientRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<ClientDto>> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Actualizando cliente con ID: {Id}", request.Id);

        var existingClient = await _clientRepository.GetByIdAsync(request.Id);
        if (existingClient == null)
        {
            throw new NotFoundException("Cliente", request.Id);
        }

        existingClient.UpdateEntity(new UpdateClientDto(request.Name, request.Email, request.Phone));
        var updatedClient = await _clientRepository.UpdateAsync(request.Id, existingClient);

        await _cacheService.RemoveByPatternAsync("clients:*");

        _logger.LogInformation("Cliente actualizado exitosamente: {Id}", request.Id);

        return ApiResponse<ClientDto>.Ok(updatedClient!.ToDto(), "Cliente actualizado exitosamente");
    }
}
