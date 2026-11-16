using System.ComponentModel.DataAnnotations;

namespace DineLeb.WebMvc.Models;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}

public class RegisterDinerViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class RegisterRestaurantViewModel
{
    [Required]
    [Display(Name = "Restaurant Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Cuisine & Description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Street / Building Address")]
    public string Address { get; set; } = string.Empty;

    [Required]
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [Required]
    [Phone]
    [Display(Name = "Business Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Commercial Register Number (Sejjil Tijari)")]
    public string CommercialRegisterNumber { get; set; } = string.Empty;

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Owner Email")]
    public string OwnerEmail { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}