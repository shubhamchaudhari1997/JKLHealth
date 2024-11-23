using JKLHealthAPI.Models.Authentication;

namespace JKLHealthAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterModel model);
        Task<AuthResponse> LoginAsync(LoginModel model);
        Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken);
    }
}
