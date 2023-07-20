using System.ComponentModel.DataAnnotations;

namespace JwtAuthApi.DTOs
{
    public class UserRegistrationDTO
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Password doesn't match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}