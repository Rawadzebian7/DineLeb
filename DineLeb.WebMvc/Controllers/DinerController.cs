using DineLeb.Application.Common.Interfaces;
using DineLeb.Domain.Entities;
using DineLeb.WebMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DineLeb.WebMvc.Controllers;

[Authorize]
public class DinerController : Controller
{
    private readonly IAsyncRepository<FeedbackTicket> _ticketRepo;
    private readonly IAsyncRepository<Review> _reviewRepo;
    private readonly IRestaurantRepository _restaurantRepo;
    private readonly UserManager<ApplicationUser> _userManager;

    public DinerController(
        IAsyncRepository<FeedbackTicket> ticketRepo,
        IAsyncRepository<Review> reviewRepo,
        IRestaurantRepository restaurantRepo,
        UserManager<ApplicationUser> userManager)
    {
        _ticketRepo = ticketRepo;
        _reviewRepo = reviewRepo;
        _restaurantRepo = restaurantRepo;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> MyActivity()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var email = currentUser?.Email ?? User.Identity?.Name ?? string.Empty;
        var fullName = currentUser?.FullName ?? string.Empty;

        var allTickets = await _ticketRepo.GetAllAsync();
        var userTickets = allTickets
            .Where(t => (!string.IsNullOrEmpty(t.CustomerId) &&
                        (t.CustomerId.Contains(email, StringComparison.OrdinalIgnoreCase) ||
                         (!string.IsNullOrEmpty(fullName) && t.CustomerId.Contains(fullName, StringComparison.OrdinalIgnoreCase)))))
            .OrderByDescending(t => t.CreatedAt)
            .ToList();

        var allReviews = await _reviewRepo.GetAllAsync();
        var userReviews = allReviews
            .Where(r => (!string.IsNullOrEmpty(r.CustomerId) &&
                        (r.CustomerId.Equals(fullName, StringComparison.OrdinalIgnoreCase) ||
                         r.CustomerId.Equals(email, StringComparison.OrdinalIgnoreCase))))
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        var allRestaurants = await _restaurantRepo.GetAllAsync();
        var restaurantDict = allRestaurants.ToDictionary(r => r.Id, r => r.Name);
        ViewBag.RestaurantNames = restaurantDict;

        var vm = new DinerDashboardViewModel
        {
            Tickets = userTickets,
            Reviews = userReviews
        };

        return View(vm);
    }
}