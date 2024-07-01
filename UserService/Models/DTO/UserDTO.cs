namespace UserService.Models.DTO
{
    public class UserDTO : DTO
    {
        public string UserId { get; set; }
        public string Role { get; set; }
        public bool IsOnline { get; set; }
    }
}
