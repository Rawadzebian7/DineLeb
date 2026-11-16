using DineLeb.Application.Common.Interfaces;
using DineLeb.Infrastructure.Persistence;
using DineLeb.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DineLeb.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}