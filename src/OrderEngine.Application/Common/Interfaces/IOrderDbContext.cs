namespace OrderEngine.Application.Common.Interfaces;

using Microsoft.EntityFrameworkCore;
using OrderEngine.Domain.Entities;

public interface IOrderDbContext
{
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
