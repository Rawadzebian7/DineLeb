using DineLeb.Application.Common.Interfaces;
using DineLeb.Domain.Entities;
using DineLeb.Infrastructure.Persistence;
using DineLeb.Infrastructure.Repositories;
using DineLeb.WebMvc.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Typed HttpClient to consume DineLeb.WebApi
builder.Services.AddHttpClient<IDineLebApiClient, DineLebApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7250/");
});
// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\mssqllocaldb;Database=DineLebDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

// ASP.NET Core Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Repositories & Unit of Work
builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Seed Roles, Admin & Lebanese Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    await context.Database.MigrateAsync();

    // 1. Seed Roles
    string[] roleNames = { "Admin", "RestaurantOwner", "Diner" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // 2. Seed Super Admin Account
    var adminEmail = "admin@dineleb.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "DineLeb Platform Owner",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, "Admin@123456");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // 3. Seed Restaurants if empty
    if (!context.Restaurants.Any())
    {
        var babel = new Restaurant
        {
            Name = "Babel Bahr",
            Description = "Iconic Mediterranean seafood experience offering signature mezzeh and authentic coastal Lebanese dishes.",
            Address = "Old Sea Road",
            City = "Amchit",
            PhoneNumber = "+961 9 620888",
            AverageRating = 4.8,
            ReviewCount = 42,
            ResolvedTicketsCount = 5,
            TotalTicketsCount = 5,
            IsVerified = true,
            IsActive = true,
            CommercialRegisterNumber = "LB-982311"
        };

        var emSherif = new Restaurant
        {
            Name = "Em Sherif Restaurant",
            Description = "Distinguished fine dining oriental restaurant serving a lavish Lebanese menu with opulent traditional ambiance.",
            Address = "Victor Hugo Street, Achrafieh",
            City = "Beirut",
            PhoneNumber = "+961 1 206206",
            AverageRating = 4.9,
            ReviewCount = 78,
            ResolvedTicketsCount = 12,
            TotalTicketsCount = 12,
            IsVerified = true,
            IsActive = true,
            CommercialRegisterNumber = "LB-104922"
        };

        var abouJoseph = new Restaurant
        {
            Name = "Abou Joseph",
            Description = "Legendary Lebanese sandwich destination celebrated for premium charcoal-grilled shawarma and authentic street flavor.",
            Address = "Sin El Fil Highway",
            City = "Beirut",
            PhoneNumber = "+961 1 481999",
            AverageRating = 4.7,
            ReviewCount = 115,
            ResolvedTicketsCount = 3,
            TotalTicketsCount = 4,
            IsVerified = true,
            IsActive = true,
            CommercialRegisterNumber = "LB-443910"
        };

        context.Restaurants.AddRange(babel, emSherif, abouJoseph);
        await context.SaveChangesAsync();
    }
}

// HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();