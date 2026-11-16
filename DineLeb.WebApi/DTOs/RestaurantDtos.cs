namespace DineLeb.WebApi.DTOs;

public record CreateRestaurantDto(
    string Name,
    string Description,
    string Address,
    string City,
    string PhoneNumber
);

public record RestaurantResponseDto(
    int Id,
    string Name,
    string Description,
    string Address,
    string City,
    string PhoneNumber,
    bool IsVerified,
    double AverageRating,
    int ReviewCount,
    int ResolvedTicketsCount
);