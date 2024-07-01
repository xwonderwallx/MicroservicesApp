using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Database
{
    [Index(nameof(PermissionName), IsUnique = true)]
    public class Permission
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string PermissionName { get; set; }

        public virtual ICollection<Admin>? Admins { get; set; }
    }
}
