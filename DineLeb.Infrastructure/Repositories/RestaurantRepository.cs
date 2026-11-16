using DineLeb.Application.Common.Interfaces;
using DineLeb.Domain.Entities;
using DineLeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DineLeb.Infrastructure.Repositories;

public class RestaurantRepository : EfRepository<Restaurant>, IRestaurantRepository
{
    public RestaurantRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Restaurant>> GetAllWithCuisinesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Restaurants
            .Include(r => r.Cuisines)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Restaurant?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Restaurants
            .Include(r => r.Cuisines)
            .Include(r => r.Reviews)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Restaurant>> GetTopRankedRestaurantsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Restaurants
            .Include(r => r.Cuisines)
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.AverageRating)
            .ThenByDescending(r => r.TotalTicketsCount == 0 ? 1.0 : (double)r.ResolvedTicketsCount / r.TotalTicketsCount)
            .ThenByDescending(r => r.ReviewCount)
            .Take(count)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}