﻿namespace PortfolioBackend.Services.UserService
{
    public interface IUserService
    {
        public Task<UserServiceResponse<List<GetUserDto>>> GetUsers();
        public Task<UserServiceResponse<GetUserDto>> RegisterUser(RegisterUserDto newUser);
        public Task<UserServiceResponse<GetLoggedInUserDto>> LoginUser(LoginUserDto loginUser);
        public Task<UserServiceResponse<GetLoggedInUserDto>> UpdateUser(int id, UpdateUserDto user);
        public Task<UserServiceResponse<DeleteUserDto>> DeleteUser(int id);
        public Task<UserServiceResponse<GetLoggedInUserDto>> RefreshToken();
        public Task<UserServiceResponse<GetForgotPasswordUserDto>> ForgotPassword(ForgotPasswordUserDto user);
        public Task<UserServiceResponse<GetResetPasswordUserDto>> ResetPasswordConfirmation(string token);
        public Task<UserServiceResponse<PasswordResetUserDto>> ResetPassword(ResetPasswordUserDto resetPassword);
        public Task<UserServiceResponse<GetLoggedOutUserDto>> LogoutUser();
        public void TokenCheck();
    }
}
