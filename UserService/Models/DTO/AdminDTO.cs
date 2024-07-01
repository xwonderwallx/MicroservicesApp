namespace UserService.Models.DTO
{
    public class AdminDTO : UserDTO
    {
        public string? Email { get; set; }
        public string? PermissionName { get; set; }
    }
}
