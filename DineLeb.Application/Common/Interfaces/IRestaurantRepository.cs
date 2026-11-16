using DineLeb.Domain.Entities;

namespace DineLeb.Application.Common.Interfaces;

public interface IRestaurantRepository : IAsyncRepository<Restaurant>
{
    Task<IReadOnlyList<Restaurant>> GetTopRankedRestaurantsAsync(int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Restaurant>> GetAllWithCuisinesAsync(CancellationToken cancellationToken = default);
    Task<Restaurant?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}