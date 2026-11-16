using Microsoft.AspNetCore.Identity;

namespace DineLeb.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Multi-tenant link: eza ken saheb mat3am, bykoun marbout b-mat3amo
    public int? RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }
}