namespace UserService.Models.DTO
{
    public class RegularUserDTO : UserDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? AvatarS3Url { get; set; }
        public string? DefaultBillingAddress { get; set; }
    }
}
