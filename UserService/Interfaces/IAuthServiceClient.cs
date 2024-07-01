using UserService.Models.Requests;

namespace UserService.Interfaces
{
    public interface IAuthServiceClient
    {
        Task<string> RegisterUserAsync(Credentials credentials);
        Task<string> AuthenticateUserAsync(Credentials credentials);
        Task<bool> ValidateTokenAsync(string token);
        void SetAuthorizationHeader(string token);
        Task<bool> UpdatePassword(string password, string email);
        Task<bool> LogoutAsync(string token);
    }
}
