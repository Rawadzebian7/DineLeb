using DineLeb.Application.Common.Interfaces;
using DineLeb.Domain.Entities;
using DineLeb.Domain.Enums;
using DineLeb.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DineLeb.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly IAsyncRepository<FeedbackTicket> _ticketRepo;
    private readonly IRestaurantRepository _restaurantRepo;

    public TicketsController(
        IAsyncRepository<FeedbackTicket> ticketRepo,
        IRestaurantRepository restaurantRepo)
    {
        _ticketRepo = ticketRepo;
        _restaurantRepo = restaurantRepo;
    }

    // POST: api/tickets (Customer submits private complaint)
    [HttpPost]
    public async Task<ActionResult<TicketResponseDto>> Create([FromBody] CreateTicketDto dto)
    {
        var restaurant = await _restaurantRepo.GetByIdAsync(dto.RestaurantId);
        if (restaurant == null)
            return NotFound(new { message = "Restaurant not found." });

        var ticket = new FeedbackTicket
        {
            RestaurantId = dto.RestaurantId,
            CustomerId = dto.CustomerId,
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _ticketRepo.AddAsync(ticket);

        restaurant.TotalTicketsCount += 1;
        await _restaurantRepo.UpdateAsync(restaurant);

        var response = new TicketResponseDto(
            created.Id, created.RestaurantId, created.CustomerId,
            created.Title, created.Description, created.Category.ToString(),
            created.Status.ToString(), created.CreatedAt, created.ResolvedAt, created.OwnerResolutionNotes
        );

        return Ok(response);
    }

    // PUT: api/tickets/5/resolve (Restaurant Owner resolves a complaint)
    [HttpPut("{id:int}/resolve")]
    public async Task<ActionResult> Resolve(int id, [FromBody] ResolveTicketDto dto)
    {
        var ticket = await _ticketRepo.GetByIdAsync(id);
        if (ticket == null)
            return NotFound(new { message = "Ticket not found." });

        if (ticket.Status == TicketStatus.Resolved)
            return BadRequest(new { message = "Ticket is already resolved." });

        ticket.Status = TicketStatus.Resolved;
        ticket.ResolvedAt = DateTime.UtcNow;
        ticket.OwnerResolutionNotes = dto.ResolutionNotes;
        await _ticketRepo.UpdateAsync(ticket);

        var restaurant = await _restaurantRepo.GetByIdAsync(ticket.RestaurantId);
        if (restaurant != null)
        {
            restaurant.ResolvedTicketsCount += 1;
            await _restaurantRepo.UpdateAsync(restaurant);
        }

        return Ok(new { message = "Ticket marked as resolved and restaurant score updated." });
    }
}