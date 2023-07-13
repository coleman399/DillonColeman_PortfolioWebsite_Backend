using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioWebsite_Backend.Controllers.AuthController
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // POST api/<AuthController>/getUsers
        [HttpGet("getUsers"), Authorize(Roles = "SuperUser, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserServiceResponse<List<GetUserDto>>>> GetUsers()
        {
            UserServiceResponse<List<GetUserDto>> result = await _userService.GetUsers();
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        // POST api/<AuthController>/register
        [HttpPost("register"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserServiceResponse<GetUserDto>>> RegisterUser(RegisterUserDto newUser)
        {
            UserServiceResponse<GetUserDto> result = await _userService.RegisterUser(newUser);
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        // POST api/<AuthController>/login
        [HttpPost("login"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<GetLoggedInUserDto>>> LoginUser(LoginUserDto loginUser)
        {
            UserServiceResponse<GetLoggedInUserDto> result = await _userService.LoginUser(loginUser);
            if (result.Success == false) return Unauthorized(result);
            return Ok(result);
        }

        // PUT api/<AuthController>/{id}
        [HttpPut("updateUser"), Authorize(Roles = "SuperUser, Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<GetLoggedInUserDto>>> UpdateUser(int id, [FromBody] UpdateUserDto user)
        {
            UserServiceResponse<GetLoggedInUserDto> result = await _userService.UpdateUser(id, user);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }

        // DELETE api/<AuthController>/{id}
        [HttpDelete("deleteUser"), Authorize(Roles = "SuperUser, Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<DeleteUserDto>>> DeleteUser(int id)
        {
            UserServiceResponse<DeleteUserDto> result = await _userService.DeleteUser(id);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }

        // Post api/<AuthController>/refreshToken
        [HttpPost("refreshToken"), Authorize(Roles = "SuperUser, Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<GetLoggedInUserDto>>> RefreshToken()
        {
            UserServiceResponse<GetLoggedInUserDto> result = await _userService.RefreshToken();
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }

        // Post api/<AuthController>/logout
        [HttpPost("logout"), Authorize(Roles = "SuperUser, Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<GetLoggedOutUserDto>>> Logout()
        {
            UserServiceResponse<GetLoggedOutUserDto> result = await _userService.Logout();
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);

        }

        // Post api/<AuthController>/forgotPassword
        [HttpPost("forgotPassword"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserServiceResponse<GetForgotPasswordUserDto>>> ForgotPassword(LoginUserDto user)
        {
            UserServiceResponse<GetForgotPasswordUserDto> result = await _userService.ForgotPassword(user);
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        // Post api/<AuthController>/resetPasswordConfirmation
        [HttpPost("resetPasswordConfirmation"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserServiceResponse<GetResetPasswordUserDto>>> ResetPasswordConfirmation(string token)
        {
            UserServiceResponse<GetResetPasswordUserDto> result = await _userService.ResetPasswordConfirmation(token);
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        // Post api/<AuthController>/resetPassword
        [HttpPost("resetPassword"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserServiceResponse<GetLoggedInUserDto>>> ResetPassword(LoginUserDto user)
        {
            UserServiceResponse<GetLoggedInUserDto> result = await _userService.ResetPassword(user);
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }
    }
}
