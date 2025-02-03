using Microsoft.EntityFrameworkCore;
using TradingMicroservice.Core.Abstraction;
using TradingMicroservice.Infrastructure.Data;
using TradingMicroservice.Infrastructure.Reposityory;

namespace TradingMicroservice.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TradingContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("TradingMicroservice.Infrastructure")));

        services.AddScoped<ITradeRepository, TradeRepository>();

        return services;
    }
}
