using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Enums;
using UserService.Interfaces;
using UserService.Models.Database;
using UserService.Models.DTO;
using UserService.Models.Requests;
using UserService.Models.Responses;
using UserService.Services;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Register(Credentials credentials)
        {
            var response = await _profileService.RegisterAsync(credentials);
            return HandleServiceResponse(response, () => CreatedAtAction(nameof(GetUser), new { id = response.AdditionalObject }, response.AdditionalObject));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Login(Credentials credentials)
        {
            var response = await _profileService.LoginAsync(credentials);
            return HandleServiceResponse(response, () => Ok(response.AdditionalObject));
        }

        [HttpPost("logout")]
        public async Task<ActionResult<object>> Logout([FromHeader] string authorization)
        {
            var token = authorization?.Split(' ').Last();
            var response = await _profileService.LogoutAsync(token);
            return HandleServiceResponse(response, () => Ok(response.ResponseMessage));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _profileService.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(string id)
        {
            var user = await _profileService.FindUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(_profileService.GetUserDTO(user));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateUserInformation(UserDTO userDTO, string? password)
        {
            var response = await _profileService.UpdateUserInformationAsync(userDTO, password);
            return HandleServiceResponse(response, () => NoContent());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteUser(string id)
        {
            var response = await _profileService.DeleteUserAsync(id);
            return HandleServiceResponse(response, () => NoContent());
        }

        private ActionResult<object> HandleServiceResponse(ServiceResponse response, Func<ActionResult<object>> successAction)
        {
            return response.ResponseType switch
            {
                ResponseType.Conflict => Conflict(response.ResponseMessage),
                ResponseType.NotFound => NotFound(response.ResponseMessage),
                ResponseType.BadRequest => BadRequest(response.ResponseMessage),
                ResponseType.InternalServerError => StatusCode(500, response.ResponseMessage),
                ResponseType.Created => successAction.Invoke(),
                ResponseType.Ok => successAction.Invoke(),
                ResponseType.NoContent => successAction.Invoke(),
                ResponseType.Unauthorized => Unauthorized(response.ResponseMessage),
                _ => throw new Exception(ExceptionMessage.UnknownResponseType)
            };
        }

        private async Task<ActionResult<object>> ProcessRegistrationAsync(Credentials credentials)
        {
            var response = await _profileService.RegisterAsync(credentials);
            return HandleServiceResponse(response, () => CreatedAtAction(nameof(GetUser), new { id = response.AdditionalObject }, response.AdditionalObject));
        }

        private async Task<ActionResult<object>> ProcessLoginAsync(Credentials credentials)
        {
            var response = await _profileService.LoginAsync(credentials);
            return HandleServiceResponse(response, () => Ok(response.AdditionalObject));
        }

        private async Task<ActionResult<object>> ProcessUpdateAsync(UserDTO userDTO, string? password)
        {
            var response = await _profileService.UpdateUserInformationAsync(userDTO, password);
            return HandleServiceResponse(response, () => NoContent());
        }

        private async Task<ActionResult<object>> ProcessDeleteAsync(string id)
        {
            var response = await _profileService.DeleteUserAsync(id);
            return HandleServiceResponse(response, () => NoContent());
        }
    }
}
