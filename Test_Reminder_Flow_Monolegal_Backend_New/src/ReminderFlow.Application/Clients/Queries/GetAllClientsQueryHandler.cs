using MediatR;
using Microsoft.Extensions.Logging;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Application.Mappers;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Application.Clients.Queries;

public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, ApiResponse<ClientListResponseDto>>
{
    private readonly IClientRepository _clientRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetAllClientsQueryHandler> _logger;
    private const string CacheKey = "clients:all";

    public GetAllClientsQueryHandler(
        IClientRepository clientRepository,
        ICacheService cacheService,
        ILogger<GetAllClientsQueryHandler> logger)
    {
        _clientRepository = clientRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<ClientListResponseDto>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obteniendo todos los clientes");

        var cachedResponse = await _cacheService.GetAsync<ClientListResponseDto>(CacheKey);
        if (cachedResponse != null)
        {
            _logger.LogInformation("Clientes obtenidos desde caché");
            return ApiResponse<ClientListResponseDto>.Ok(cachedResponse);
        }

        var clients = await _clientRepository.GetAllAsync();
        var clientDtos = clients.Select(c => c.ToDto()).ToList();
        var response = new ClientListResponseDto(clientDtos, clientDtos.Count);

        await _cacheService.SetAsync(CacheKey, response, TimeSpan.FromMinutes(5));

        _logger.LogInformation("Se obtuvieron {Count} clientes", clientDtos.Count);

        return ApiResponse<ClientListResponseDto>.Ok(response);
    }
}
