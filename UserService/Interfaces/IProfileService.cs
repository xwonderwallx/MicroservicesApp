
using UserService.Models.Database;
using UserService.Models.DTO;
using UserService.Models.Requests;
using UserService.Models.Responses;

namespace UserService.Interfaces
{
    public interface IProfileService
    {
        Task<ServiceResponse> RegisterAsync(Credentials credentials);
        Task<User?> FindUserByEmailAsync(string email);
        Task<User?> FindUserByIdAsync(string id);
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        UserDTO GetUserDTO(User? user);
        Task<ServiceResponse> UpdateUserInformationAsync(UserDTO userDTO, string? password);
        Task<ServiceResponse> DeleteUserAsync(string id);
        Task<ServiceResponse> LogoutAsync(string token);
        Task<ServiceResponse> LoginAsync(Credentials credentials);
    }
}
