using JKLHealthAPI.Models;
using JKLHealthAPI.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JKLHealthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return new AuthResponse { IsSuccess = false, Message = "User already exists!" };

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                UserType = model.UserType
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return new AuthResponse { IsSuccess = false, Message = string.Join(", ", result.Errors.Select(x => x.Description)),UserId };

            return await GenerateAuthResponseForUserAsync(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new AuthResponse { IsSuccess = false, Message = "Invalid credentials" };

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return new AuthResponse { IsSuccess = false, Message = "Invalid credentials" };

            return await GenerateAuthResponseForUserAsync(user);
        }

        private async Task<AuthResponse> GenerateAuthResponseForUserAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new AuthResponse
            {
                IsSuccess = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                // RefreshToken = GenerateRefreshToken(),
                ExpiryDate = expiry,
                UserType = user.UserType,
                UserId = user.Id,
                Message = "Authentication successful"
            };
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public async Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken)
        {
            // Implement refresh token logic here
            throw new NotImplementedException();
        }
    }
}
