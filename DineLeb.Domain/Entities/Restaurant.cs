namespace DineLeb.Domain.Entities;

public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // Security & Business Verification
    public bool IsActive { get; set; } = false;
    public string? CommercialRegisterNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Ownership & Badges
    public string? OwnerId { get; set; }
    public bool IsVerified { get; set; } = false;

    // Ratings & Performance
    public double AverageRating { get; set; } = 0.0;
    public int ReviewCount { get; set; } = 0;
    public int TotalTicketsCount { get; set; } = 0;
    public int ResolvedTicketsCount { get; set; } = 0;

    // Subscription & Monetization (Whish / OMT)
    public bool IsSubscribed { get; set; } = false;
    public string? SubscriptionPlan { get; set; } = "Free";
    public string? PaymentMethod { get; set; }
    public string? PaymentReferenceNumber { get; set; }
    public DateTime? SubscriptionExpiresAt { get; set; }

    // Relations (One-to-Many & Many-to-Many)
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<FeedbackTicket> FeedbackTickets { get; set; } = new List<FeedbackTicket>();
    public ICollection<ClaimRequest> ClaimRequests { get; set; } = new List<ClaimRequest>();
    public ICollection<Cuisine> Cuisines { get; set; } = new List<Cuisine>();
}