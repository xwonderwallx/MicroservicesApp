using AuthService.Models;

namespace AuthService.Interfaces
{
    public interface IAuthService
    {
        string? Authenticate(string email, string encryptedPassword);
        bool ValidateToken(string token);
        bool Register(User user);
        bool UpdatePassword(User user, string newPassword);
        bool Logout(string token);
    }
}
