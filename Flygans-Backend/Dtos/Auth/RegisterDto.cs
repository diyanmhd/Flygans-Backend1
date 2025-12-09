using System.ComponentModel.DataAnnotations;

namespace Flygans_Backend.DTOs.Auth;

public class RegisterDto
{
    [Required(ErrorMessage = "Full Name is required")]
    [MinLength(3, ErrorMessage = "Full Name must be at least 3 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;
}
