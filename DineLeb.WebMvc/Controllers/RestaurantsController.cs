using DineLeb.Application.Common.Interfaces;
using DineLeb.Domain.Entities;
using DineLeb.Domain.Enums;
using DineLeb.WebMvc.Models;
using DineLeb.WebMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DineLeb.WebMvc.Controllers;

public class RestaurantsController : Controller
{
    private readonly IRestaurantRepository _restaurantRepo;
    private readonly IAsyncRepository<Review> _reviewRepo;
    private readonly IAsyncRepository<FeedbackTicket> _ticketRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDineLebApiClient _apiClient;

    public RestaurantsController(
        IRestaurantRepository restaurantRepo,
        IAsyncRepository<Review> reviewRepo,
        IAsyncRepository<FeedbackTicket> ticketRepo,
        UserManager<ApplicationUser> userManager,
        IDineLebApiClient apiClient)
    {
        _restaurantRepo = restaurantRepo;
        _reviewRepo = reviewRepo;
        _ticketRepo = ticketRepo;
        _userManager = userManager;
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index(string? searchTerm, string? city)
    {
        var all = await _apiClient.GetRestaurantsAsync(searchTerm, city);
        var top = await _apiClient.GetLeaderboardAsync(5);

        var vm = new RestaurantListViewModel
        {
            Restaurants = all,
            TopLeaderboard = top
        };

        return View(vm);
    }

    public async Task<IActionResult> Leaderboard()
    {
        var top = await _apiClient.GetLeaderboardAsync(10);
        return View(top);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterRestaurantViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRestaurantViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var ownerUser = new ApplicationUser
        {
            UserName = model.OwnerEmail,
            Email = model.OwnerEmail,
            FullName = model.Name + " Management",
            PhoneNumber = model.PhoneNumber
        };

        var result = await _userManager.CreateAsync(ownerUser, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        await _userManager.AddToRoleAsync(ownerUser, "RestaurantOwner");

        var restaurant = new Restaurant
        {
            Name = model.Name,
            Description = model.Description,
            Address = model.Address,
            City = model.City,
            PhoneNumber = model.PhoneNumber,
            CommercialRegisterNumber = model.CommercialRegisterNumber,
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            OwnerId = ownerUser.Id,
            IsActive = false,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        await _restaurantRepo.AddAsync(restaurant);

        ownerUser.RestaurantId = restaurant.Id;
        await _userManager.UpdateAsync(ownerUser);

        return View("RegistrationPending");
    }

    public async Task<IActionResult> Details(int id)
    {
        var restaurant = await _apiClient.GetRestaurantByIdAsync(id) ?? await _restaurantRepo.GetByIdAsync(id);
        if (restaurant == null) return NotFound();

        var allReviews = await _reviewRepo.GetAllAsync();
        restaurant.Reviews = allReviews.Where(r => r.RestaurantId == id).OrderByDescending(r => r.CreatedAt).ToList();

        var vm = new RestaurantDetailsViewModel
        {
            Restaurant = restaurant,
            NewReview = new CreateReviewInputModel { RestaurantId = id },
            NewTicket = new CreateTicketInputModel { RestaurantId = id }
        };

        return View(vm);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(int restaurantId, int rating, string comment)
    {
        var restaurant = await _restaurantRepo.GetByIdAsync(restaurantId);
        if (restaurant == null) return NotFound();

        if (string.IsNullOrWhiteSpace(comment))
        {
            return RedirectToAction(nameof(Details), new { id = restaurantId });
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var authorName = !string.IsNullOrWhiteSpace(currentUser?.FullName) ? currentUser.FullName : (User.Identity?.Name ?? "Verified Diner");

        var review = new Review
        {
            RestaurantId = restaurantId,
            CustomerId = authorName,
            Rating = rating < 1 ? 5 : (rating > 5 ? 5 : rating),
            Comment = comment.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepo.AddAsync(review);

        var totalRating = (restaurant.AverageRating * restaurant.ReviewCount) + review.Rating;
        restaurant.ReviewCount += 1;
        restaurant.AverageRating = Math.Round(totalRating / restaurant.ReviewCount, 1);
        await _restaurantRepo.UpdateAsync(restaurant);

        TempData["ReviewSuccess"] = "Your review has been verified and posted!";
        return RedirectToAction(nameof(Details), new { id = restaurantId });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReview(int reviewId, int restaurantId)
    {
        var review = await _reviewRepo.GetByIdAsync(reviewId);
        if (review != null)
        {
            await _reviewRepo.DeleteAsync(review);

            var restaurant = await _restaurantRepo.GetByIdAsync(restaurantId);
            if (restaurant != null)
            {
                var remainingReviews = (await _reviewRepo.GetAllAsync())
                                        .Where(r => r.RestaurantId == restaurantId && r.Id != reviewId)
                                        .ToList();

                restaurant.ReviewCount = remainingReviews.Count;
                restaurant.AverageRating = restaurant.ReviewCount > 0
                    ? Math.Round(remainingReviews.Average(r => r.Rating), 1)
                    : 0.0;

                await _restaurantRepo.UpdateAsync(restaurant);
            }

            TempData["ReviewSuccess"] = "Review removed by administrator.";
        }

        return RedirectToAction(nameof(Details), new { id = restaurantId });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTicket(int restaurantId, string title, string description, TicketCategory category)
    {
        var restaurant = await _restaurantRepo.GetByIdAsync(restaurantId);
        if (restaurant == null) return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);

        var ticket = new FeedbackTicket
        {
            RestaurantId = restaurantId,
            CustomerId = $"{currentUser?.FullName ?? "Verified Diner"} ({currentUser?.Email ?? User.Identity?.Name})",
            Title = title,
            Description = description,
            Category = category,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        await _ticketRepo.AddAsync(ticket);
        restaurant.TotalTicketsCount += 1;
        await _restaurantRepo.UpdateAsync(restaurant);

        TempData["TicketSuccess"] = "Your private feedback ticket has been sent directly to management.";
        return RedirectToAction(nameof(Details), new { id = restaurantId });
    }

    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<IActionResult> Dashboard(int id)
    {
        var restaurant = await _restaurantRepo.GetByIdAsync(id);
        if (restaurant == null) return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (!User.IsInRole("Admin") && restaurant.OwnerId != currentUser?.Id && currentUser?.RestaurantId != restaurant.Id)
        {
            return Forbid();
        }

        var allTickets = await _ticketRepo.GetAllAsync();
        var restaurantTickets = allTickets.Where(t => t.RestaurantId == id).OrderByDescending(t => t.CreatedAt).ToList();

        var allReviews = await _reviewRepo.GetAllAsync();
        var restaurantReviews = allReviews.Where(r => r.RestaurantId == id).OrderByDescending(r => r.CreatedAt).ToList();

        var vm = new RestaurantDashboardViewModel
        {
            Restaurant = restaurant,
            Tickets = restaurantTickets,
            Reviews = restaurantReviews
        };

        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResolveTicket(int ticketId, int restaurantId, string? resolutionNotes)
    {
        var ticket = await _ticketRepo.GetByIdAsync(ticketId);
        if (ticket == null) return NotFound();

        if (ticket.Status != TicketStatus.Resolved)
        {
            ticket.Status = TicketStatus.Resolved;
            ticket.ResolvedAt = DateTime.UtcNow;
            ticket.OwnerResolutionNotes = string.IsNullOrWhiteSpace(resolutionNotes) ? "Resolved by management." : resolutionNotes;
            await _ticketRepo.UpdateAsync(ticket);

            var restaurant = await _restaurantRepo.GetByIdAsync(restaurantId);
            if (restaurant != null)
            {
                restaurant.ResolvedTicketsCount += 1;
                await _restaurantRepo.UpdateAsync(restaurant);
            }
        }

        TempData["DashboardSuccess"] = "Ticket resolved successfully! Trust score updated.";
        return RedirectToAction(nameof(Dashboard), new { id = restaurantId });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTicket(int ticketId, int restaurantId)
    {
        var ticket = await _ticketRepo.GetByIdAsync(ticketId);
        if (ticket != null)
        {
            await _ticketRepo.DeleteAsync(ticket);

            var restaurant = await _restaurantRepo.GetByIdAsync(restaurantId);
            if (restaurant != null && restaurant.TotalTicketsCount > 0)
            {
                restaurant.TotalTicketsCount -= 1;
                if (ticket.Status == TicketStatus.Resolved && restaurant.ResolvedTicketsCount > 0)
                {
                    restaurant.ResolvedTicketsCount -= 1;
                }
                await _restaurantRepo.UpdateAsync(restaurant);
            }

            TempData["DashboardSuccess"] = "Ticket removed by administrator for policy violation.";
        }

        return RedirectToAction(nameof(Dashboard), new { id = restaurantId });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Subscribe(int id)
    {
        var restaurant = await _restaurantRepo.GetByIdAsync(id);
        if (restaurant == null) return NotFound();

        return View(restaurant);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessSubscription(int restaurantId, string paymentMethod, string paymentRef)
    {
        var restaurant = await _restaurantRepo.GetByIdAsync(restaurantId);
        if (restaurant == null) return NotFound();

        if (string.IsNullOrWhiteSpace(paymentRef))
        {
            ModelState.AddModelError("", "Please enter the transfer transaction ID / MTCN.");
            return View("Subscribe", restaurant);
        }

        restaurant.IsSubscribed = true;
        restaurant.SubscriptionPlan = "Verified Pro Partner";
        restaurant.PaymentMethod = paymentMethod;
        restaurant.PaymentReferenceNumber = paymentRef.Trim();
        restaurant.SubscriptionExpiresAt = DateTime.UtcNow.AddMonths(1);

        await _restaurantRepo.UpdateAsync(restaurant);

        TempData["DashboardSuccess"] = $"Subscription activated successfully via {paymentMethod}! Reference: {paymentRef}";
        return RedirectToAction(nameof(Dashboard), new { id = restaurantId });
    }
}