using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Enums;
using UserService.Interfaces;
using UserService.Models.Database;
using UserService.Models.DTO;
using UserService.Models.Requests;
using UserService.Models.Responses;

namespace UserService.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserContext _context;
        private readonly IAuthServiceClient _authServiceClient;

        public ProfileService(UserContext context, IAuthServiceClient authServiceClient)
        {
            _context = context;
            _authServiceClient = authServiceClient;
        }

        public async Task<ServiceResponse> RegisterAsync(Credentials credentials)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(credentials.Role))
                {
                    return new ServiceResponse()
                    {
                        ResponseType = ResponseType.BadRequest,
                        ResponseMessage = "Role cannot be null or empty"
                    };
                }

                var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name.Trim().ToLower() == credentials.Role.Trim().ToLower());

                if (userRole == null)
                {
                    return new ServiceResponse()
                    {
                        ResponseType = ResponseType.NotFound,
                        ResponseMessage = "Role not found"
                    };
                }

                // Check if user already exists
                if (await _context.Users.FirstOrDefaultAsync(u => u.Email == credentials.Email) != null)
                {
                    return new ServiceResponse()
                    {
                        ResponseType = ResponseType.Conflict,
                        ResponseMessage = "User with this email already exists"
                    };
                }

                // Register user in AuthService
                if (string.IsNullOrEmpty(await _authServiceClient.RegisterUserAsync(credentials)))
                {
                    return new ServiceResponse()
                    {
                        ResponseType = ResponseType.InternalServerError,
                        ResponseMessage = "Failed to register user in AuthService"
                    };
                }

                var user = CreateNewUser(credentials, userRole.Id);

                return new ServiceResponse()
                {
                    ResponseType = ResponseType.Created,
                    ResponseMessage = "The request has succeeded and has led to the creation of a user, user ID: "
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse()
                {
                    ResponseType = ResponseType.InternalServerError,
                    ResponseMessage = $"An error occurred during registration, stack trace: {e.StackTrace}"
                };
            }
        }

        public async Task<ServiceResponse> LoginAsync(Credentials credentials)
        {
            if (credentials == null || string.IsNullOrWhiteSpace(credentials.Password) || string.IsNullOrWhiteSpace(credentials.Email) || string.IsNullOrWhiteSpace(credentials.Role))
            {
                return new ServiceResponse()
                {
                    ResponseType = ResponseType.BadRequest,
                    ResponseMessage = "Incorrect data"
                };
            }

            var user = await FindUserByEmailAsync(credentials.Email);
            if (user == null)
            {
                return new ServiceResponse()
                {
                    ResponseType = ResponseType.NotFound,
                    ResponseMessage = "User not found"
                };
            }

            // Login user in AuthService
            var authToken = await _authServiceClient.AuthenticateUserAsync(credentials);
            if (string.IsNullOrEmpty(authToken))
            {
                return new ServiceResponse()
                {
                    ResponseType = ResponseType.Unauthorized,
                    ResponseMessage = "Failed to login user in AuthService"
                };
            }

            var isCorrectPassword = BCrypt.Net.BCrypt.Verify(credentials.Password, user.PasswordHash);
            if (!isCorrectPassword)
            {
                return new ServiceResponse()
                {
                    ResponseType = ResponseType.Unauthorized,
                    ResponseMessage = "Incorrect password"
                };
            }

            _authServiceClient.SetAuthorizationHeader(authToken);
            return new ServiceResponse()
            {
                ResponseType = ResponseType.Ok,
                ResponseMessage = "Success",
                AdditionalObject = new { Token = authToken }
            };
        }

        public async Task<ServiceResponse> LogoutAsync(string token)
        {
            var response = await _authServiceClient.LogoutAsync(token);
            if (!response)
            {
                return new ServiceResponse()
                {
                    ResponseType = ResponseType.InternalServerError,
                    ResponseMessage = "Failed to logout user"
                };
            }

            return new ServiceResponse()
            {
                ResponseType = ResponseType.Ok,
                ResponseMessage = "Logout successful"
            };
        }

        public async Task<User?> FindUserByEmailAsync(string email)
        {
            return await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User?> FindUserByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            var finalUsersList = new List<UserDTO>();
            foreach (var user in users)
            {
                finalUsersList.Add(GetUserDTO(user));
            }
            return finalUsersList;
        }

        public UserDTO GetUserDTO(User? user)
        {
            string? role = GetUserRole(user);

            switch (role)
            {
                case UserRoleType.RegularUser:
                    return GetRegularUserDTO(user, role);

                case UserRoleType.Admin:
                    return GetAdminDTO(user, role);

                default:
                    throw new Exception("Unknown role type");
            }
        }

        public async Task<ServiceResponse> UpdateUserInformationAsync(UserDTO userDTO, string? password)
        {
            var user = await _context.Users.Where(u => u.Id == userDTO.UserId).FirstOrDefaultAsync();

            if (user == null)
            {
                return new ServiceResponse()
                {
                    ResponseType = ResponseType.NotFound,
                    ResponseMessage = $"User with ID {userDTO.UserId} does not exist"
                };
            }

            if (password != null)
            {
                _authServiceClient.UpdatePassword(password, user.Email);
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                _context.Update(user);
                _context.SaveChanges();
            }

            return await UpdateUserByRole(userDTO, user, userDTO.Role);
        }

        public async Task<ServiceResponse> DeleteUserAsync(string id)
        {
            var user = await FindUserByIdAsync(id);

            if (user == null)
            {
                return new ServiceResponse()
                {
                    ResponseType = ResponseType.NotFound,
                    ResponseMessage = "User with this ID does not exist"
                };
            }

            RemoveUserByRole(user, GetUserRole(user));
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new ServiceResponse()
            {
                ResponseType = ResponseType.NoContent,
                ResponseMessage = "User was removed"
            };
        }

        private UserDTO GetRegularUserDTO(User? user, string? role)
        {
            var regularUser = _context.RegularUsers.Find(user?.Id);
            return new RegularUserDTO()
            {
                FirstName = regularUser?.FirstName,
                LastName = regularUser?.LastName,
                Email = user?.Email,
                AvatarS3Url = regularUser?.AvatarS3Url,
                DefaultBillingAddress = regularUser?.DefaultBillingAddress,
                IsOnline = user.IsOnline,
                Role = role,
                UserId = user.Id
            };
        }

        private UserDTO GetAdminDTO(User? user, string? role)
        {
            var adminUser = _context.Admins.Find(user?.Id);
            return new AdminDTO()
            {
                Email = user?.Email,
                IsOnline = user.IsOnline,
                Role = role,
                PermissionName = _context.Permissions.Find(adminUser?.PermissionId).PermissionName,
                UserId = user.Id
            };
        }

        private User CreateNewUser(Credentials credentials, string roleId)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(credentials.Password);
            var user = new User()
            {
                Email = credentials.Email,
                RoleId = roleId,
                PasswordHash = hashedPassword,
                IsOnline = false
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        private string? GetUserRole(User? user)
        {
            return _context.Roles.Find(user?.RoleId)?.Name;
        }

        private async Task<ServiceResponse> UpdateUserByRole(UserDTO userDTO, User user, string role)
        {
            var specificUser = GetSpecificUserByRole(user, role);
            switch (role)
            {
                case UserRoleType.RegularUser:
                    var regularUser = specificUser as RegularUser;
                    return UpdateRegularUserAsync(userDTO as RegularUserDTO, regularUser);

                case UserRoleType.Admin:
                    var admin = specificUser as Admin;
                    return UpdateAdminAsync(userDTO as AdminDTO, admin);

                default:
                    throw new Exception(ExceptionMessage.UnknownRoleType);
            }
        }

        private ServiceResponse UpdateAdminAsync(AdminDTO? adminDTO, Admin admin)
        {
            if (adminDTO == null || admin == null)
            {
                return new ServiceResponse()
                {
                    ResponseType = ResponseType.NotFound
                };
            }

            if (adminDTO.PermissionName != null)
            {
                admin.PermissionId = _context.Permissions.Where(p => p.PermissionName == adminDTO.PermissionName).FirstOrDefault().Id;
            }

            _context.Update(admin);
            _context.SaveChanges();
            return new ServiceResponse()
            {
                ResponseType = ResponseType.NoContent,
                ResponseMessage = $"Admin with ID <{admin.Id}> was updated"
            };
        }

        private ServiceResponse UpdateRegularUserAsync(RegularUserDTO? regularUserDTO, RegularUser regularUser)
        {
            if (regularUserDTO == null || regularUser == null)
            {
                return new ServiceResponse()
                {
                    ResponseType = ResponseType.NotFound
                };
            }

            if (regularUserDTO.FirstName != null)
            {
                regularUser.FirstName = regularUserDTO.FirstName;
            }

            if (regularUserDTO.LastName != null)
            {
                regularUser.LastName = regularUserDTO.LastName;
            }

            if (regularUserDTO.AvatarS3Url != null)
            {
                regularUser.AvatarS3Url = regularUserDTO.AvatarS3Url;
            }

            if (regularUserDTO.DefaultBillingAddress != null)
            {
                regularUser.DefaultBillingAddress = regularUserDTO.DefaultBillingAddress;
            }

            _context.Update(regularUser);
            _context.SaveChanges();
            return new ServiceResponse()
            {
                ResponseType = ResponseType.NoContent,
                ResponseMessage = $"Regular user with ID <{regularUser.Id}> was updated"
            };
        }

        private void RemoveUserByRole(User? user, string? role)
        {
            var specificUser = GetSpecificUserByRole(user, role);
            switch (role)
            {
                case UserRoleType.RegularUser:
                    _context.RegularUsers.Remove(specificUser as RegularUser);
                    return;

                case UserRoleType.Admin:
                    _context.Admins.Remove(specificUser as Admin);
                    return;

                default:
                    break;
            }
        }

        private User? GetSpecificUserByRole(User? user, string? role)
        {
            switch (role)
            {
                case UserRoleType.RegularUser:
                    return _context.RegularUsers.Find(user.Id);

                case UserRoleType.Admin:
                    return _context.Admins.Find(user.Id);

                default:
                    throw new Exception(ExceptionMessage.UnknownRoleType);
            }
        }
    }
}
