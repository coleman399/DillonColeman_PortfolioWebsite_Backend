using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioWebsite_Backend.Controllers.AuthController
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST api/<AuthController>/getUsers
        [HttpGet("getUsers"), Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserServiceResponse<List<GetUserDto>>>> GetUsers()
        {
            UserServiceResponse<List<GetUserDto>> result = await _authService.GetUsers();
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        // POST api/<AuthController>/register
        [HttpPost("register"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserServiceResponse<GetUserDto>>> RegisterUser(RegisterUserDto newUser)
        {
            UserServiceResponse<GetUserDto> result = await _authService.AddUser(newUser);
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        // POST api/<AuthController>/login
        [HttpPost("login"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<GetLoggedInUserDto>>> LoginUser(LoginUserDto loginUser)
        {
            UserServiceResponse<GetLoggedInUserDto> result = await _authService.LoginUser(loginUser);
            if (result.Success == false) return Unauthorized(result);
            return Ok(result);
        }

        // PUT api/<AuthController>/{id}
        // Admin should be able to update any user, user should only be able to update their own account
        [HttpPut("updateUser"), Authorize(Roles = "Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<GetLoggedInUserDto>>> UpdateUser(int id, [FromBody] UpdateUserDto user)
        {
            UserServiceResponse<GetLoggedInUserDto> result = await _authService.UpdateUser(id, user);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }

        // DELETE api/<AuthController>/{id}
        // Admin should be able to delete any user, user should only be able to delete their own account
        [HttpDelete("deleteUser"), Authorize(Roles = "Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<DeleteUserDto>>> DeleteContact(int id)
        {
            UserServiceResponse<DeleteUserDto> result = await _authService.DeleteUser(id);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }
    }
}
