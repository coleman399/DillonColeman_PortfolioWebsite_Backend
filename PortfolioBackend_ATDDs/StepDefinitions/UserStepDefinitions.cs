using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using PortfolioBackend.Dtos.UserDtos;
using PortfolioBackend.Helpers;
using PortfolioBackend.Models.UserModel;
using System.Net;
using System.Net.Http.Json;


namespace PortfolioBackend_ATDDs.StepDefinitions
{
    [Binding]
    public class UserStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public UserStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void Cleanup()
        {
            _factory.Dispose();
            _client.Dispose();
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
                var superUser = new LoginUserDto()
                {
                    UserName = "TestSuperUser",
                    Email = "SuperUserEmail@test.test",
                    Password = "SuperUserPassword1",
                    PasswordConfirmation = "SuperUserPassword1"
                };
                var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
                {
                    Content = JsonContent.Create(superUser)
                };
                var result = _client.SendAsync(request).GetAwaiter().GetResult();
                result.StatusCode.Should().Be(HttpStatusCode.OK);
                _scenarioContext.Add("superUser", superUser);
                _scenarioContext.Add("SuperUserAccessToken", "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIyIiwicm9sZSI6IlN1cGVyVXNlciIsImVtYWlsIjoiU3VwZXJVc2VyRW1haWxAdGVzdC50ZXN0IiwidW5pcXVlX25hbWUiOiJUZXN0U3VwZXJVc2VyIiwibmJmIjoxNjkxMzYzOTMyLCJleHAiOjE4NDkyMTY3MzIsImlhdCI6MTY5MTM2MzkzMn0.OZQ7l48iKSqC4ptRmkk8ie4jojDuze7BJ8ZvQzOmYCu9J3EzkNHUAjyyfdDY6wNyxp6Z_88-igoqG8MqA5Pf-g");
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
                string token;
                if (_scenarioContext.ContainsKey("SuperUserAccessToken"))
                {
                    token = _scenarioContext.Get<string>("SuperUserAccessToken");
                }
                else
                {
                    token = _scenarioContext.Get<string>("AdminAccessToken");
                }
                _client.SetBearerToken(token);
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
                result.StatusCode.Should().Be(HttpStatusCode.OK);
                _scenarioContext.Add("UserAccessToken", "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI1Iiwicm9sZSI6IlVzZXIiLCJlbWFpbCI6IlVzZXIxRW1haWxAdGVzdC50ZXN0IiwidW5pcXVlX25hbWUiOiJUZXN0VXNlcjEiLCJuYmYiOjE2OTEzNjk0NzAsImV4cCI6MTg0OTIyMjI3MCwiaWF0IjoxNjkxMzY5NDcwfQ.t415ny5NRTFulZjZJLjv1V8PTxNOTK0spczU_592C41v9YPztb6OxrWFF-l2bCGNj2TtbbCwArwVE7WQ32_vOw");
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
                string token;
                if (_scenarioContext.ContainsKey("SuperUserAccessToken"))
                {
                    token = _scenarioContext.Get<string>("SuperUserAccessToken");
                }
                else if (_scenarioContext.ContainsKey("AdminAccessToken"))
                {
                    token = _scenarioContext.Get<string>("AdminAccessToken");
                }
                else
                {
                    token = _scenarioContext.Get<string>("UserAccessToken");
                }
                _client.SetBearerToken(token);
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
                var admin = new LoginUserDto()
                {
                    UserName = "TestAdmin1",
                    Email = "TestAdmin1@test.test",
                    Password = "AdminPassword1",
                    PasswordConfirmation = "AdminPassword1"
                };
                var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
                {
                    Content = JsonContent.Create(admin)
                };
                var result = _client.SendAsync(request).GetAwaiter().GetResult();
                result.StatusCode.Should().Be(HttpStatusCode.OK);
                _scenarioContext.Add("admin", admin);
                _scenarioContext.Add("AdminAccessToken", "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIzIiwicm9sZSI6IkFkbWluIiwiZW1haWwiOiJBZG1pbjFFbWFpbEB0ZXN0LnRlc3QiLCJ1bmlxdWVfbmFtZSI6IlRlc3RBZG1pbjEiLCJuYmYiOjE2OTEzNjM1NDgsImV4cCI6MTg0OTIxNjM0OCwiaWF0IjoxNjkxMzYzNTQ4fQ.iJVAac5WWTdYGphD8iId3gLnIDOEX17UkfoWotwt9XH_DmfEbwDyBSl_ALQJCcO5GfMoQlo-zGZvo8k2VmqIQQ");
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
                var token = _scenarioContext.Get<string>("UserAccessToken");
                var updateUser = new UpdateUserDto()
                {
                    UserName = "TestUser1111",
                    Email = "User1Email@test.test",
                    Password = "UserPassword1",
                    PasswordConfirmation = "UserPassword1"
                };
                var content = JsonContent.Create(updateUser);
                _client.SetBearerToken(token);
                var updateUserResponse = _client.PutAsync("api/Auth/updateUser?id=5", content).GetAwaiter().GetResult();
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
                var token = _scenarioContext.Get<string>("UserAccessToken");
                _client.SetBearerToken(token);
                var updateUser = new UpdateUserDto()
                {
                    UserName = "TestUser1111",
                    Email = "User2Email@test.test",
                    Password = "UserPassword2",
                    PasswordConfirmation = "UserPassword2"
                };
                var content = JsonContent.Create(updateUser);
                var updateAnotherUserResult = _client.PutAsync("api/Auth/updateUser?id=6", content).GetAwaiter().GetResult();
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
                string token;
                if (_scenarioContext.ContainsKey("SuperUserAccessToken"))
                {
                    token = _scenarioContext.Get<string>("SuperUserAccessToken");
                }
                else
                {
                    token = _scenarioContext.Get<string>("AdminAccessToken");
                }
                _client.SetBearerToken(token);
                var updateUser = new UpdateUserDto()
                {
                    UserName = "TestUser1Updated",
                    Email = "User1Email@test.test",
                    Password = "UserPassword1",
                    PasswordConfirmation = "UserPassword1"
                };
                JsonContent content = JsonContent.Create(updateUser);
                var request = new HttpRequestMessage(HttpMethod.Put, "api/Auth/updateUser?id=5")
                {
                    Content = content
                };
                var updateUserResponse = _client.SendAsync(request).GetAwaiter().GetResult();
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
                string token = _scenarioContext.Get<string>("UserAccessToken");
                _client.SetBearerToken(token);
                var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id=5").GetAwaiter().GetResult();
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
                var token = _scenarioContext.Get<string>("UserAccessToken");
                _client.SetBearerToken(token);
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
                string token;
                if (_scenarioContext.ContainsKey("SuperUserAccessToken"))
                {
                    token = _scenarioContext.Get<string>("SuperUserAccessToken");
                }
                else
                {
                    token = _scenarioContext.Get<string>("AdminAccessToken");
                }
                _client.SetBearerToken(token);
                var deleteUserResponse = _client.DeleteAsync($"api/Auth/deleteUser?id=5").GetAwaiter().GetResult();
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
            string token;
            if (_scenarioContext.ContainsKey("SuperUserAccessToken"))
            {
                token = _scenarioContext.Get<string>("SuperUserAccessToken");
            }
            else if (_scenarioContext.ContainsKey("AdminAccessToken"))
            {
                token = _scenarioContext.Get<string>("AdminAccessToken");
            }
            else
            {
                token = _scenarioContext.Get<string>("UserAccessToken");
            }
            _client.SetBearerToken(token);
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
                if (table.Rows[0]["UserName"].IsNullOrEmpty())
                    switch (table.Rows[0]["Email"])
                    {
                        case "SuperUserEmail@test.test":
                            GivenIAmLoggedInAsASuperUser();
                            break;
                        case "TestAdmin1@test.test":
                            GivenIAmLoggedInAsAAdmin();
                            break;
                        case "User1Email@test.test":
                            GivenIAmLoggedInAsAUser();
                            break;
                    }
                else
                {
                    switch (table.Rows[0]["UserName"])
                    {
                        case "TestSuperUser":
                            GivenIAmLoggedInAsASuperUser();
                            break;
                        case "TestAdmin1":
                            GivenIAmLoggedInAsAAdmin();
                            break;
                        case "TestUser1":
                            GivenIAmLoggedInAsAUser();
                            break;
                    }
                }
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
                string token;
                if (_scenarioContext.ContainsKey("SuperUserAccessToken"))
                {
                    token = Constants.TEST_ADMIN_FORGOT_PASSWORD_TOKEN;
                }
                else if (_scenarioContext.ContainsKey("AdminAccessToken"))
                {
                    token = Constants.TEST_ADMIN_FORGOT_PASSWORD_TOKEN;
                }
                else
                {
                    token = Constants.TEST_USER_FORGOT_PASSWORD_TOKEN;
                }
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
                string token;
                if (_scenarioContext.ContainsKey("SuperUserAccessToken"))
                {
                    token = _scenarioContext.Get<string>("SuperUserAccessToken");
                }
                else if (_scenarioContext.ContainsKey("AdminAccessToken"))
                {
                    token = _scenarioContext.Get<string>("AdminAccessToken");
                }
                else
                {
                    token = _scenarioContext.Get<string>("UserAccessToken");
                }
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
                throw new Exception("Error in ThenIShouldBeAbleToResetMyPassword", exception);
            }
        }
        #endregion
    }
}
