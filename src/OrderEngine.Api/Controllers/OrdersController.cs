namespace OrderEngine.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderEngine.Application.Dtos;
using OrderEngine.Application.Orders.Commands;
using OrderEngine.Application.Orders.Queries;
using OrderEngine.Domain.Enums;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(request.CustomerEmail, request.Items);
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders([FromQuery] OrderStatus? status, CancellationToken cancellationToken)
    {
        var query = new GetOrdersListQuery(status);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateOrderStatusCommand(id, request.Status);
        var result = await _sender.Send(command, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
