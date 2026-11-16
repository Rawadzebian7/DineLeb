using DineLeb.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DineLeb.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<FeedbackTicket> FeedbackTickets => Set<FeedbackTicket>();
    public DbSet<ClaimRequest> ClaimRequests => Set<ClaimRequest>();
    public DbSet<Cuisine> Cuisines => Set<Cuisine>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Relation: Restaurant Owner link
        builder.Entity<Restaurant>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(r => r.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relation: User to Restaurant
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Restaurant)
            .WithMany()
            .HasForeignKey(u => u.RestaurantId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relation: Many-to-Many Restaurant <-> Cuisine
        builder.Entity<Restaurant>()
            .HasMany(r => r.Cuisines)
            .WithMany(c => c.Restaurants)
            .UsingEntity(j => j.ToTable("RestaurantCuisines"));
    }
}