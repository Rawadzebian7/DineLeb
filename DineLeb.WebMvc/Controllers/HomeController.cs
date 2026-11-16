using DineLeb.Application.Common.Interfaces;
using DineLeb.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DineLeb.WebMvc.Controllers;

public class HomeController : Controller
{
    private readonly IRestaurantRepository _restaurantRepo;
    private readonly IAsyncRepository<Review> _reviewRepo;
    private readonly IAsyncRepository<FeedbackTicket> _ticketRepo;

    public HomeController(
        IRestaurantRepository restaurantRepo,
        IAsyncRepository<Review> reviewRepo,
        IAsyncRepository<FeedbackTicket> ticketRepo)
    {
        _restaurantRepo = restaurantRepo;
        _reviewRepo = reviewRepo;
        _ticketRepo = ticketRepo;
    }

    public async Task<IActionResult> Index()
    {
        var restaurants = (await _restaurantRepo.GetAllAsync()).Where(r => r.IsActive).ToList();
        var reviews = await _reviewRepo.GetAllAsync();
        var tickets = await _ticketRepo.GetAllAsync();

        ViewBag.ActiveRestaurantsCount = restaurants.Count;
        ViewBag.TotalReviewsCount = reviews.Count();
        ViewBag.ResolvedTicketsCount = tickets.Count(t => t.Status == DineLeb.Domain.Enums.TicketStatus.Resolved);
        ViewBag.TopRestaurants = restaurants.OrderByDescending(r => r.AverageRating).Take(3).ToList();

        return View();
    }
}