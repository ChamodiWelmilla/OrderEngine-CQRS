namespace OrderEngine.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderEngine.Application.Common.Interfaces;
using OrderEngine.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=orderengine.db";

        services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IOrderDbContext>(provider => provider.GetRequiredService<OrderDbContext>());

        return services;
    }
}
