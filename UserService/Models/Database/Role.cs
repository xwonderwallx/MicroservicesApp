using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Database
{
    [Index(nameof(Name), IsUnique = true)]
    public class Role
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<User>? Users { get; set; }
    }
}
