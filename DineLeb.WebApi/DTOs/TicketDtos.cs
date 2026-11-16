using System.ComponentModel.DataAnnotations;
using DineLeb.Domain.Enums;

namespace DineLeb.WebApi.DTOs;

public record CreateTicketDto(
    [Required] int RestaurantId,
    [Required] string CustomerId,
    [Required, StringLength(120)] string Title,
    [Required, StringLength(2000)] string Description,
    TicketCategory Category
);

public record ResolveTicketDto(
    [Required] string ResolutionNotes
);

public record TicketResponseDto(
    int Id,
    int RestaurantId,
    string CustomerId,
    string Title,
    string Description,
    string Category,
    string Status,
    DateTime CreatedAt,
    DateTime? ResolvedAt,
    string? OwnerResolutionNotes
);