﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.AuthController
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

        // POST api/Auth/getUsers
        [HttpGet("getUsers"), Authorize(Roles = "SuperUser, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserServiceResponse<List<GetUserDto>>>> GetUsers()
        {
            UserServiceResponse<List<GetUserDto>> result = await _userService.GetUsers();
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        // POST api/Auth/register
        [HttpPost("register"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<GetUserDto>>> RegisterUser([FromBody] RegisterUserDto newUser)
        {
            UserServiceResponse<GetUserDto> result = await _userService.RegisterUser(newUser);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Created("register", result);
        }

        // POST api/Auth/login
        [HttpPost("login"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<GetLoggedInUserDto>>> LoginUser([FromBody] LoginUserDto loginUser)
        {
            UserServiceResponse<GetLoggedInUserDto> result = await _userService.LoginUser(loginUser);
            if (result.Success == false) return Unauthorized(result);
            return Ok(result);
        }

        // PUT api/Auth/updateUser?id={id}
        [HttpPut("updateUser"), Authorize(Roles = "SuperUser, Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<GetLoggedInUserDto>>> UpdateUser([FromQuery] int id, [FromBody] UpdateUserDto user)
        {
            UserServiceResponse<GetLoggedInUserDto> result = await _userService.UpdateUser(id, user);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }

        // DELETE api/Auth/deleteUser?id={id}
        [HttpDelete("deleteUser"), Authorize(Roles = "SuperUser, Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<DeleteUserDto>>> DeleteUser([FromQuery] int id)
        {
            UserServiceResponse<DeleteUserDto> result = await _userService.DeleteUser(id);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }

        // Post api/Auth/refreshToken
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

        // Post api/Auth/logout
        [HttpPost("logout"), Authorize(Roles = "SuperUser, Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<GetLoggedOutUserDto>>> LogoutUser()
        {
            UserServiceResponse<GetLoggedOutUserDto> result = await _userService.LogoutUser();
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);

        }

        // Post api/Auth/forgotPassword
        [HttpPost("forgotPassword"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserServiceResponse<GetForgotPasswordUserDto>>> ForgotPassword(ForgotPasswordUserDto user)
        {
            UserServiceResponse<GetForgotPasswordUserDto> result = await _userService.ForgotPassword(user);
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        // Post api/Auth/resetPasswordConfirmation
        [HttpPost("resetPasswordConfirmation"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<GetResetPasswordUserDto>>> ResetPasswordConfirmation([FromQuery] string token)
        {
            UserServiceResponse<GetResetPasswordUserDto> result = await _userService.ResetPasswordConfirmation(token);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }

        // Post api/Auth/resetPassword
        [HttpPost("resetPassword"), Authorize(Roles = "SuperUser, Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<PasswordResetUserDto>>> ResetPassword([FromBody] ResetPasswordUserDto resetPassword)
        {
            UserServiceResponse<PasswordResetUserDto> result = await _userService.ResetPassword(resetPassword);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }
    }
}
