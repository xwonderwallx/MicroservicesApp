using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace UserService.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Permissions { get; set; }
        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; }
        [AllowNull]
        public DateTime CreatedDate { get; set; }
        [AllowNull] 
        public DateTime? UpdatedDate { get; set; }

        public virtual User? User { get; set; }
    }
}
