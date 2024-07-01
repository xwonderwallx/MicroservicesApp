using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class Token
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public DateTime ExpiryDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; }

        virtual public User User { get; set; }
    }
}
