using DineLeb.Domain.Entities;

namespace DineLeb.WebMvc.Services;

public interface IDineLebApiClient
{
    Task<List<Restaurant>> GetRestaurantsAsync(string? searchTerm = null, string? city = null);
    Task<List<Restaurant>> GetLeaderboardAsync(int count = 10);
    Task<Restaurant?> GetRestaurantByIdAsync(int id);
}