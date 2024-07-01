using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using UserService.Interfaces;
using UserService.Models.Requests;

namespace UserService.Services
{
    public class AuthServiceClient : IAuthServiceClient
    {
        private readonly HttpClient _httpClient;

        public AuthServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> RegisterUserAsync(Credentials credentials)
        {
            credentials.Password = EncryptPassword(credentials.Password);
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", credentials);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> AuthenticateUserAsync(Credentials credentials)
        {
            credentials.Password = EncryptPassword(credentials.Password);
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", credentials);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/validate", new { token });
            return response.IsSuccessStatusCode;
        }

        private string EncryptPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public void SetAuthorizationHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<bool> UpdatePassword(string password, string email)
        {
            var encryptedPassword = EncryptPassword(password);
            var response = await _httpClient.PutAsJsonAsync("/api/auth/update", new { email, password = encryptedPassword });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LogoutAsync(string token)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/logout", new { token });
            return response.IsSuccessStatusCode;
        }
    }
}
