using DineLeb.Domain.Enums;

namespace DineLeb.Domain.Entities;

public class ClaimRequest
{
    public int Id { get; set; }
    public string BusinessRegistrationDocumentUrl { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
    public string? AdminReviewNotes { get; set; }

    // Restaurant Foreign Key & Navigation
    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;

    // Requesting User (Business Owner Candidate)
    public string UserId { get; set; } = string.Empty;
}