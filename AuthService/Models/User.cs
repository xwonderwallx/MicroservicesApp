using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public User(string email, string passwordHash, string role)
        {
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            CreatedDate = DateTime.UtcNow;
        }
    }
}
