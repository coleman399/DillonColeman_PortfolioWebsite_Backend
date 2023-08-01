using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework.Internal;
using PortfolioWebsite_Backend.Dtos.UserDtos;
using PortfolioWebsite_Backend.Helpers.Constants;
using PortfolioWebsite_Backend.Models.UserModel;
using PortfolioWebsite_Backend_UnitTests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace PortfolioWebsite_Backend_UnitTests
{
    [TestFixture]
    public class UserService_UnitTests
    {
        private WebApplicationFactory<Program> _factory;
        private IConfiguration _configuration;
        private HttpClient _client;


        [SetUp]
        public void Setup()
        {
            _factory = DependencyInjection.FactoryProvider();
            _configuration = DependencyInjection.GetConfiguration(_factory);
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void Cleanup()
        {
            _factory.Dispose();
        }

        private string? LoginAsSuperUser()
        {
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var user = new LoginUserDto()
            {
                UserName = "TestSuperUser",
                Email = "SuperUserEmail@test.test",
                Password = "SuperUserPassword1",
                PasswordConfirmation = "SuperUserPassword1"
            };
            var userInDb = userList.FirstOrDefault(u => u.Email == user.Email)!;
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
            {
                Content = JsonContent.Create(user)
            };
            var result = _client.SendAsync(request).GetAwaiter().GetResult();
            var superUserSigninResponse = result.Content.ReadFromJsonAsync<UserServiceResponse<GetLoggedInUserDto>>().Result;
            var token = superUserSigninResponse?.Data?.Token;
            userInDb.AccessToken = token!;
            db.SaveChanges();
            return token;
        }

        private string? LoginAsAdmin()
        {
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var user = new LoginUserDto()
            {
                UserName = "TestAdmin1",
                Email = "AdminEmail@test.test",
                Password = "AdminPassword1",
                PasswordConfirmation = "AdminPassword1"
            };
            var userInDb = userList.FirstOrDefault(u => u.Email == user.Email)!;
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
            {
                Content = JsonContent.Create(user)
            };
            var result = _client.SendAsync(request).GetAwaiter().GetResult();
            var adminLoginResponse = result.Content.ReadFromJsonAsync<UserServiceResponse<GetLoggedInUserDto>>().Result;
            var token = adminLoginResponse?.Data?.Token;
            userInDb.AccessToken = token!;
            db.SaveChanges();
            return token;
        }

        private string? LoginAsUser()
        {
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var user = new LoginUserDto()
            {
                UserName = "TestUser1",
                Email = "User1Email@test.test",
                Password = "UserPassword1",
                PasswordConfirmation = "UserPassword1"
            };
            var userInDb = userList.FirstOrDefault(u => u.Email == user.Email)!;
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
            {
                Content = JsonContent.Create(user)
            };
            var result = _client.SendAsync(request).GetAwaiter().GetResult();
            var userLoginResponse = result.Content.ReadFromJsonAsync<UserServiceResponse<GetLoggedInUserDto>>().Result;
            var token = userLoginResponse?.Data?.Token;
            userInDb.AccessToken = token!;
            db.SaveChanges();
            return token;
        }

        [Test]
        public void ConfirmDefaultSuperUser()
        {

            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var defaultSuperUser = userList.FirstOrDefault(x => x.Email.Equals(_configuration["SuperUser:Email"]!));
            if (defaultSuperUser != null || defaultSuperUser!.AccessToken != null || defaultSuperUser.UserName != null || defaultSuperUser.PasswordHash != null || defaultSuperUser.Role.Equals(Roles.SuperUser.ToString()))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void GetUsers_Positive_SuperUser()
        {

            var token = LoginAsSuperUser();
            _client.SetBearerToken(token);
            var getUsersResponse = _client.GetAsync("api/Auth/getUsers").GetAwaiter().GetResult();
            Assert.That(getUsersResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetUsers_Positive_Admin()
        {

            var token = LoginAsAdmin();
            _client.SetBearerToken(token);
            var getUsersResponse = _client.GetAsync("api/Auth/getUsers").GetAwaiter().GetResult();
            Assert.That(getUsersResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetUsers_Negative_User()
        {

            var token = LoginAsUser();
            _client.SetBearerToken(token);
            var getUsersResponse = _client.GetAsync("api/Auth/getUsers").GetAwaiter().GetResult();
            Assert.That(getUsersResponse.StatusCode.Equals(HttpStatusCode.Forbidden));
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

            var token = LoginAsSuperUser();
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
            Assert.That(registerUserResponse.StatusCode.Equals(HttpStatusCode.BadRequest));
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
            Assert.That(registerUserResponse.StatusCode.Equals(HttpStatusCode.BadRequest));
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
            Assert.That(registerUserResponse.StatusCode.Equals(HttpStatusCode.BadRequest));
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
            Assert.That(registerUserResponse.StatusCode.Equals(HttpStatusCode.BadRequest));
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
            Assert.That(registerUserResponse.StatusCode.Equals(HttpStatusCode.BadRequest));
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
            Assert.That(userLoginResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
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
            Assert.That(userLoginResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
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
            Assert.That(userLoginResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void LogoutUser_Positive_SuperUser()
        {

            var token = LoginAsSuperUser();
            _client.SetBearerToken(token);
            var userLogoutResponse = _client.PostAsync("api/Auth/logout", null).GetAwaiter().GetResult();
            Assert.That(userLogoutResponse.IsSuccessStatusCode);
        }

        [Test]
        public void LogoutUser_Positive_User()
        {

            var token = LoginAsUser();
            _client.SetBearerToken(token);
            var userLogoutResponse = _client.PostAsync("api/Auth/logout", null).GetAwaiter().GetResult();
            Assert.That(userLogoutResponse.IsSuccessStatusCode);
        }

        [Test]
        public void LogoutUser_Positive_Admin()
        {

            var token = LoginAsAdmin();
            _client.SetBearerToken(token);
            var userLogoutResponse = _client.PostAsync("api/Auth/logout", null).GetAwaiter().GetResult();
            Assert.That(userLogoutResponse.IsSuccessStatusCode);
        }

        [Test]
        public void LogoutUser_Negative_NotSignedIn()
        {

            var userLogoutResponse = _client.PostAsync("api/Auth/logout", null).GetAwaiter().GetResult();
            Assert.That(userLogoutResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void UpdateUser_Positive_User()
        {

            var token = LoginAsUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToUpdate = userList.FirstOrDefault(u => u.AccessToken == token)!;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestUser1111",
                Email = "User1111Email@test.test",
                Password = "User1111Password",
                PasswordConfirmation = "User1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateUser_Positive_Admin()
        {
            var token = LoginAsAdmin();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToUpdate = userList.FirstOrDefault(u => u.AccessToken == token)!;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestAdmin1111",
                Email = "Admin1111Email@test.test",
                Password = "Admin1111Password",
                PasswordConfirmation = "Admin1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateUser_Positive_SuperUser()
        {

            var token = LoginAsSuperUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToUpdate = userList.FirstOrDefault(u => u.AccessToken == token)!;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestSuperUser1111",
                Email = "SuperUser1111Email@test.test",
                Password = "SuperUser1111Password",
                PasswordConfirmation = "SuperUser1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateUser_Positive_SuperUserUpdateUser()
        {

            var token = LoginAsSuperUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToUpdate = userList.FirstOrDefault(u => u.Role.Equals(Roles.User.ToString()))!;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestUser1111",
                Email = "User1111Email@test.test",
                Password = "User1111Password",
                PasswordConfirmation = "User1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateUser_Positive_SuperUserUpdateAdmin()
        {

            var token = LoginAsSuperUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToUpdate = userList.FirstOrDefault(u => u.Role.Equals(Roles.Admin.ToString()))!;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestAdmin1111",
                Email = "Admin1111Email@test.test",
                Password = "Admin1111Password",
                PasswordConfirmation = "Admin1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateUser_Positive_AdminUpdateUser()
        {

            var token = LoginAsAdmin();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToUpdate = userList.FirstOrDefault(u => u.Role.Equals(Roles.User.ToString()))!;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestUser1111",
                Email = "User1111Email@test.test",
                Password = "User1111Password",
                PasswordConfirmation = "User1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateUser_Negative_AdminUpdateSuperUser()
        {

            var token = LoginAsAdmin();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToUpdate = userList.FirstOrDefault(u => u.Role.Equals(Roles.SuperUser.ToString()))!;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestSuperUser1111",
                Email = "SuperUser1111Email@test.test",
                Password = "SuperUser1111Password",
                PasswordConfirmation = "SuperUser1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void UpdateUser_Negative_UserUpdateSuperUser()
        {

            var token = LoginAsUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToUpdate = userList.FirstOrDefault(u => u.Role.Equals(Roles.SuperUser.ToString()))!;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestSuperUser1111",
                Email = "SuperUser1111Email@test.test",
                Password = "SuperUser1111Password",
                PasswordConfirmation = "SuperUser1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void UpdateUser_Negative_UserUpdateAdmin()
        {

            var token = LoginAsUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToUpdate = userList.FirstOrDefault(u => u.Role.Equals(Roles.Admin.ToString()))!;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestAdmin1111",
                Email = "Admin1111Email@test.test",
                Password = "Admin1111Password",
                PasswordConfirmation = "Admin1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void UpdateUser_Negative_NotSignedIn()
        {

            var token = LoginAsUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToUpdate = userList.FirstOrDefault(u => u.AccessToken == token)!;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestUser1111",
                Email = "User1Email@test.test",
                Password = "UserPassword1",
                PasswordConfirmation = "UserPassword1"
            };
            var content = JsonContent.Create(updateUser);
            var updateUserResponse = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void UpdateUser_Negative_InvalidUserName()
        {

            var token = LoginAsUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToUpdate = userList.FirstOrDefault(u => u.AccessToken == token)!;
            var updateUser = new UpdateUserDto()
            {
                UserName = "",
                Email = "User1Email@test.test",
                Password = "UserPassword1",
                PasswordConfirmation = "UserPassword1"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.StatusCode.Equals(HttpStatusCode.BadRequest));
        }

        [Test]
        public void UpdateUser_Negative_InvalidEmail()
        {

            var token = LoginAsUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToUpdate = userList.FirstOrDefault(u => u.AccessToken == token)!;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestUser1111",
                Email = "",
                Password = "User1111Password",
                PasswordConfirmation = "User1111Password"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.StatusCode.Equals(HttpStatusCode.BadRequest));
        }

        [Test]
        public void UpdateUser_Negative_InvalidPassword()
        {

            var token = LoginAsUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToUpdate = userList.FirstOrDefault(u => u.AccessToken == token)!;
            var updateUser = new UpdateUserDto()
            {
                UserName = "TestUser1111",
                Email = "User1Email@test.test",
                Password = "InvalidPassword",
                PasswordConfirmation = "InvalidPassword"
            };
            var content = JsonContent.Create(updateUser);
            _client.SetBearerToken(token);
            var updateUserResponse = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateUserResponse.StatusCode.Equals(HttpStatusCode.BadRequest));
        }

        [Test]
        public void DeleteUser_Positive_User()
        {

            var token = LoginAsUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToDelete = userList.FirstOrDefault(u => u.AccessToken == token)!;
            _client.SetBearerToken(userToDelete?.AccessToken);
            var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id={userToDelete?.Id}").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteUser_Positive_Admin()
        {

            var token = LoginAsAdmin();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToDelete = userList.FirstOrDefault(u => u.AccessToken == token)!;
            _client.SetBearerToken(userToDelete?.AccessToken);
            var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id={userToDelete?.Id}").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteUser_Positive_SuperUser()
        {

            var token = LoginAsSuperUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToDelete = userList.FirstOrDefault(u => u.AccessToken == token)!;
            _client.SetBearerToken(userToDelete?.AccessToken);
            var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id={userToDelete?.Id}").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteUser_Positive_SuperUserDeleteUser()
        {
            var token = LoginAsSuperUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToDelete = userList.FirstOrDefault(u => u.Role.Equals(Roles.User.ToString()))!;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id={userToDelete.Id}").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteUser_Positive_SuperUserDeleteAdmin()
        {
            var token = LoginAsSuperUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToDelete = userList.FirstOrDefault(u => u.Role.Equals(Roles.Admin.ToString()))!;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id={userToDelete.Id}").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteUser_Negative_SuperUserDeleteDefaultSuperUser()
        {
            var token = LoginAsSuperUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToDelete = userList.FirstOrDefault(u => u.Role.Equals(Roles.SuperUser.ToString()) && u.Email == _configuration["SuperUser:Email"])!;
            _client.SetBearerToken(userToDelete?.AccessToken);
            var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id={userToDelete?.Id}").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void DeleteUser_Positive_AdminDeleteUser()
        {

            var token = LoginAsAdmin();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToDelete = userList.FirstOrDefault(u => u.Role.Equals(Roles.User.ToString()))!;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id={userToDelete.Id}").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteUser_Positive_AdminDeleteAdmin()
        {

            var token = LoginAsAdmin();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToDelete = userList.FirstOrDefault(u => u.Role.Equals(Roles.Admin.ToString()) && u.AccessToken != token)!;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id={userToDelete.Id}").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteUser_Negative_AdminDeleteSuperUser()
        {
            var token = LoginAsAdmin();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToDelete = userList.FirstOrDefault(u => u.Role.Equals(Roles.SuperUser.ToString()) && u.AccessToken != token)!;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id={userToDelete.Id}").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void DeleteUser_Negative_UserDeleteUser()
        {

            var token = LoginAsUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToDelete = userList.FirstOrDefault(u => u.Role.Equals(Roles.User.ToString()) && u.AccessToken != token)!;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id={userToDelete.Id}").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void DeleteUser_Negative_UserDeleteAdmin()
        {

            var token = LoginAsUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToDelete = userList.FirstOrDefault(u => u.Role.Equals(Roles.Admin.ToString()))!;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id={userToDelete.Id}").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void DeleteUser_Negative_UserDeleteSuperUser()
        {

            var token = LoginAsUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToDelete = userList.FirstOrDefault(u => u.Role.Equals(Roles.SuperUser.ToString()))!;
            _client.SetBearerToken(token);
            var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id={userToDelete?.Id}").GetAwaiter().GetResult();
            Assert.That(deleteUserResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void RefreshToken_Positive_User()
        {

            var token = LoginAsUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToRefresh = userList.FirstOrDefault(u => u.AccessToken == token)!;
            _client.SetBearerToken(token);
            var refreshTokenResponse = _client.PostAsync($"api/Auth/refreshToken?id={userToRefresh.Id}", null).GetAwaiter().GetResult();
            Assert.That(refreshTokenResponse.IsSuccessStatusCode);
        }

        [Test]
        public void RefreshToken_Positive_Admin()
        {

            var token = LoginAsAdmin();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToRefresh = userList.FirstOrDefault(u => u.AccessToken == token)!;
            _client.SetBearerToken(token);
            var refreshTokenResponse = _client.PostAsync($"api/Auth/refreshToken?id={userToRefresh.Id}", null).GetAwaiter().GetResult();
            Assert.That(refreshTokenResponse.IsSuccessStatusCode);
        }

        [Test]
        public void RefreshToken_Positive_SuperUser()
        {

            var token = LoginAsSuperUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToRefresh = userList.FirstOrDefault(u => u.AccessToken == token)!;
            _client.SetBearerToken(token);
            var refreshTokenResponse = _client.PostAsync($"api/Auth/refreshToken?id={userToRefresh.Id}", null).GetAwaiter().GetResult();
            Assert.That(refreshTokenResponse.IsSuccessStatusCode);
        }

        [Test]
        public void RefreshToken_Negative_NotLoggedIn()
        {
            var token = LoginAsSuperUser();
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var userToRefresh = userList.FirstOrDefault(u => u.AccessToken == token)!;
            var refreshTokenResponse = _client.PostAsync($"api/Auth/refreshToken?id={userToRefresh.Id}", null).GetAwaiter().GetResult();
            Assert.That(refreshTokenResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
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
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var user = userList.FirstOrDefault(x => x.UserName.Equals(forgotPasswordContent.UserName))!;
            var forgotPasswordToken = user.ForgotPasswordToken?.Token;
            var resetPasswordConfirmationRequest = new HttpRequestMessage(HttpMethod.Post, $"api/Auth/resetPasswordConfirmation?token={forgotPasswordToken}");
            var resetPasswordResponse = _client.SendAsync(resetPasswordConfirmationRequest).GetAwaiter().GetResult();
            Assert.That(resetPasswordResponse.IsSuccessStatusCode);
            var responseJson = resetPasswordResponse.Content.ReadFromJsonAsync<UserServiceResponse<GetResetPasswordUserDto>>().GetAwaiter().GetResult()!;
            var resetPasswordToken = responseJson.Data!.Token;
            var resetPasswordContent = new ResetPasswordUserDto()
            {
                Password = "Password123!!!",
                PasswordConfirmation = "Password123!!!"
            };
            _client.SetBearerToken(resetPasswordToken);
            var resetPasswordRequest = new HttpRequestMessage(HttpMethod.Post, "api/Auth/resetPassword")
            {
                Content = JsonContent.Create(resetPasswordContent)
            };
            var passwordResetResponse = _client.PostAsJsonAsync("api/Auth/resetPassword", resetPasswordContent).GetAwaiter().GetResult();
            Assert.That(passwordResetResponse.IsSuccessStatusCode);
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
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var user = userList.FirstOrDefault(x => x.UserName.Equals(forgotPasswordContent.UserName));
            var forgotPasswordToken = user?.ForgotPasswordToken?.Token;
            var resetPasswordConfirmationRequest = new HttpRequestMessage(HttpMethod.Post, $"api/Auth/resetPasswordConfirmation?token={forgotPasswordToken}");
            var resetPasswordResponse = _client.SendAsync(resetPasswordConfirmationRequest).GetAwaiter().GetResult();
            Assert.That(resetPasswordResponse.IsSuccessStatusCode);
            var responseJson = resetPasswordResponse.Content.ReadFromJsonAsync<UserServiceResponse<GetResetPasswordUserDto>>().GetAwaiter().GetResult()!;
            var resetPasswordToken = responseJson.Data!.Token;
            var resetPasswordContent = new ResetPasswordUserDto()
            {
                Password = "Password123!!!",
                PasswordConfirmation = "Password123!!!"
            };
            _client.SetBearerToken(resetPasswordToken);
            var resetPasswordRequest = new HttpRequestMessage(HttpMethod.Post, "api/Auth/resetPassword")
            {
                Content = JsonContent.Create(resetPasswordContent)
            };
            var passwordResetResponse = _client.PostAsJsonAsync("api/Auth/resetPassword", resetPasswordContent).GetAwaiter().GetResult();
            Assert.That(passwordResetResponse.IsSuccessStatusCode);

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
            var serviceContainer = _factory.Services.CreateScope();
            var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
            var userList = db.Users.ToList();
            var user = userList.FirstOrDefault(x => x.Email.Equals(forgotPasswordContent.Email));
            var forgotPasswordToken = user?.ForgotPasswordToken?.Token;
            var resetPasswordConfirmationRequest = new HttpRequestMessage(HttpMethod.Post, $"api/Auth/resetPasswordConfirmation?token={forgotPasswordToken}");
            var resetPasswordResponse = _client.SendAsync(resetPasswordConfirmationRequest).GetAwaiter().GetResult();
            Assert.That(resetPasswordResponse.IsSuccessStatusCode);
            var responseJson = resetPasswordResponse.Content.ReadFromJsonAsync<UserServiceResponse<GetResetPasswordUserDto>>().GetAwaiter().GetResult()!;
            var resetPasswordToken = responseJson.Data!.Token;
            var resetPasswordContent = new ResetPasswordUserDto()
            {
                Password = "Password123!!!",
                PasswordConfirmation = "Password123!!!"
            };
            _client.SetBearerToken(resetPasswordToken);
            var resetPasswordRequest = new HttpRequestMessage(HttpMethod.Post, "api/Auth/resetPassword")
            {
                Content = JsonContent.Create(resetPasswordContent)
            };
            var passwordResetResponse = _client.PostAsJsonAsync("api/Auth/resetPassword", resetPasswordContent).GetAwaiter().GetResult();
            Assert.That(passwordResetResponse.IsSuccessStatusCode);
        }

        [Test]
        public void ForgotPassword_Negative_NoInput_FakeToken_TokenUnconfirmed()
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
            Assert.That(forgotPasswordResponse.StatusCode.Equals(HttpStatusCode.BadRequest));
            var resetPasswordConfirmationRequest = new HttpRequestMessage(HttpMethod.Post, $"api/Auth/resetPasswordConfirmation?token=FAKETOKENSTRING");
            var resetPasswordResponse = _client.SendAsync(resetPasswordConfirmationRequest).GetAwaiter().GetResult();
            Assert.That(resetPasswordResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
            var resetPasswordContent = new ResetPasswordUserDto()
            {
                Password = "Password123!!!",
                PasswordConfirmation = "Password123!!!"
            };
            var passwordResetResponse = _client.PostAsJsonAsync("api/Auth/resetPassword", resetPasswordContent).GetAwaiter().GetResult();
            Assert.That(passwordResetResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }
    }
}