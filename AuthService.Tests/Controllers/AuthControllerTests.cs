using AuthService.Controllers;
using AuthService.Data;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace AuthService.Tests.Controllers
{
    public class AuthControllerTests
    {
        private AuthService.Services.AuthService _authService;
        private AuthController _controller;
        private AuthContext _context;
        private IConfiguration _configuration;


        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuthContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AuthContext(options);

            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "92d0895f1c174c05a0a26c755503796f"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _authService = new AuthService.Services.AuthService(_context, _configuration);
            _controller = new AuthController(_authService);
        }

        public IDictionary<string, object> DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            return jsonToken.Claims.ToDictionary(c => c.Type, c => (object)c.Value);
        }

        [Test]
        public void Login_ValidUser_ReturnsOkWithToken()
        {
            // Arrange
            var email = "test12@example.com";
            var password = "password";

            var user = new User(email, password, "User")
            {
                //Id = 2,
                CreatedDate = DateTime.UtcNow
            };

            // Arrange
            _context.Users.Add(user);
            _context.SaveChanges();

            var token = _authService.Authenticate(email, password);

            // Act
            var result = _controller.Login(user);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var resultToken = okResult.Value.GetType().GetProperty("token").GetValue(okResult.Value, null);

            // Decode tokens and compare claims
            var expectedClaims = DecodeToken(token);
            var actualClaims = DecodeToken(resultToken.ToString());

            foreach (var claim in expectedClaims)
            {
                Assert.IsTrue(actualClaims.ContainsKey(claim.Key), $"Claim {claim.Key} is missing in the actual token.");
                Assert.AreEqual(claim.Value, actualClaims[claim.Key], $"Claim {claim.Key} value does not match.");
            }
        }

        [Test]
        public void Login_InvalidUser_ReturnsUnauthorized()
        {
            // Arrange
            var email = "invalid@example.com"; // This email does not exist in the database
            var password = "wrongpassword";    // This password does not match any user's password

            // Act
            var result = _controller.Login(new User (email, password, "User"));

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public void Validate_ValidToken_ReturnsOk()
        {
            // Arrange
            var email = "test1234@example.com";
            var password = "password";

            var user = new User(email, password, "User")
            {
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var token = _authService.Authenticate(email, password);

            // Act
            var result = _controller.Validate(token);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public void Validate_InvalidToken_ReturnsUnauthorized()
        {
            // Act
            var result = _controller.Validate("invalid-token");

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public void Register_NewUser_ReturnsOk()
        {
            // Arrange
            var email = "newuser@example.com";
            var password = "password";

            var user = new User(email, password, "User")
            {
                CreatedDate = DateTime.UtcNow
            };

            // Act
            var result = _controller.Register(user);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("User registered successfully.", okResult.Value);
        }

        [Test]
        public void Register_ExistingUser_ReturnsBadRequest()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password";

            var user = new User(email, password, "User")
            {
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _controller.Register(user);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("User already exists.", badRequestResult.Value);
        }
    }
}
