using DineLeb.Application.Common.Interfaces;
using DineLeb.Domain.Entities;
using DineLeb.Infrastructure.Persistence;

namespace DineLeb.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IRestaurantRepository Restaurants { get; }
    public IAsyncRepository<Review> Reviews { get; }
    public IAsyncRepository<FeedbackTicket> Tickets { get; }
    public IAsyncRepository<Cuisine> Cuisines { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IRestaurantRepository restaurants,
        IAsyncRepository<Review> reviews,
        IAsyncRepository<FeedbackTicket> tickets,
        IAsyncRepository<Cuisine> cuisines)
    {
        _context = context;
        Restaurants = restaurants;
        Reviews = reviews;
        Tickets = tickets;
        Cuisines = cuisines;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}