using MediatR;
using Microsoft.Extensions.Logging;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Application.Mappers;
using ReminderFlow.Domain.Exceptions;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Application.Clients.Queries;

public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ApiResponse<ClientDto>>
{
    private readonly IClientRepository _clientRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetClientByIdQueryHandler> _logger;

    public GetClientByIdQueryHandler(
        IClientRepository clientRepository,
        ICacheService cacheService,
        ILogger<GetClientByIdQueryHandler> logger)
    {
        _clientRepository = clientRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<ClientDto>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obteniendo cliente con ID: {Id}", request.Id);

        var cacheKey = $"clients:{request.Id}";
        var cachedClient = await _cacheService.GetAsync<ClientDto>(cacheKey);
        if (cachedClient != null)
        {
            _logger.LogInformation("Cliente obtenido desde caché: {Id}", request.Id);
            return ApiResponse<ClientDto>.Ok(cachedClient);
        }

        var client = await _clientRepository.GetByIdAsync(request.Id);
        if (client == null)
        {
            throw new NotFoundException("Cliente", request.Id);
        }

        var clientDto = client.ToDto();
        await _cacheService.SetAsync(cacheKey, clientDto, TimeSpan.FromMinutes(5));

        return ApiResponse<ClientDto>.Ok(clientDto);
    }
}
