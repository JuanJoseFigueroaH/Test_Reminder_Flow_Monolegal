using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Application.Invoices.Commands;
using ReminderFlow.Application.Invoices.Queries;

namespace ReminderFlow.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<InvoiceListResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllInvoicesQuery());
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<InvoiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<InvoiceDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _mediator.Send(new GetInvoiceByIdQuery(id));
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("client/{clientId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<InvoiceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByClientId(string clientId)
    {
        var result = await _mediator.Send(new GetInvoicesByClientIdQuery(clientId));
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("status/{statuses}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<InvoiceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByStatus(string statuses)
    {
        var statusList = statuses.Split(',').Select(s => s.Trim());
        var result = await _mediator.Send(new GetInvoicesByStatusQuery(statusList));
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<InvoiceDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<InvoiceDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto)
    {
        var result = await _mediator.Send(new CreateInvoiceCommand(
            dto.ClientId,
            dto.InvoiceNumber,
            dto.Amount,
            dto.DueDate,
            dto.Status
        ));
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _mediator.Send(new DeleteInvoiceCommand(id));
        return StatusCode(result.StatusCode, result);
    }
}
