using System.ComponentModel.DataAnnotations;

namespace Flygans_Backend.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // ROLE BASED AUTH
        [Required]
        public string Role { get; set; } = "User";

        // REFRESH TOKEN
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // BLOCK / UNBLOCK
        public bool IsBlocked { get; set; } = false;

        // ⭐ SOFT DELETE SUPPORT
        public bool IsDeleted { get; set; } = false;
    }
}
