using System.ComponentModel.DataAnnotations;
using DineLeb.Domain.Entities;
using DineLeb.Domain.Enums;

namespace DineLeb.WebMvc.Models;

public class RestaurantListViewModel
{
    public IReadOnlyList<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    public IReadOnlyList<Restaurant> TopLeaderboard { get; set; } = new List<Restaurant>();
}

public class RestaurantDetailsViewModel
{
    public Restaurant Restaurant { get; set; } = default!;
    public CreateReviewInputModel NewReview { get; set; } = new();
    public CreateTicketInputModel NewTicket { get; set; } = new();
}

public class CreateReviewInputModel
{
    public int RestaurantId { get; set; }
    [Required] public string CustomerName { get; set; } = string.Empty;
    [Range(1, 5)] public int Rating { get; set; } = 5;
    [Required, StringLength(1000)] public string Comment { get; set; } = string.Empty;
}

public class CreateTicketInputModel
{
    public int RestaurantId { get; set; }
    [Required] public string CustomerName { get; set; } = string.Empty;
    [Required, StringLength(120)] public string Title { get; set; } = string.Empty;
    [Required, StringLength(2000)] public string Description { get; set; } = string.Empty;
    public TicketCategory Category { get; set; }
}

public class CreateRestaurantInputModel
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
    [Required] public string Address { get; set; } = string.Empty;
    [Required] public string City { get; set; } = string.Empty;
    [Required] public string PhoneNumber { get; set; } = string.Empty;
}

public class RestaurantDashboardViewModel
{
    public Restaurant Restaurant { get; set; } = default!;
    public IReadOnlyList<FeedbackTicket> Tickets { get; set; } = new List<FeedbackTicket>();
    public IReadOnlyList<Review> Reviews { get; set; } = new List<Review>();
}