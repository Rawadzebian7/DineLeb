using DineLeb.Domain.Entities;

namespace DineLeb.WebMvc.Models;

public class DinerDashboardViewModel
{
    public List<FeedbackTicket> Tickets { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}