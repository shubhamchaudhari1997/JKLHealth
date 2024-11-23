using System.ComponentModel.DataAnnotations;

namespace JKLHealthAPI.Models.Authentication
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name must be less than 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name must be less than 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "The Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        [Required(ErrorMessage = "User Type is required")]
        public UserType UserType { get; set; }
    }
}
