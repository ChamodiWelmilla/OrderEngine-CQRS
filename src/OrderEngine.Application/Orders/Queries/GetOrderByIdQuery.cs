namespace OrderEngine.Application.Orders.Queries;

using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderEngine.Application.Common.Interfaces;
using OrderEngine.Application.Dtos;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto?>;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderDbContext _context;

    public GetOrderByIdQueryHandler(IOrderDbContext context) => _context = context;

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order == null) return null;

        var itemDtos = order.Items.Select(i => new OrderItemDto(i.Id, i.ProductName, i.Quantity, i.UnitPrice)).ToList();
        return new OrderDto(order.Id, order.OrderNumber, order.CustomerEmail, order.TotalAmount, order.Status, order.CreatedAt, itemDtos);
    }
}
