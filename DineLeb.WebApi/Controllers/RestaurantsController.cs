using DineLeb.Application.Common.Interfaces;
using DineLeb.Domain.Entities;
using DineLeb.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DineLeb.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public RestaurantsController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // GET: api/restaurants?search=...&city=...
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RestaurantResponseDto>>> GetAll([FromQuery] string? search, [FromQuery] string? city, CancellationToken ct)
    {
        var restaurants = await _uow.Restaurants.GetAllAsync(ct);

        if (!string.IsNullOrWhiteSpace(search))
            restaurants = restaurants.Where(r =>
                r.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                r.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrWhiteSpace(city))
            restaurants = restaurants.Where(r =>
                r.City.Contains(city, StringComparison.OrdinalIgnoreCase)).ToList();

        var response = restaurants.Select(r => new RestaurantResponseDto(
            r.Id, r.Name, r.Description, r.Address, r.City, r.PhoneNumber,
            r.IsVerified, r.AverageRating, r.ReviewCount, r.ResolvedTicketsCount
        )).ToList();

        return Ok(response);
    }

    // GET: api/restaurants/leaderboard?count=10
    [HttpGet("leaderboard")]
    public async Task<ActionResult<IReadOnlyList<RestaurantResponseDto>>> GetLeaderboard([FromQuery] int count = 10, CancellationToken ct = default)
    {
        var top = await _uow.Restaurants.GetTopRankedRestaurantsAsync(count, ct);
        var response = top.Select(r => new RestaurantResponseDto(
            r.Id, r.Name, r.Description, r.Address, r.City, r.PhoneNumber,
            r.IsVerified, r.AverageRating, r.ReviewCount, r.ResolvedTicketsCount
        )).ToList();

        return Ok(response);
    }

    // GET: api/restaurants/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RestaurantResponseDto>> GetById(int id, CancellationToken ct)
    {
        var r = await _uow.Restaurants.GetByIdAsync(id, ct);
        if (r == null) return NotFound(new { message = $"Restaurant with ID {id} not found." });

        return Ok(new RestaurantResponseDto(
            r.Id, r.Name, r.Description, r.Address, r.City, r.PhoneNumber,
            r.IsVerified, r.AverageRating, r.ReviewCount, r.ResolvedTicketsCount
        ));
    }

    // POST: api/restaurants
    [HttpPost]
    public async Task<ActionResult<RestaurantResponseDto>> Create([FromBody] CreateRestaurantDto dto, CancellationToken ct)
    {
        var restaurant = new Restaurant
        {
            Name = dto.Name,
            Description = dto.Description,
            Address = dto.Address,
            City = dto.City,
            PhoneNumber = dto.PhoneNumber
        };

        var created = await _uow.Restaurants.AddAsync(restaurant, ct);

        var response = new RestaurantResponseDto(
            created.Id, created.Name, created.Description, created.Address, created.City, created.PhoneNumber,
            created.IsVerified, created.AverageRating, created.ReviewCount, created.ResolvedTicketsCount
        );

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }
}