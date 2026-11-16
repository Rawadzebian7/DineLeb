namespace DineLeb.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public int Rating { get; set; } // 1 to 5
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Restaurant? Restaurant { get; set; }
}