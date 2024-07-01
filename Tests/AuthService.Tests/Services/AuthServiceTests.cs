using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Models;

namespace AuthService.Tests.Services
{
    [TestFixture]
    public class AuthServiceTests
    {
        private AuthContext _context;
        private IConfiguration _configuration;
        private AuthService.Services.AuthService _authService;

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
        }

        [Test]
        public void Authenticate_ValidUser_ReturnsToken()
        {
            var email = $"{(new Random()).NextDouble()}@example.com";

            // Arrange
            _context.Users.Add(new User(email, "password", "User")
            {
                //Id = 1,
                CreatedDate = DateTime.UtcNow
            });
            _context.SaveChanges();

            // Act
            var token = _authService.Authenticate(email, "password");

            // Assert
            Assert.IsNotNull(token);
        }

        [Test]
        public void Authenticate_InvalidUser_ReturnsNull()
        {
            // Arrange
            // No users added to the context

            // Act
            var token = _authService.Authenticate("wrong@example.com", "password");

            // Assert
            Assert.IsNull(token);
        }

        [Test]
        public void ValidateToken_ValidToken_ReturnsTrue()
        {
            var email = "test2@example.com";
            var password = "password";

            // Arrange
            _context.Users.Add(new User(email, password, "User")
            {
                //Id = 3,
                CreatedDate = DateTime.UtcNow
            });
            _context.SaveChanges();

            var token = _authService.Authenticate(email, password);

            // Act
            var isValid = _authService.ValidateToken(token);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void ValidateToken_InvalidToken_ReturnsFalse()
        {
            // Act
            var isValid = _authService.ValidateToken("invalidtoken");

            // Assert
            Assert.IsFalse(isValid);
        }
    }
}
