using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Requests
{
    public class Credentials
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public Credentials(string email, string password, string role)
        {
            Email = email;
            Password = password;
            Role = role;    
        }
    }
}
