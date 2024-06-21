using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [EmailAddress]
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public bool IsOnline { get; set; }
        [ForeignKey(nameof(RoleId))]
        public int RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual RegularUser? RegularUser { get; set; }
        public virtual Admin? Admin { get; set; }
    }
}
