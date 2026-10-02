namespace OrderEngine.Application.Orders.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderEngine.Application.Common.Interfaces;
using OrderEngine.Application.Dtos;
using OrderEngine.Domain.Enums;

public record UpdateOrderStatusCommand(Guid Id, OrderStatus Status) : IRequest<OrderDto>;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    private readonly IOrderDbContext _context;

    public UpdateOrderStatusCommandHandler(IOrderDbContext context) => _context = context;

    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        order.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);

        var itemDtos = order.Items.Select(i => new OrderItemDto(i.Id, i.ProductName, i.Quantity, i.UnitPrice)).ToList();
        return new OrderDto(order.Id, order.OrderNumber, order.CustomerEmail, order.TotalAmount, order.Status, order.CreatedAt, itemDtos);
    }
}
