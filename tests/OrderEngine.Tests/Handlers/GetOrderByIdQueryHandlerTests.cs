namespace OrderEngine.Tests.Handlers;

using Microsoft.EntityFrameworkCore;
using OrderEngine.Application.Orders.Queries;
using OrderEngine.Domain.Entities;
using OrderEngine.Domain.Enums;
using OrderEngine.Infrastructure.Persistence;
using Xunit;

public class GetOrderByIdQueryHandlerTests
{
    private OrderDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new OrderDbContext(options);
    }

    [Fact]
    public async Task Handle_ExistingOrder_ReturnsOrderDto()
    {
        var context = CreateInMemoryDbContext();
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            OrderNumber = "ORD-123456",
            CustomerEmail = "test@example.com",
            TotalAmount = 500.00m,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = new List<OrderItem>
            {
                new OrderItem { Id = Guid.NewGuid(), ProductName = "Keyboard", Quantity = 1, UnitPrice = 500.00m }
            }
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var handler = new GetOrderByIdQueryHandler(context);
        var query = new GetOrderByIdQuery(orderId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
        Assert.Equal("ORD-123456", result.OrderNumber);
        Assert.Equal("test@example.com", result.CustomerEmail);
    }

    [Fact]
    public async Task Handle_NonExistingOrder_ReturnsNull()
    {
        var context = CreateInMemoryDbContext();
        var handler = new GetOrderByIdQueryHandler(context);
        var query = new GetOrderByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }
}
