using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }

        public virtual ICollection<User>? Users { get; set; }
    }
}
