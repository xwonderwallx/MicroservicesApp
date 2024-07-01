using AuthService.Interfaces;
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var token = _authService.Authenticate(user.Email, user.PasswordHash);
            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(new { token });
        }

        [HttpPost("validate")]
        public IActionResult Validate([FromBody] string token)
        {
            var isValid = _authService.ValidateToken(token);
            if (!isValid)
            {
                return Unauthorized();
            }

            return Ok();
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            var isRegistered = _authService.Register(user);
            if (!isRegistered)
            {
                return BadRequest("User already exists.");
            }

            // Mock implementation
            new MessagePublisher().Publish("User registered successfully");

            return Ok("User registered successfully.");
        }

        [HttpPut("update")]
        public IActionResult UpdatePassword([FromHeader] User user, [FromHeader] string password)
        {
            var isUpdated = _authService.UpdatePassword(user, password);
            if (!isUpdated)
            {
                return BadRequest("User already exists.");
            }

            // Mock implementation
            new MessagePublisher().Publish("User updated successfully");

            return Ok("User updated successfully.");
        }

        [HttpPost("logout")]
        public IActionResult Logout([FromBody] string token)
        {
            var isLoggedOut = _authService.Logout(token);
            if (!isLoggedOut)
            {
                return BadRequest("Invalid token.");
            }

            return Ok("Logged out successfully.");
        }
    }
}
