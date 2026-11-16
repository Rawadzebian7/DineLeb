using System.Net.Http.Json;
using DineLeb.Domain.Entities;

namespace DineLeb.WebMvc.Services;

public class DineLebApiClient : IDineLebApiClient
{
    private readonly HttpClient _httpClient;

    public DineLebApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Restaurant>> GetRestaurantsAsync(string? searchTerm = null, string? city = null)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(searchTerm)) query.Add($"search={Uri.EscapeDataString(searchTerm)}");
        if (!string.IsNullOrWhiteSpace(city)) query.Add($"city={Uri.EscapeDataString(city)}");

        var url = "api/restaurants" + (query.Count > 0 ? "?" + string.Join("&", query) : "");
        return await _httpClient.GetFromJsonAsync<List<Restaurant>>(url) ?? new List<Restaurant>();
    }

    public async Task<List<Restaurant>> GetLeaderboardAsync(int count = 10)
    {
        return await _httpClient.GetFromJsonAsync<List<Restaurant>>($"api/restaurants/leaderboard?count={count}")
               ?? new List<Restaurant>();
    }

    public async Task<Restaurant?> GetRestaurantByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Restaurant>($"api/restaurants/{id}");
    }
}