using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace UserService.Models.Database
{
    public class Admin : User
    {
        [ForeignKey(nameof(PermissionId))]
        public string PermissionId { get; set; }

        [AllowNull]
        public DateTime CreatedDate { get; set; }

        [AllowNull] 
        public DateTime? UpdatedDate { get; set; }

        public virtual Permission? Permission { get; set; }
    }
}
