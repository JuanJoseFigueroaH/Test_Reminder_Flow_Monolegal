using MediatR;
using Microsoft.Extensions.Logging;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Application.Mappers;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Application.Clients.Commands;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ApiResponse<ClientDto>>
{
    private readonly IClientRepository _clientRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CreateClientCommandHandler> _logger;

    public CreateClientCommandHandler(
        IClientRepository clientRepository,
        ICacheService cacheService,
        ILogger<CreateClientCommandHandler> logger)
    {
        _clientRepository = clientRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<ClientDto>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creando nuevo cliente: {Name}", request.Name);

        var client = new CreateClientDto(request.Name, request.Email, request.Phone).ToEntity();
        var createdClient = await _clientRepository.CreateAsync(client);

        await _cacheService.RemoveByPatternAsync("clients:*");

        _logger.LogInformation("Cliente creado exitosamente con ID: {Id}", createdClient.Id);

        return ApiResponse<ClientDto>.Created(createdClient.ToDto(), "Cliente creado exitosamente");
    }
}
