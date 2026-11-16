using DineLeb.Domain.Enums;

namespace DineLeb.Domain.Entities;

public class FeedbackTicket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketCategory Category { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    // Multi-tenant Reference (Marbout bi ayya mat3am)
    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;

    // Customer Identity Reference (Men el-zboune li ba3ata)
    public string CustomerId { get; set; } = string.Empty;

    // Direct Business Resolution (Kif saheb el-mat3am hal el-qadiyeh)
    public string? OwnerResolutionNotes { get; set; }
}