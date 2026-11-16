namespace DineLeb.Domain.Entities;

public class Cuisine
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Many-to-Many relation with Restaurant
    public ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
}