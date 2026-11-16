using DineLeb.Application.Common.Interfaces;
using DineLeb.Domain.Entities;
using DineLeb.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DineLeb.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IAsyncRepository<Review> _reviewRepo;
    private readonly IRestaurantRepository _restaurantRepo;

    public ReviewsController(
        IAsyncRepository<Review> reviewRepo,
        IRestaurantRepository restaurantRepo)
    {
        _reviewRepo = reviewRepo;
        _restaurantRepo = restaurantRepo;
    }

    // POST: api/reviews
    [HttpPost]
    public async Task<ActionResult<ReviewResponseDto>> Create([FromBody] CreateReviewDto dto)
    {
        var restaurant = await _restaurantRepo.GetByIdAsync(dto.RestaurantId);
        if (restaurant == null)
            return NotFound(new { message = "Restaurant not found." });

        var review = new Review
        {
            RestaurantId = dto.RestaurantId,
            CustomerId = dto.CustomerId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _reviewRepo.AddAsync(review);

        // Update Restaurant Average Rating & Review Count
        var totalRating = (restaurant.AverageRating * restaurant.ReviewCount) + dto.Rating;
        restaurant.ReviewCount += 1;
        restaurant.AverageRating = Math.Round(totalRating / restaurant.ReviewCount, 1);
        await _restaurantRepo.UpdateAsync(restaurant);

        var response = new ReviewResponseDto(
            created.Id, created.RestaurantId, created.CustomerId,
            created.Rating, created.Comment, created.CreatedAt
        );

        return Ok(response);
    }
}