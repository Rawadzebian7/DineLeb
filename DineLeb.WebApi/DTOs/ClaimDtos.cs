using System.ComponentModel.DataAnnotations;

namespace DineLeb.WebApi.DTOs;

public record CreateClaimDto(
    [Required] int RestaurantId,
    [Required] string UserId,
    [Required] string ContactPhone,
    [Required] string BusinessRegistrationDocumentUrl
);

public record ReviewClaimDto(
    [Required] bool Approve,
    string? AdminNotes
);

public record ClaimResponseDto(
    int Id,
    int RestaurantId,
    string UserId,
    string ContactPhone,
    string BusinessRegistrationDocumentUrl,
    string Status,
    DateTime RequestedAt,
    DateTime? ReviewedAt,
    string? AdminReviewNotes
);