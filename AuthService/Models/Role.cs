using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Role(string name)
        {
            Name = name;
        }
    }
}
