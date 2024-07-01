using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace UserService.Models.Database
{
    public class RegularUser : User
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [AllowNull]
        public string AvatarS3Url { get; set; }

        [AllowNull]
        public string DefaultBillingAddress { get; set; }

        [AllowNull]
        public DateTime CreatedDate { get; set; }

        [AllowNull]
        public DateTime? UpdatedDate { get; set; }
    }
}
