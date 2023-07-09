namespace PortfolioWebsite_Backend.Services.UserService
{
    public interface IUserService
    {
        public Task<UserServiceResponse<List<GetUserDto>>> GetUsers();
        public Task<UserServiceResponse<GetUserDto>> RegisterUser(RegisterUserDto newUser);
        public Task<UserServiceResponse<GetLoggedInUserDto>> LoginUser(LoginUserDto loginUser);
        public Task<UserServiceResponse<GetLoggedInUserDto>> UpdateUser(int id, UpdateUserDto user);
        public Task<UserServiceResponse<DeleteUserDto>> DeleteUser(int id);
        public Task<UserServiceResponse<GetLoggedInUserDto>> RefreshToken();
        public void TokenCheck();
    }
}
