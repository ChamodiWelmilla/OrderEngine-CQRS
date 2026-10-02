namespace OrderEngine.Application.Orders.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderEngine.Application.Common.Interfaces;
using OrderEngine.Application.Dtos;
using OrderEngine.Domain.Entities;
using OrderEngine.Domain.Enums;

public record CreateOrderCommand(string CustomerEmail, List<CreateOrderItemRequest> Items) : IRequest<OrderDto>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderDbContext _context;

    public CreateOrderCommandHandler(IOrderDbContext context) => _context = context;

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderNumber = $"ORD-{Random.Shared.Next(100000, 999999)}";
        var items = request.Items.Select(i => new OrderItem
        {
            ProductName = i.ProductName,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList();

        var totalAmount = items.Sum(i => i.Quantity * i.UnitPrice);

        var order = new Order
        {
            OrderNumber = orderNumber,
            CustomerEmail = request.CustomerEmail,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = items
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        var itemDtos = order.Items.Select(i => new OrderItemDto(i.Id, i.ProductName, i.Quantity, i.UnitPrice)).ToList();
        return new OrderDto(order.Id, order.OrderNumber, order.CustomerEmail, order.TotalAmount, order.Status, order.CreatedAt, itemDtos);
    }
}
