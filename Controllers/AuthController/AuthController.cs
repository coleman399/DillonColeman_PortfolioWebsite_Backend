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

        [HttpGet("getUsers"), Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserServiceResponse<List<GetUserDto>>>> GetUsers()
        {
            UserServiceResponse<List<GetUserDto>> result = await _authService.GetUsers();
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }


        [HttpPost("register"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserServiceResponse<GetUserDto>>> RegisterUser(RegisterUserDto newUser)
        {
            UserServiceResponse<GetUserDto> result = await _authService.AddUser(newUser);
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("login"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserServiceResponse<GetLoggedInUserDto>>> LoginUser(LoginUserDto loginUser)
        {
            UserServiceResponse<GetLoggedInUserDto> result = await _authService.LoginUser(loginUser);
            if (result.Success == false) return Unauthorized(result);
            return Ok(result);
        }
    }
}
