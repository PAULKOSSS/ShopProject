using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopApi.DTO;
using ShopApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopApi.Services
{
    public class AuthService
    {
        private readonly ISRPO_PR4Context _context;
        private readonly PasswordHasher<Users> _passwordHasher = new();
        private readonly IConfiguration _configuration;

        public AuthService(ISRPO_PR4Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<(AuthDtos.AuthResponse? Data, string? Error)> RegisterAsync(AuthDtos.RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password)) return (null, "Логин и пароль не могут быть пустыми.");

            if (request.Password != request.SecPassword) return (null, "Пароли не совпадают.");

            var exists = await _context.Users.AnyAsync(u => u.Login == request.Login);
            if (exists) return (null, "Пользователь с таким логином уже существует.");

            var user = new Users
            {
                Login = request.Login,
                Phone = request.PhoneNumber,
                DeliveryAddress = request.DeliveryAddress,
                Role = "Customer"
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return (new AuthDtos.AuthResponse(token, user.Login, user.Role), null);
        }

        public async Task<AuthDtos.AuthResponse?> LoginAsync(AuthDtos.LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.Login);
            if (user == null) return null;
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed) return null;
            var token = GenerateJwtToken(user);
            return new AuthDtos.AuthResponse(token, user.Login, user.Role);
        }

        private string GenerateJwtToken(Users user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.Role, user.Role)
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
