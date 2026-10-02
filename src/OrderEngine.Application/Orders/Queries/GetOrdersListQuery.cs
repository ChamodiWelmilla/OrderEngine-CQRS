namespace OrderEngine.Application.Orders.Queries;

using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderEngine.Application.Common.Interfaces;
using OrderEngine.Application.Dtos;
using OrderEngine.Domain.Enums;

public record GetOrdersListQuery(OrderStatus? Status = null) : IRequest<List<OrderDto>>;

public class GetOrdersListQueryHandler : IRequestHandler<GetOrdersListQuery, List<OrderDto>>
{
    private readonly IOrderDbContext _context;

    public GetOrdersListQueryHandler(IOrderDbContext context) => _context = context;

    public async Task<List<OrderDto>> Handle(GetOrdersListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(o => o.Status == request.Status.Value);
        }

        var orders = await query.ToListAsync(cancellationToken);

        return orders.Select(o => new OrderDto(
            o.Id,
            o.OrderNumber,
            o.CustomerEmail,
            o.TotalAmount,
            o.Status,
            o.CreatedAt,
            o.Items.Select(i => new OrderItemDto(i.Id, i.ProductName, i.Quantity, i.UnitPrice)).ToList()
        )).ToList();
    }
}
