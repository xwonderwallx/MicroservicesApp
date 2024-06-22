using UserService.Models;

namespace UserService.Services
{
    public class AuthServiceClient
    {
        private readonly HttpClient _httpClient;

        public AuthServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> RegisterUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", new {user.Email, user.Password, user.Role});
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> AuthenticateUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new { user.Email, user.Password, user.Role });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
