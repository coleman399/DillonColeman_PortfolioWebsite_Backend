namespace PortfolioWebsite_Backend.Services.AuthService
{
    public interface IAuthService
    {
        public Task<UserServiceResponse<List<GetUserDto>>> GetUsers();
        public Task<UserServiceResponse<GetUserDto>> AddUser(RegisterUserDto newUser);
        public Task<UserServiceResponse<GetLoggedInUserDto>> LoginUser(LoginUserDto loginUser);
        public Task<UserServiceResponse<GetLoggedInUserDto>> UpdateUser(int id, UpdateUserDto user);
        public Task<UserServiceResponse<DeleteUserDto>> DeleteUser(int id);
    }
}
