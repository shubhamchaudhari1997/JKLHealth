namespace JKLHealthAPI.Models.Authentication
{
    public class AuthResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
    }
}
