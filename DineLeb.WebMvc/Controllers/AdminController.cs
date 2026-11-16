using DineLeb.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DineLeb.WebMvc.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IRestaurantRepository _restaurantRepo;

    public AdminController(IRestaurantRepository restaurantRepo)
    {
        _restaurantRepo = restaurantRepo;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var allRestaurants = await _restaurantRepo.GetAllAsync();
        return View(allRestaurants.OrderByDescending(r => r.CreatedAt).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveRestaurant(int id)
    {
        var restaurant = await _restaurantRepo.GetByIdAsync(id);
        if (restaurant != null)
        {
            restaurant.IsActive = true;
            restaurant.IsVerified = true;
            await _restaurantRepo.UpdateAsync(restaurant);
            TempData["AdminMessage"] = $"Restaurant '{restaurant.Name}' has been approved and activated!";
        }
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectRestaurant(int id)
    {
        var restaurant = await _restaurantRepo.GetByIdAsync(id);
        if (restaurant != null)
        {
            await _restaurantRepo.DeleteAsync(restaurant);
            TempData["AdminMessage"] = $"Application for '{restaurant.Name}' was rejected and removed.";
        }
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRestaurant(int id)
    {
        var restaurant = await _restaurantRepo.GetByIdAsync(id);
        if (restaurant != null)
        {
            await _restaurantRepo.DeleteAsync(restaurant);
            TempData["AdminMessage"] = $"Restaurant '{restaurant.Name}' has been permanently removed.";
        }
        return RedirectToAction(nameof(Dashboard));
    }
}