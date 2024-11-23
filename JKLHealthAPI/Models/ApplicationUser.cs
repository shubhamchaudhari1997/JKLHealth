using Microsoft.AspNetCore.Identity;

namespace JKLHealthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }
        public UserType UserType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }

    public enum UserType
    {
        Administrator,
        Caregiver,
        Patient
    }
}
