using AuthService.Data;
using AuthService.Interfaces;
using AuthService.Models;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AuthContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string? Authenticate(string email, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == email);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenValue = tokenHandler.WriteToken(token);

            _context.Tokens.Add(new Token
            {
                Value = tokenValue,
                ExpiryDate = tokenDescriptor.Expires.Value,
                UserId = user.Id
            });

            _context.SaveChanges();

            return tokenValue;
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Register(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                return false;
            }

            user.PasswordHash = HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public bool UpdatePassword(User user, string newPassword)
        {
            user.PasswordHash = HashPassword(newPassword);
            _context.Update(user);
            _context.SaveChanges();
            return true;
        }

        public bool Logout(string token)
        {
            var tokenEntity = _context.Tokens.SingleOrDefault(t => t.Value == token);
            if (tokenEntity != null)
            {
                _context.Tokens.Remove(tokenEntity);
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
