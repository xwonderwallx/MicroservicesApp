using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Services.AuthService _authService;

        public AuthController(Services.AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var token = _authService.Authenticate(user.Email, user.PasswordHash);
            if (token == null)
                return Unauthorized();

            return Ok(new { token });
        }

        [HttpPost("validate")]
        public IActionResult Validate([FromBody] string token)
        {
            var isValid = _authService.ValidateToken(token);
            if (!isValid)
                return Unauthorized();

            return Ok();
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            var isRegistered = _authService.Register(user);
            if (!isRegistered)
                return BadRequest("User already exists.");

            return Ok("User registered successfully.");
        }
    }
}
