using System.ComponentModel.DataAnnotations;

namespace DineLeb.WebApi.DTOs;

public record CreateReviewDto(
    [Required] int RestaurantId,
    [Required] string CustomerId,
    [Range(1, 5)] int Rating,
    [Required, StringLength(1000)] string Comment
);

public record ReviewResponseDto(
    int Id,
    int RestaurantId,
    string CustomerId,
    int Rating,
    string Comment,
    DateTime CreatedAt
);