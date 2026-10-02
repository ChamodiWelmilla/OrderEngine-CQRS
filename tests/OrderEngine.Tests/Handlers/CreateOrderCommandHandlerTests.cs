namespace OrderEngine.Tests.Handlers;

using Microsoft.EntityFrameworkCore;
using OrderEngine.Application.Dtos;
using OrderEngine.Application.Orders.Commands;
using OrderEngine.Infrastructure.Persistence;
using Xunit;

public class CreateOrderCommandHandlerTests
{
    private OrderDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new OrderDbContext(options);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesOrderAndCalculatesTotal()
    {
        var context = CreateInMemoryDbContext();
        var handler = new CreateOrderCommandHandler(context);

        var items = new List<CreateOrderItemRequest>
        {
            new CreateOrderItemRequest("Laptop", 1, 1200.00m),
            new CreateOrderItemRequest("Mouse", 2, 25.00m)
        };
        var command = new CreateOrderCommand("chamodi@example.com", items);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("chamodi@example.com", result.CustomerEmail);
        Assert.Equal(1250.00m, result.TotalAmount);
        Assert.Equal(2, result.Items.Count);

        var savedOrder = await context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == result.Id);
        Assert.NotNull(savedOrder);
        Assert.Equal("chamodi@example.com", savedOrder.CustomerEmail);
        Assert.Equal(1250.00m, savedOrder.TotalAmount);
    }
}
