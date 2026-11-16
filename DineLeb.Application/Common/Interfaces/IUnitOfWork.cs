using DineLeb.Domain.Entities;

namespace DineLeb.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRestaurantRepository Restaurants { get; }
    IAsyncRepository<Review> Reviews { get; }
    IAsyncRepository<FeedbackTicket> Tickets { get; }
    IAsyncRepository<Cuisine> Cuisines { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}