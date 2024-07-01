using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models.Database
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        public string Id { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public bool IsOnline { get; set; }

        [ForeignKey(nameof(RoleId))]
        public string RoleId { get; set; }

        public virtual Role? Role { get; set; }
    }
}
