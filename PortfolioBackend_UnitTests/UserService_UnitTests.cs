using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using PortfolioBackend.Dtos.UserDtos;
using PortfolioBackend.Helpers;
using PortfolioBackend.Models.UserModel;
using System.Net;
using System.Net.Http.Json;

namespace PortfolioBackend_UnitTests
{
    [TestFixture]
    public class UserService_UnitTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void Cleanup()
        {
            _factory.Dispose();
            _client.Dispose();
        }

        [Test]
        public void GetUsers_Positive_SuperUser()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var getUsersResponse = _client.GetAsync("api/Auth/getUsers").GetAwaiter().GetResult();
            Assert.That(getUsersResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetUsers_Positive_Admin()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var getUsersResponse = _client.GetAsync("api/Auth/getUsers").GetAwaiter().GetResult();
            Assert.That(getUsersResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetUsers_Negative_User()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var getUsersResponse = _client.GetAsync("api/Auth/getUsers").GetAwaiter().GetResult();
            Assert.That(getUsersResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public void RegisterUser_Positive()
        {
            var registerUser = new RegisterUserDto()
            {
                UserName = "JohnTester",
                Email = "Test@test.test",
                Role = "User",
                Password = "Test1234!",
                PasswordConfirmation = "Test1234!"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/register")
            {
                Content = JsonContent.Create(registerUser)
            };
            var registerUserResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(registerUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void RegisterUser_Positive_CreateAdminAsSuperUser()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            var registerUser = new RegisterUserDto()
            {
                UserName = "JohnTester",
                Email = "Test@test.test",
                Role = "Admin",
                Password = "Test1234!",
                PasswordConfirmation = "Test1234!"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/register")
            {
                Content = JsonContent.Create(registerUser)
            };
            _client.SetBearerToken(token);
            var registerUserResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(registerUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void RegisterUser_Negative_InvalidUserName()
        {
            var registerUser = new RegisterUserDto()
            {
                UserName = "",
                Email = "Test@test.test",
                Role = "User",
                Password = "Test1234!",
                PasswordConfirmation = "Test1234!"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/register")
            {
                Content = JsonContent.Create(registerUser)
            };
            var registerUserResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(registerUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void RegisterUser_Negative_InvalidEmail()
        {
            var registerUser = new RegisterUserDto()
            {
                UserName = "JohnTester",
                Email = "Test@",
                Role = "User",
                Password = "Test1234!",
                PasswordConfirmation = "Test1234!"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/register")
            {
                Content = JsonContent.Create(registerUser)
            };
            var registerUserResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(registerUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void RegisterUser_Negative_InvalidPassword()
        {
            var registerUser = new RegisterUserDto()
            {
                UserName = "JohnTester",
                Email = "Test@test.test",
                Role = "User",
                Password = "invalid",
                PasswordConfirmation = "invalid"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/register")
            {
                Content = JsonContent.Create(registerUser)
            };
            var registerUserResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(registerUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void RegisterUser_Negative_RoleAdmin()
        {
            var registerUser = new RegisterUserDto()
            {
                UserName = "JohnTester",
                Email = "Test@test.test",
                Role = "Admin",
                Password = "invalid",
                PasswordConfirmation = "invalid"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/register")
            {
                Content = JsonContent.Create(registerUser)
            };
            var registerUserResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(registerUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void RegisterUser_Negative_RoleSuperUser()
        {

            var registerUser = new RegisterUserDto()
            {
                UserName = "JohnTester",
                Email = "Test@test.test",
                Role = "SuperUser",
                Password = "invalid",
                PasswordConfirmation = "invalid"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/register")
            {
                Content = JsonContent.Create(registerUser)
            };
            var registerUserResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(registerUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void LoginUser_Positive_JustUserName()
        {
            var loginUser = new LoginUserDto()
            {
                UserName = "TestUser1",
                Email = "",
                Password = "UserPassword1",
                PasswordConfirmation = "UserPassword1"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
            {
                Content = JsonContent.Create(loginUser)
            };
            var userLoginResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(userLoginResponse.IsSuccessStatusCode);
        }

        [Test]
        public void LoginUser_Positive_JustEmail()
        {
            var loginUser = new LoginUserDto()
            {
                UserName = "",
                Email = "User1Email@test.test",
                Password = "UserPassword1",
                PasswordConfirmation = "UserPassword1"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
            {
                Content = JsonContent.Create(loginUser)
            };
            var userLoginResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(userLoginResponse.IsSuccessStatusCode);
        }

        [Test]
        public void LoginUser_Negative_UserDoesNotExist()
        {
            var loginUser = new LoginUserDto()
            {
                UserName = "TestUser1111",
                Email = "User1111Email@test.test",
                Password = "UserPassword1",
                PasswordConfirmation = "UserPassword1"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
            {
                Content = JsonContent.Create(loginUser)
            };
            var userLoginResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(userLoginResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void LoginUser_Negative_WrongPassword()
        {
            var loginUser = new LoginUserDto()
            {
                UserName = "TestUser1",
                Email = "User1Email@test.test",
                Password = "WrongPassword",
                PasswordConfirmation = "WrongPassword"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
            {
                Content = JsonContent.Create(loginUser)
            };
            var userLoginResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            var result = userLoginResponse.Content.ReadFromJsonAsync<UserServiceResponse<GetLoggedInUserDto>>().GetAwaiter().GetResult()!;
            Assert.That(result.Message!, Is.EqualTo("Password is incorrect."));
        }

        [Test]
        public void LoginUser_Negative_MissingUserNameAndEmail()
        {

            var loginUser = new LoginUserDto()
            {
                UserName = "",
                Email = "",
                Password = "UserPassword1",
                PasswordConfirmation = "UserPassword1"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
            {
                Content = JsonContent.Create(loginUser)
            };
            var userLoginResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(userLoginResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void LogoutUser_Positive_SuperUser()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var userLogoutResponse = _client.PostAsync("api/Auth/logout", null).GetAwaiter().GetResult();
            Assert.That(userLogoutResponse.IsSuccessStatusCode);
        }

        [Test]
        public void LogoutUser_Positive_User()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var userLogoutResponse = _client.PostAsync("api/Auth/logout", null).GetAwaiter().GetResult();
            Assert.That(userLogoutResponse.IsSuccessStatusCode);
        }

        [Test]
        public void LogoutUser_Positive_Admin()
        {

            var token = Constants.TEST_ADMIN_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var userLogoutResponse = _client.PostAsync("api/Auth/logout", null).GetAwaiter().GetResult();
            Assert.That(userLogoutResponse.IsSuccessStatusCode);
        }

        [Test]
        public void LogoutUser_Negative_NotSignedIn()
        {
            var userLogoutResponse = _client.PostAsync("api/Auth/logout", null).GetAwaiter().GetResult();
            Assert.That(userLogoutResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void UpdateUser_Positive_User()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestUser1111",
                Email = "User1111Email@test.test",
                Password = "User1111Password",
                PasswordConfirmation = "User1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync("api/Auth/updateUser?id=5", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateUser_Positive_Admin()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestAdmin1111",
                Email = "Admin1111Email@test.test",
                Password = "Admin1111Password",
                PasswordConfirmation = "Admin1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync("api/Auth/updateUser?id=3", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateUser_Positive_SuperUser()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestSuperUser1111",
                Email = "SuperUser1111Email@test.test",
                Password = "SuperUser1111Password",
                PasswordConfirmation = "SuperUser1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync("api/Auth/updateUser?id=2", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateUser_Positive_SuperUserUpdateUser()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestUser1111",
                Email = "User1111Email@test.test",
                Password = "User1111Password",
                PasswordConfirmation = "User1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync("api/Auth/updateUser?id=5", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateUser_Positive_SuperUserUpdateAdmin()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestAdmin1111",
                Email = "Admin1111Email@test.test",
                Password = "Admin1111Password",
                PasswordConfirmation = "Admin1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync("api/Auth/updateUser?id=3", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateUser_Positive_AdminUpdateUser()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestUser1111",
                Email = "User1111Email@test.test",
                Password = "User1111Password",
                PasswordConfirmation = "User1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync("api/Auth/updateUser?id=5", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateUser_Negative_AdminUpdateSuperUser()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestSuperUser1111",
                Email = "SuperUser1111Email@test.test",
                Password = "SuperUser1111Password",
                PasswordConfirmation = "SuperUser1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync("api/Auth/updateUser?id=2", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void UpdateUser_Negative_UserUpdateSuperUser()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestSuperUser1111",
                Email = "SuperUser1111Email@test.test",
                Password = "SuperUser1111Password",
                PasswordConfirmation = "SuperUser1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync("api/Auth/updateUser?id=2", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void UpdateUser_Negative_UserUpdateAdmin()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestAdmin1111",
                Email = "Admin1111Email@test.test",
                Password = "Admin1111Password",
                PasswordConfirmation = "Admin1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync("api/Auth/updateUser?id=3", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void UpdateUser_Negative_NotSignedIn()
        {
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestUser1111",
                Email = "User1Email@test.test",
                Password = "UserPassword1",
                PasswordConfirmation = "UserPassword1"
            };
            var content = JsonContent.Create(updateUser);
            var updateUserResponse = _client.PutAsync("api/Auth/updateUser?id=5", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void UpdateUser_Negative_InvalidUserName()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            var updateUser = new UpdateUserDto()
            {
                UserName = "",
                Email = "User1Email@test.test",
                Password = "UserPassword1",
                PasswordConfirmation = "UserPassword1"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync("api/Auth/updateUser?id=5", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void UpdateUser_Negative_InvalidEmail()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestUser1111",
                Email = "",
                Password = "User1111Password",
                PasswordConfirmation = "User1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync("api/Auth/updateUser?id=5", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void UpdateUser_Negative_InvalidPassword()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestUser1111",
                Email = "User1Email@test.test",
                Password = "InvalidPassword",
                PasswordConfirmation = "InvalidPassword"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync("api/Auth/updateUser?id=5", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void DeleteUser_Positive_User()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync("api/Auth/deleteUser?id=5").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteUser_Positive_Admin()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync("api/Auth/deleteUser?id=5").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteUser_Positive_SuperUser()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync("api/Auth/deleteUser?id=5").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteUser_Positive_SuperUserDeleteUser()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync("api/Auth/deleteUser?id=5").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteUser_Positive_SuperUserDeleteAdmin()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync("api/Auth/deleteUser?id=3").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteUser_Positive_AdminDeleteUser()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync("api/Auth/deleteUser?id=5").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteUser_Positive_AdminDeleteAdmin()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync("api/Auth/deleteUser?id=4").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteUser_Negative_AdminDeleteSuperUser()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync("api/Auth/deleteUser?id=2").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void DeleteUser_Negative_UserDeleteUser()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync("api/Auth/deleteUser?id=6").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void DeleteUser_Negative_UserDeleteAdmin()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync("api/Auth/deleteUser?id=3").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void DeleteUser_Negative_UserDeleteSuperUser()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync("api/Auth/deleteUser?id=2").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void RefreshToken_Positive_User()
        {

            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var refreshTokenResponse = _client.PostAsync("api/Auth/refreshToken?id=5", null).GetAwaiter().GetResult();
            Assert.That(refreshTokenResponse.IsSuccessStatusCode);
        }

        [Test]
        public void RefreshToken_Positive_Admin()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var refreshTokenResponse = _client.PostAsync("api/Auth/refreshToken?id=3", null).GetAwaiter().GetResult();
            Assert.That(refreshTokenResponse.IsSuccessStatusCode);
        }

        [Test]
        public void RefreshToken_Positive_SuperUser()
        {

            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var refreshTokenResponse = _client.PostAsync("api/Auth/refreshToken?id=2", null).GetAwaiter().GetResult();
            Assert.That(refreshTokenResponse.IsSuccessStatusCode);
        }

        [Test]
        public void RefreshToken_Negative_NotLoggedIn()
        {
            var refreshTokenResponse = _client.PostAsync($"api/Auth/refreshToken?id=5", null).GetAwaiter().GetResult();
            Assert.That(refreshTokenResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void ForgotPassword_Positive()
        {
            var forgotPasswordContent = new ForgotPasswordUserDto()
            {
                UserName = "TestUser1",
                Email = "User1Email@test.test",
            };
            var forgotPasswordRequest = new HttpRequestMessage(HttpMethod.Post, "api/Auth/forgotPassword")
            {
                Content = JsonContent.Create(forgotPasswordContent)
            };
            var forgotPasswordResponse = _client.SendAsync(forgotPasswordRequest).GetAwaiter().GetResult();
            Assert.That(forgotPasswordResponse.IsSuccessStatusCode);
        }

        [Test]
        public void ResetPasswordConfirmation_Positive()
        {
            var forgotPasswordToken = Constants.TEST_USER_FORGOT_PASSWORD_TOKEN;
            var resetPasswordConfirmationRequest = new HttpRequestMessage(HttpMethod.Post, $"api/Auth/resetPasswordConfirmation?token={forgotPasswordToken}");
            var resetPasswordResponse = _client.SendAsync(resetPasswordConfirmationRequest).GetAwaiter().GetResult();
            Assert.That(resetPasswordResponse.IsSuccessStatusCode);
        }

        [Test]
        public void ResetPassword_Positve()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var resetPasswordContent = new ResetPasswordUserDto()
            {
                Password = "Password123!!!",
                PasswordConfirmation = "Password123!!!"
            };
            var resetPasswordRequest = new HttpRequestMessage(HttpMethod.Post, $"api/Auth/resetPassword")
            {
                Content = JsonContent.Create(resetPasswordContent)
            };
            var resetPasswordResponse = _client.SendAsync(resetPasswordRequest).GetAwaiter().GetResult();
            Assert.That(resetPasswordResponse.IsSuccessStatusCode);
        }

        [Test]
        public void ForgotPassword_Positive_UserName()
        {
            var forgotPasswordContent = new ForgotPasswordUserDto()
            {
                UserName = "TestUser1",
                Email = "",
            };
            var forgotPasswordRequest = new HttpRequestMessage(HttpMethod.Post, "api/Auth/forgotPassword")
            {
                Content = JsonContent.Create(forgotPasswordContent)
            };
            var forgotPasswordResponse = _client.SendAsync(forgotPasswordRequest).GetAwaiter().GetResult();
            Assert.That(forgotPasswordResponse.IsSuccessStatusCode);
        }

        [Test]
        public void ForgotPassword_Positive_Email()
        {

            var forgotPasswordContent = new ForgotPasswordUserDto()
            {
                UserName = "",
                Email = "User1Email@test.test",
            };
            var forgotPasswordRequest = new HttpRequestMessage(HttpMethod.Post, "api/Auth/forgotPassword")
            {
                Content = JsonContent.Create(forgotPasswordContent)
            };
            var forgotPasswordResponse = _client.SendAsync(forgotPasswordRequest).GetAwaiter().GetResult();
            Assert.That(forgotPasswordResponse.IsSuccessStatusCode);
        }

        [Test]
        public void ForgotPassword_Negative_InvalidInput()
        {
            var forgotPasswordContent = new ForgotPasswordUserDto()
            {
                UserName = "",
                Email = "",
            };
            var forgotPasswordRequest = new HttpRequestMessage(HttpMethod.Post, "api/Auth/forgotPassword")
            {
                Content = JsonContent.Create(forgotPasswordContent)
            };
            var forgotPasswordResponse = _client.SendAsync(forgotPasswordRequest).GetAwaiter().GetResult();
            Assert.That(forgotPasswordResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void ResetPasswordConfirmation_Negative_FakeToken()
        {
            var resetPasswordConfirmationRequest = new HttpRequestMessage(HttpMethod.Post, $"api/Auth/resetPasswordConfirmation?token=FAKETOKENSTRING");
            var resetPasswordResponse = _client.SendAsync(resetPasswordConfirmationRequest).GetAwaiter().GetResult();
            Assert.That(resetPasswordResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void PasswordReset_Negative_NotLoggedIn()
        {
            var resetPasswordContent = new ResetPasswordUserDto()
            {
                Password = "Password123!!!",
                PasswordConfirmation = "Password123!!!"
            };
            var passwordResetResponse = _client.PostAsJsonAsync("api/Auth/resetPassword", resetPasswordContent).GetAwaiter().GetResult();
            Assert.That(passwordResetResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }
}