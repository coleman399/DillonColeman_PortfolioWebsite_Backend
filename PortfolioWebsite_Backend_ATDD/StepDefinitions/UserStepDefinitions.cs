using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PortfolioWebsite_Backend.Dtos.UserDtos;
using PortfolioWebsite_Backend.Helpers.Constants;
using PortfolioWebsite_Backend.Models.UserModel;
using System.Net;
using System.Net.Http.Json;

namespace PortfolioWebsite_Backend_Testing.StepDefinitions
{
    [Binding]
    public class UserStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public UserStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _factory = FactoryReset();
            _client = _factory.CreateClient();
        }

        private static WebApplicationFactory<Program> FactoryReset()
        {
            var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UserContext>));
                    services.Remove(dbContextDescriptor!);

                    services.AddDbContext<UserContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestingDB");
                    });

                    // Seeding database
                    var serviceProvider = services.BuildServiceProvider();
                    using var scope = serviceProvider.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<UserContext>();
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    try
                    {
                        SeedData(context);
                        context.SaveChanges();
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Error in seeding user data", exception);
                    }
                });
            });
            return factory;
        }

        private static void SeedData(UserContext context)
        {
            /*  * Retrieve data from the database *
                var responseContent = _factory.Services.CreateScope();
                var db = responseContent.ServiceProvider.GetRequiredService<UserContext>();
                var userList = db.Users.ToList(); 
            */

            context.Users.Add(new User()
            {
                UserName = "TestSuperUser",
                Email = "SuperUserEmail@test.test",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperUserPassword1"),
                Role = Roles.SuperUser.ToString(),
                AccessToken = string.Empty,
            });
            context.Users.Add(new User()
            {
                UserName = "TestAdmin",
                Email = "AdminEmail@test.test",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword1"),
                Role = Roles.Admin.ToString(),
                AccessToken = string.Empty,
            });
            context.Users.Add(new User()
            {
                UserName = "TestUser1",
                Email = "User1Email@test.test",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("UserPassword1"),
                Role = Roles.User.ToString(),
                AccessToken = string.Empty,
            });
            context.Users.Add(new User()
            {
                UserName = "TestUser2",
                Email = "User2Email@test.test",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("UserPassword2"),
                Role = Roles.User.ToString(),
                AccessToken = string.Empty,
            });
            context.Users.Add(new User()
            {
                UserName = "TestUser3",
                Email = "User3Email@test.test",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("UserPassword3"),
                Role = Roles.User.ToString(),
                AccessToken = string.Empty,
            });
        }

        [TearDown]
        public void Cleanup()
        {
            _factory.Dispose();
            _factory = FactoryReset();
            _client = _factory.CreateClient();
        }

        #region Register User
        [Given(@"That I provide valid information to register for a user account:")]
        public void GivenThatIProvideValidInformationToRegisterForAUserAccount(Table table)
        {
            try
            {
                var registerUser = new RegisterUserDto()
                {
                    UserName = table.Rows[0]["UserName"],
                    Email = table.Rows[0]["Email"],
                    Role = table.Rows[0]["Role"],
                    Password = table.Rows[0]["Password"],
                    PasswordConfirmation = table.Rows[0]["PasswordConfirmation"]
                };
                _scenarioContext.Add("registerUser", registerUser);
                _scenarioContext.Get<RegisterUserDto>("registerUser").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in GivenThatIProvideValidInformationToRegisterForAUserAccount", exception);
            }
        }

        [When(@"submitting a register user form")]
        public void WhenSubmittingARegisterUserForm()
        {
            try
            {
                var user = _scenarioContext.Get<RegisterUserDto>("registerUser");
                var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/register")
                {
                    Content = JsonContent.Create(user)
                };
                var registerUserResponse = _client.SendAsync(request).GetAwaiter().GetResult();
                _scenarioContext.Add("registerUserResponse", registerUserResponse);
                _scenarioContext.Get<HttpResponseMessage>("registerUserResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenSubmittingARegisterUserForm ", exception);
            }
        }

        [Then(@"my account is created")]
        public void ThenMyAccountIsCreated()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("registerUserResponse");
                result.StatusCode.Should().Be(HttpStatusCode.Created);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenMyAccountIsCreated", exception);
            }
        }
        #endregion

        #region Login User
        [Given(@"That I provide valid information to login to my user account:")]
        public void GivenThatIProvideValidInformationToLoginToMyUserAccount(Table table)
        {
            try
            {
                var loginUser = new LoginUserDto()
                {
                    UserName = table.Rows[0]["UserName"],
                    Email = table.Rows[0]["Email"],
                    Password = table.Rows[0]["Password"],
                    PasswordConfirmation = table.Rows[0]["PasswordConfirmation"]
                };
                _scenarioContext.Add("loginUser", loginUser);
                _scenarioContext.Get<LoginUserDto>("loginUser").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in GivenThatIProvideValidInformationToRegisterForAUserAccount", exception);
            }
        }

        [When(@"submitting a login user form")]
        public void WhenSubmittingALoginUserForm()
        {
            try
            {
                var user = _scenarioContext.Get<LoginUserDto>("loginUser");
                var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
                {
                    Content = JsonContent.Create(user)
                };
                var userLoginResponse = _client.SendAsync(request).GetAwaiter().GetResult();
                _scenarioContext.Add("userLoginResponse", userLoginResponse);
                _scenarioContext.Get<HttpResponseMessage>("userLoginResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenSubmittingARegisterUserForm ", exception);
            }
        }

        [Then(@"I should be logged in")]
        public void ThenIShouldBeLoggedIn()
        {
            var result = _scenarioContext.Get<HttpResponseMessage>("userLoginResponse");
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        #endregion

        #region Get All Users
        [Given(@"I am logged in as a SuperUser")]
        public void GivenIAmLoggedInAsASuperUser()
        {
            try
            {
                var user = new LoginUserDto()
                {
                    UserName = "TestSuperUser",
                    Email = "SuperUserEmail@test.test",
                    Password = "SuperUserPassword1",
                    PasswordConfirmation = "SuperUserPassword1"
                };
                var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
                {
                    Content = JsonContent.Create(user)
                };
                var result = _client.SendAsync(request).GetAwaiter().GetResult();
                var superUserSigninResponse = result.Content.ReadFromJsonAsync<UserServiceResponse<GetLoggedInUserDto>>().Result;
                _scenarioContext.Add("user", user);
                _scenarioContext.Add("superUserLoginResponse", superUserSigninResponse);
                _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("superUserLoginResponse").Data!.Token.Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in GivenIAmLoggedInAsASuperUser", exception);
            }
        }

        [When(@"I request all users")]
        public void WhenIRequestAllUsers()
        {
            try
            {
                var response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("superUserLoginResponse");
                _client.SetBearerToken(response.Data!.Token);
                var getUsersResponse = _client.GetAsync("api/Auth/getUsers").GetAwaiter().GetResult();
                _scenarioContext.Add("getUsersResponse", getUsersResponse);
                _scenarioContext.Get<HttpResponseMessage>("getUsersResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestAllUsers", exception);
            }
        }

        [Then(@"I should get all users")]
        public void ThenIShouldGetAllUsers()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("getUsersResponse");
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenIShouldGetAllUsers", exception);
            }
        }
        #endregion

        #region Logout User
        [Given(@"I am logged in as a User")]
        public void GivenIAmLoggedInAsAUser()
        {
            try
            {
                var user = new LoginUserDto()
                {
                    UserName = "TestUser1",
                    Email = "User1Email@test.test",
                    Password = "UserPassword1",
                    PasswordConfirmation = "UserPassword1"
                };
                var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
                {
                    Content = JsonContent.Create(user)
                };
                var result = _client.SendAsync(request).GetAwaiter().GetResult();
                var userLoginResponse = result.Content.ReadFromJsonAsync<UserServiceResponse<GetLoggedInUserDto>>().Result;
                _scenarioContext.Add("user", user);
                _scenarioContext.Add("userLoginResponse", userLoginResponse);
                _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse").Data!.Token.Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in GivenIAmLoggedInAsAUser", exception);
            }
        }

        [When(@"I request to log out")]
        public void WhenIRequestToLogOut()
        {
            try
            {
                var response = new UserServiceResponse<GetLoggedInUserDto>();
                if (_scenarioContext.ContainsKey("userLoginResponse"))
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse");
                }
                else if (_scenarioContext.ContainsKey("adminLoginResponse"))
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("adminLoginResponse");
                }
                else if (_scenarioContext.ContainsKey("superUserLoginResponse"))
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("superUserLoginResponse");
                }
                _client.SetBearerToken(response.Data!.Token);
                var userLogoutResponse = _client.PostAsync("api/Auth/logout", null).GetAwaiter().GetResult();
                _scenarioContext.Add("userLogoutResponse", userLogoutResponse);
                _scenarioContext.Get<HttpResponseMessage>("userLogoutResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToLogOut", exception);
            }
        }

        [Then(@"I should be logged out")]
        public void ThenIShouldBeLoggedOut()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("userLogoutResponse");
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenIShouldBeLoggedOut", exception);
            }
        }

        [Given(@"I am logged in as a Admin")]
        public void GivenIAmLoggedInAsAAdmin()
        {
            try
            {
                var user = new LoginUserDto()
                {
                    UserName = "TestAdmin",
                    Email = "AdminEmail@test.test",
                    Password = "AdminPassword1",
                    PasswordConfirmation = "AdminPassword1"
                };
                var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
                {
                    Content = JsonContent.Create(user)
                };
                var result = _client.SendAsync(request).GetAwaiter().GetResult();
                var adminLoginResponse = result.Content.ReadFromJsonAsync<UserServiceResponse<GetLoggedInUserDto>>().Result;
                _scenarioContext.Add("user", user);
                _scenarioContext.Add("adminLoginResponse", adminLoginResponse);
                _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("adminLoginResponse").Data!.Token.Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in GivenIAmLoggedInAsAAdmin", exception);
            }
        }
        #endregion

        #region Update User
        [When(@"I request to update my account information")]
        public void WhenIRequestToUpdateMyAccountInformation()
        {
            try
            {
                var userLoginResponse = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse");
                var serviceContainer = _factory.Services.CreateScope();
                var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
                var userList = db.Users.ToList();
                var userToUpdate = userList.FirstOrDefault(u => u.Email.Equals("User1Email@test.test"))!;
                var updateUser = new UpdateUserDto()
                {
                    UserName = "TestUser1111",
                    Email = "User1Email@test.test",
                    Password = "UserPassword1",
                    PasswordConfirmation = "UserPassword1"
                };
                var content = JsonContent.Create(updateUser);
                _client.SetBearerToken(userToUpdate.AccessToken);
                var updateUserResponse = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate?.Id}", content).GetAwaiter().GetResult();
                _scenarioContext.Add("updateUserResponse", updateUserResponse);
                _scenarioContext.Get<HttpResponseMessage>("updateUserResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToUpdateMyAccountInformation", exception);
            }
        }

        [Then(@"my account information should be updated")]
        public void ThenMyAccountInformationShouldBeUpdated()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("updateUserResponse");
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenMyAccountInformationShouldBeUpdated", exception);
            }
        }

        [Then(@"if I attempt to update another user's account information")]
        public void ThenIfIAttemptToUpdateAnotherUsersAccountInformation()
        {
            try
            {
                var user = _scenarioContext.Get<LoginUserDto>("user");
                var loginResponse = _client.PostAsync("api/Auth/login", JsonContent.Create(user)).Result;
                var loginContent = loginResponse.Content.ReadFromJsonAsync<UserServiceResponse<GetLoggedInUserDto>>().Result;
                _client.SetBearerToken(loginContent!.Data!.Token);
                var serviceContainer = _factory.Services.CreateScope();
                var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
                var userList = db.Users.ToList();
                var userToUpdate = userList.FirstOrDefault(u => u.AccessToken != loginContent.Data?.Token && u.Role == Roles.User.ToString() && u.Email != user.Email);
                var updateUser = new UpdateUserDto()
                {
                    UserName = "TestUser1111",
                    Email = userToUpdate?.Email!,
                    Password = user.Password,
                    PasswordConfirmation = user.PasswordConfirmation
                };
                var content = JsonContent.Create(updateUser);
                var updateAnotherUserResult = _client.PutAsync($"api/Auth/updateUser?id={userToUpdate?.Id}", content).GetAwaiter().GetResult();
                _scenarioContext.Add("updateAnotherUserResult", updateAnotherUserResult);
                _scenarioContext.Get<HttpResponseMessage>("updateAnotherUserResult").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenIfIAttemptToUpdateAnotherUsersAccountInformation", exception);
            }
        }

        [Then(@"I should be denied")]
        public void ThenIShouldBeDenied()
        {
            try
            {
                HttpResponseMessage? response;
                if (_scenarioContext.ContainsKey("updateAnotherUserResult"))
                {
                    response = _scenarioContext.Get<HttpResponseMessage>("updateAnotherUserResult");
                }
                else if (_scenarioContext.ContainsKey("deleteAnotherUserResult"))
                {
                    response = _scenarioContext.Get<HttpResponseMessage>("deleteAnotherUserResult");
                }
                else if (_scenarioContext.ContainsKey("updateAnotherUsersContactResponse"))
                {
                    response = _scenarioContext.Get<HttpResponseMessage>("updateAnotherUsersContactResponse");
                }
                else if (_scenarioContext.ContainsKey("deleteAnotherUsersContactResponse"))
                {
                    response = _scenarioContext.Get<HttpResponseMessage>("deleteAnotherUsersContactResponse");
                }
                else if (_scenarioContext.ContainsKey("getAnotherUsersContactWithIdResponse"))
                {
                    response = _scenarioContext.Get<HttpResponseMessage>("getAnotherUsersContactWithIdResponse");
                }
                else
                {
                    response = _scenarioContext.Get<HttpResponseMessage>("getAnotherUsersContactWithEmailResponse");
                }
                response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenIShouldBeDenied", exception);
            }
        }

        [When(@"I request to update an account")]
        public void WhenIRequestToUpdateAnAccount()
        {
            try
            {
                var user = _scenarioContext.Get<LoginUserDto>("user");
                var updateUser = new UpdateUserDto()
                {
                    UserName = "TestUser1Updated",
                    Email = user.Email!,
                    Password = user.Password,
                    PasswordConfirmation = user.PasswordConfirmation
                };
                var content = JsonContent.Create(updateUser);
                var response = new UserServiceResponse<GetLoggedInUserDto>();
                if (_scenarioContext.ContainsKey("superUserLoginResponse"))
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("superUserLoginResponse");
                }
                else if (_scenarioContext.ContainsKey("adminLoginResponse"))
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("adminLoginResponse");
                }
                _client.SetBearerToken(response.Data!.Token);
                var updateUserResponse = _client.PutAsJsonAsync("api/Auth/updateUser?id=4", content).GetAwaiter().GetResult();
                _scenarioContext.Add("updateUserResponse", updateUserResponse);
                _scenarioContext.Get<HttpResponseMessage>("updateUserResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToUpdateAnAccount", exception);
            }
        }

        [Then(@"the account should be updated")]
        public void ThenTheAccountShouldBeUpdated()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("updateUserResponse");
                result.Content.ReadFromJsonAsync<UserServiceResponse<GetLoggedInUserDto>>().Result!.Success.Should().BeTrue();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenTheAccountShouldBeUpdated", exception);
            }
        }
        #endregion

        #region Delete User
        [When(@"I request to delete my account")]
        public void WhenIRequestToDeleteMyAccount()
        {
            try
            {
                var login = new LoginUserDto()
                {
                    UserName = "TestUser1",
                    Email = "User1Email@test.test",
                    Password = "UserPassword1",
                    PasswordConfirmation = "UserPassword1"
                };

                var user2 = _client.PostAsync("api/Auth/login", JsonContent.Create(login)).Result;
                var content = user2.Content.ReadFromJsonAsync<UserServiceResponse<GetLoggedInUserDto>>().Result;
                var serviceContainer = _factory.Services.CreateScope();
                var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
                var userList = db.Users.ToList();
                var userToDelete = userList.FirstOrDefault(user => user.AccessToken == content?.Data?.Token);
                _client.SetBearerToken(content?.Data?.Token);
                var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id={userToDelete?.Id}").GetAwaiter().GetResult();
                _scenarioContext.Add("deleteUserResponse", deleteUserResponse);
                _scenarioContext.Get<HttpResponseMessage>("deleteUserResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToDeleteMyAccount", exception);
            }
        }

        [Then(@"my account should be deleted")]
        public void ThenMyAccountShouldBeDeleted()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("deleteUserResponse");
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenMyAccountShouldBeDeleted", exception);
            }
        }

        [Then(@"if I attempt to delete another user's account")]
        public void ThenIfIAttemptToDeleteAnotherUsersAccount()
        {
            try
            {
                var loginResponse = _client.PostAsync("api/Auth/login", JsonContent.Create(new LoginUserDto()
                {
                    UserName = "TestUser2",
                    Password = "UserPassword2",
                    PasswordConfirmation = "UserPassword2"
                })).GetAwaiter().GetResult();
                var response = loginResponse.Content.ReadFromJsonAsync<UserServiceResponse<GetLoggedInUserDto>>().Result;
                _client.SetBearerToken(response!.Data!.Token);
                var deleteAnotherUserResult = _client.DeleteAsync("api/Auth/deleteUser?id=6").GetAwaiter().GetResult();
                _scenarioContext.Add("deleteAnotherUserResult", deleteAnotherUserResult);
                _scenarioContext.Get<HttpResponseMessage>("deleteAnotherUserResult").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenIfIAttemptToDeleteAnotherUsersAccount", exception);
            }
        }

        [When(@"I request to delete an account")]
        public void WhenIRequestToDeleteAnAccount()
        {
            try
            {
                var test = _factory.Services.CreateScope();
                var db = test.ServiceProvider.GetRequiredService<UserContext>();
                var userList = db.Users.ToList();
                var id = userList.FirstOrDefault(x => x.Role.Equals(Roles.User.ToString()))!.Id;


                if (_scenarioContext.ContainsKey("adminLoginResponse"))
                {
                    var response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("adminLoginResponse");
                    _client.SetBearerToken(response.Data!.Token);
                }
                else
                {
                    var response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("superUserLoginResponse");
                    _client.SetBearerToken(response.Data!.Token);
                }
                var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id={id}").GetAwaiter().GetResult();
                _scenarioContext.Add("deleteUserResponse", deleteUserResponse);
                _scenarioContext.Get<HttpResponseMessage>("deleteUserResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToDeleteAnAccount", exception);
            }
        }

        [Then(@"the account should be deleted")]
        public void ThenTheAccountShouldBeDeleted()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("deleteUserResponse");
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenTheAccountShouldBeDeleted", exception);
            }
        }
        #endregion

        #region Refresh Token
        [When(@"I request to refresh my token")]
        public void WhenIRequestToRefreshMyToken()
        {
            UserServiceResponse<GetLoggedInUserDto> response;
            if (_scenarioContext.ContainsKey("userLoginResponse"))
            {
                response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse");
            }
            else if (_scenarioContext.ContainsKey("adminLoginResponse"))
            {
                response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("adminLoginResponse");
            }
            else
            {
                response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("superUserLoginResponse");
            }
            _client.SetBearerToken(response.Data?.Token);
            var refreshTokenResponse = _client.PostAsync($"api/Auth/refreshToken", null).GetAwaiter().GetResult();
            _scenarioContext.Add("refreshTokenResponse", refreshTokenResponse);
            _scenarioContext.Get<HttpResponseMessage>("refreshTokenResponse").Should().NotBeNull();
        }

        [Then(@"my token should be refreshed")]
        public void ThenMyTokenShouldBeRefreshed()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("refreshTokenResponse");
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenMyTokenShouldBeRefreshed", exception);
            }
        }
        #endregion

        #region Forgot Password
        [Given(@"that I provide valid information to reset my password:")]
        public void GivenThatIProvideValidInformationToResetMyPassword(Table table)
        {
            try
            {
                var forgotPasswordContent = new ForgotPasswordUserDto()
                {
                    UserName = table.Rows[0]["UserName"],
                    Email = table.Rows[0]["Email"],
                };
                _scenarioContext.Add("forgotPasswordContent", forgotPasswordContent);
                _scenarioContext.Get<ForgotPasswordUserDto>("forgotPasswordContent").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in GivenIThatIProvideValidInformationToResetMyPassword", exception);
            }
        }

        [When(@"I send a forgot password request")]
        public void WhenISendAForgotPasswordRequest()
        {
            try
            {
                var forgotPasswordContent = _scenarioContext.Get<ForgotPasswordUserDto>("forgotPasswordContent");
                var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/forgotPassword")
                {
                    Content = JsonContent.Create(forgotPasswordContent)
                };
                var forgotPasswordResponse = _client.SendAsync(request).GetAwaiter().GetResult();
                _scenarioContext.Add("forgotPasswordResponse", forgotPasswordResponse);
                _scenarioContext.Get<HttpResponseMessage>("forgotPasswordResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenISendAForgotPasswordRequest", exception);
            }
        }

        [Then(@"I should receive an email with a link to reset my password")]
        public void ThenIShouldReceiveAnEmailWithALinkToResetMyPassword()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("forgotPasswordResponse");
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenIShouldReceiveAnEmailWithALinkToResetMyPassword", exception);
            }
        }

        [When(@"I click the link")]
        public void WhenIClickTheLink()
        {
            try
            {
                var forgotPasswordContent = _scenarioContext.Get<ForgotPasswordUserDto>("forgotPasswordContent");
                var serviceContainer = _factory.Services.CreateScope();
                var db = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
                var userList = db.Users.ToList();
                var user = forgotPasswordContent.Email.IsNullOrEmpty() ? userList.FirstOrDefault(x => x.UserName.Equals(forgotPasswordContent.UserName)) : userList.FirstOrDefault(x => x.Email.Equals(forgotPasswordContent.Email));
                var token = user?.ForgotPasswordToken?.Token;
                var request = new HttpRequestMessage(HttpMethod.Post, $"api/Auth/resetPasswordConfirmation?token={token}");
                var resetPasswordResponse = _client.SendAsync(request).GetAwaiter().GetResult();
                _scenarioContext.Add("resetPasswordResponse", resetPasswordResponse);
                _scenarioContext.Get<HttpResponseMessage>("resetPasswordResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIClickTheLink", exception);
            }
        }

        [Then(@"my reset password request should be validated")]
        public void ThenMyResetPasswordRequestShouldBeValidated()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("resetPasswordResponse");
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenMyResetPasswordRequestShouldBeValidated", exception);
            }
        }

        [Then(@"I should be able to reset my password")]
        public void ThenIShouldBeAbleToResetMyPassword()
        {
            try
            {
                var resetPasswordResponse = _scenarioContext.Get<HttpResponseMessage>("resetPasswordResponse");
                var resetPasswordResponseAsString = resetPasswordResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var resetPasswordResponseAsJson = JObject.Parse(resetPasswordResponseAsString);
                var token = resetPasswordResponseAsJson["data"]?["token"]?.ToString();
                var resetPasswordContent = new ResetPasswordUserDto()
                {
                    Password = "Password123!!!",
                    PasswordConfirmation = "Password123!!!"
                };
                _client.SetBearerToken(token);
                var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/resetPassword");
                var passwordResetResponse = _client.PostAsJsonAsync("api/Auth/resetPassword", resetPasswordContent).GetAwaiter().GetResult();
                passwordResetResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIClickTheLink", exception);
            }
        }
        #endregion
    }
}
