using DineLeb.Application.Common.Interfaces;
using DineLeb.Domain.Entities;
using DineLeb.Domain.Enums;
using DineLeb.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DineLeb.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IAsyncRepository<ClaimRequest> _claimRepo;
    private readonly IRestaurantRepository _restaurantRepo;

    public ClaimsController(
        IAsyncRepository<ClaimRequest> claimRepo,
        IRestaurantRepository restaurantRepo)
    {
        _claimRepo = claimRepo;
        _restaurantRepo = restaurantRepo;
    }

    // POST: api/claims (Business owner submits ownership claim)
    [HttpPost]
    public async Task<ActionResult<ClaimResponseDto>> Create([FromBody] CreateClaimDto dto)
    {
        var restaurant = await _restaurantRepo.GetByIdAsync(dto.RestaurantId);
        if (restaurant == null)
            return NotFound(new { message = "Restaurant not found." });

        if (restaurant.IsVerified)
            return BadRequest(new { message = "Restaurant is already claimed and verified." });

        var claim = new ClaimRequest
        {
            RestaurantId = dto.RestaurantId,
            UserId = dto.UserId,
            ContactPhone = dto.ContactPhone,
            BusinessRegistrationDocumentUrl = dto.BusinessRegistrationDocumentUrl,
            Status = ClaimStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        var created = await _claimRepo.AddAsync(claim);

        var response = new ClaimResponseDto(
            created.Id, created.RestaurantId, created.UserId,
            created.ContactPhone, created.BusinessRegistrationDocumentUrl,
            created.Status.ToString(), created.RequestedAt, created.ReviewedAt, created.AdminReviewNotes
        );

        return Ok(response);
    }

    // PUT: api/claims/5/review (Admin approves or rejects claim)
    [HttpPut("{id:int}/review")]
    public async Task<ActionResult> ReviewClaim(int id, [FromBody] ReviewClaimDto dto)
    {
        var claim = await _claimRepo.GetByIdAsync(id);
        if (claim == null)
            return NotFound(new { message = "Claim request not found." });

        if (claim.Status != ClaimStatus.Pending)
            return BadRequest(new { message = "Claim request has already been processed." });

        claim.Status = dto.Approve ? ClaimStatus.Approved : ClaimStatus.Rejected;
        claim.AdminReviewNotes = dto.AdminNotes;
        claim.ReviewedAt = DateTime.UtcNow;
        await _claimRepo.UpdateAsync(claim);

        if (dto.Approve)
        {
            var restaurant = await _restaurantRepo.GetByIdAsync(claim.RestaurantId);
            if (restaurant != null)
            {
                restaurant.IsVerified = true;
                restaurant.OwnerId = claim.UserId;
                await _restaurantRepo.UpdateAsync(restaurant);
            }
        }

        return Ok(new { message = $"Claim has been {(dto.Approve ? "approved" : "rejected")} successfully." });
    }
}