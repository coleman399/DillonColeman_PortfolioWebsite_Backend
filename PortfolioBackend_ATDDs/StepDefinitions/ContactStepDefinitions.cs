using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using PortfolioBackend.Dtos.ContactDtos;
using PortfolioBackend.Dtos.UserDtos;
using PortfolioBackend.Models.ContactModel;
using System.Net;
using System.Net.Http.Json;

namespace PortfolioBackend_ATDDs.StepDefinitions
{
    [Binding]
    public class ContactStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ContactStepDefinitions(ScenarioContext scenarioContext)
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

        #region AddContact
        [Given(@"that I provide at least a valid email address:")]
        public void GivenThatIProvideAtLeastAValidEmailAddress(Table table)
        {
            try
            {
                var addContact = new AddContactDto()
                {
                    Name = table.Rows[0]["Name"],
                    Email = table.Rows[0]["Email"],
                    Phone = table.Rows[0]["Phone"],
                    Message = table.Rows[0]["Message"]
                };
                _scenarioContext.Add("addContact", addContact);
                _scenarioContext.Get<AddContactDto>("addContact").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in GivenThatIProvideAtLeastAValidEmailAddress", exception);
            }
        }

        [When(@"I submit my contact information")]
        public void WhenISubmitMyContactInformation()
        {
            try
            {
                var contact = _scenarioContext.Get<AddContactDto>("addContact");
                var request = new HttpRequestMessage(HttpMethod.Post, "api/Contact/addContact")
                {
                    Content = JsonContent.Create(contact)
                };
                var addContactResponse = _client.SendAsync(request).GetAwaiter().GetResult();
                _scenarioContext.Add("addContactResponse", addContactResponse);
                _scenarioContext.Get<HttpResponseMessage>("addContactResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenSubmittingARegisterUserForm ", exception);
            }
        }

        [Then(@"I should be able to see a message that my contact information has been received")]
        public void ThenIShouldBeAbleToSeeAMessageThatMyContactInformationHasBeenReceived()
        {
            var result = _scenarioContext.Get<HttpResponseMessage>("addContactResponse");
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }
        #endregion

        #region GetContacts
        [Given(@"that I am logged in as a User")]
        public void GivenThatIAmLoggedInAsAUser()
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

        [When(@"I request to see my contacts")]
        public void WhenIRequestToSeeMyContacts()
        {
            try
            {
                var token = _scenarioContext.Get<string>("UserAccessToken");
                _client.SetBearerToken(token);
                var getContactsResponse = _client.GetAsync("api/Contact/getContacts").GetAwaiter().GetResult();
                _scenarioContext.Add("getContactsResponse", getContactsResponse);
                _scenarioContext.Get<HttpResponseMessage>("getContactsResponse").Should().NotBeNull();

            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToSeeMyContacts", exception);
            }
        }

        [Then(@"I should recieve a list of my contacts")]
        public void ThenIShouldRecieveAListOfMyContacts()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("getContactsResponse");
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenIShouldRecieveAListOfMyContacts", exception);
            }
        }

        [Given(@"that I am logged in as a Admin")]
        public void GivenThatIAmLoggedInAsAAdmin()
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

        [Given(@"that I am logged in as a SuperUser")]
        public void GivenThatIAmLoggedInAsASuperUser()
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

        [When(@"I request to see all contacts")]
        public void WhenIRequestToSeeAllContacts()
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
                var getContactsResponse = _client.GetAsync("api/Contact/getContacts").GetAwaiter().GetResult();
                getContactsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                _scenarioContext.Add("getContactsResponse", getContactsResponse);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToSeeAllContacts", exception);
            }
        }

        [Then(@"I should recieve a list of all contacts")]
        public void ThenIShouldRecieveAListOfAllContacts()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("getContactsResponse");
                var contacts = result.Content.ReadFromJsonAsync<ContactServiceResponse<List<Contact>>>().Result!;
                contacts.Data!.Count.Should().Be(3);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenIShouldRecieveAListOfAllContacts", exception);
            }
        }
        #endregion

        #region Update Contact
        [When(@"I request to update one of my contacts")]
        public void WhenIRequestToUpdateOneOfMyContacts()
        {
            try
            {
                var token = _scenarioContext.Get<string>("UserAccessToken");
                var updateContact = new UpdateContactDto()
                {
                    Name = "TestContactUpdated",
                    Email = "User1Email@test.test",
                    Phone = "0987654321",
                    Message = "TestMessageUpdated"
                };
                var content = JsonContent.Create(updateContact);
                _client.SetBearerToken(token);
                var updateContactResponse = _client.PutAsync("api/Contact/updateContact?id=1", content).GetAwaiter().GetResult();
                _scenarioContext.Add("updateContactResponse", updateContactResponse);
                _scenarioContext.Get<HttpResponseMessage>("updateContactResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToUpdateOneOfMyContacts", exception);
            }
        }

        [Then(@"my contact should be updated")]
        public void ThenMyContactShouldBeUpdated()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("updateContactResponse");
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenMyContactShouldBeUpdated", exception);
            }
        }

        [Then(@"If I try to update a contact that is not mine")]
        public void ThenIfITryToUpdateAContactThatIsNotMine()
        {
            try
            {
                var token = _scenarioContext.Get<string>("UserAccessToken");
                var updateContact = new UpdateContactDto()
                {
                    Name = "TestContactUpdated",
                    Email = "User1Email@test.test",
                    Phone = "0987654321",
                    Message = "TestMessageUpdated"
                };
                var content = JsonContent.Create(updateContact);
                _client.SetBearerToken(token);
                var updateAnotherUsersContactResponse = _client.PutAsync("api/Contact/updateContact?id=2", content).GetAwaiter().GetResult();
                _scenarioContext.Add("updateAnotherUsersContactResponse", updateAnotherUsersContactResponse);
                _scenarioContext.Get<HttpResponseMessage>("updateAnotherUsersContactResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenIfITryToUpdateAContactThatIsNotMine", exception);
            }
        }

        [When(@"I request to update a contact")]
        public void WhenIRequestToUpdateAContact()
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
                var updateContact = new UpdateContactDto()
                {
                    Name = "TestContactUpdated",
                    Email = "User1Email@test.test",
                    Phone = "0987654321",
                    Message = "TestMessageUpdated"
                };
                var content = JsonContent.Create(updateContact);
                _client.SetBearerToken(token);
                var updateContactResponse = _client.PutAsync($"api/Contact/updateContact?id=1", content).GetAwaiter().GetResult();
                _scenarioContext.Add("updateContactResponse", updateContactResponse);
                _scenarioContext.Get<HttpResponseMessage>("updateContactResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToUpdateAContact", exception);
            }
        }

        [Then(@"the contact should be updated")]
        public void ThenTheContactShouldBeUpdated()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("updateContactResponse");
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenTheContactShouldBeUpdated", exception);
            }
        }
        #endregion

        #region Delete Contact
        [When(@"I request to delete one of my contacts")]
        public void WhenIRequestToDeleteOneOfMyContacts()
        {
            try
            {
                var token = _scenarioContext.Get<string>("UserAccessToken");
                _client.SetBearerToken(token);
                var deleteContactResponse = _client.DeleteAsync($"api/Contact/deleteContact?id=1").GetAwaiter().GetResult();
                deleteContactResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                _scenarioContext.Add("deleteContactResponse", deleteContactResponse);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToDeleteOneOfMyContacts", exception);
            }
        }

        [Then(@"my contact should be deleted")]
        public void ThenMyContactShouldBeDeleted()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("deleteContactResponse");
                var test = result.Content.ReadFromJsonAsync<ContactServiceResponse<DeleteContactDto>>().Result!;
                test.Success.Should().BeTrue();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenMyContactShouldBeDeleted", exception);
            }
        }

        [Then(@"If I try to delete a contact that is not mine")]
        public void ThenIfITryToDeleteAContactThatIsNotMine()
        {
            try
            {
                var token = _scenarioContext.Get<string>("UserAccessToken");
                _client.SetBearerToken(token);
                var deleteAnotherUsersContactResponse = _client.DeleteAsync($"api/Contact/deleteContact?id=2").GetAwaiter().GetResult();
                _scenarioContext.Add("deleteAnotherUsersContactResponse", deleteAnotherUsersContactResponse);
                _scenarioContext.Get<HttpResponseMessage>("deleteAnotherUsersContactResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenIfITryToDeleteAContactThatIsNotMine", exception);
            }
        }

        [When(@"I request to delete a contact")]
        public void WhenIRequestToDeleteAContact()
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
                var deleteContactResponse = _client.DeleteAsync($"api/Contact/deleteContact?id=1").GetAwaiter().GetResult();
                _scenarioContext.Add("deleteContactResponse", deleteContactResponse);
                _scenarioContext.Get<HttpResponseMessage>("deleteContactResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToDeleteAContact", exception);
            }
        }

        [Then(@"the contact should be deleted")]
        public void ThenTheContactShouldBeDeleted()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("deleteContactResponse");
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenTheContactShouldBeDeleted", exception);
            }
        }
        #endregion

        #region Get Contact By Id
        [When(@"I request to see a contact by ID")]
        public void WhenIRequestToSeeAContactByID()
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
                var getContactWithIdResponse = _client.GetAsync("api/Contact/getContactById?id=1").GetAwaiter().GetResult();
                _scenarioContext.Add("getContactWithIdResponse", getContactWithIdResponse);
                _scenarioContext.Get<HttpResponseMessage>("getContactWithIdResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToSeeAContactByID", exception);
            }
        }

        [Then(@"I should recieve the contact")]
        public void ThenIShouldRecieveTheContact()
        {
            try
            {
                var result = _scenarioContext.Get<HttpResponseMessage>("getContactWithIdResponse");
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenIShouldRecieveTheContact", exception);
            }
        }

        [Then(@"If I try to get a contact that is not mine")]
        public void ThenIfITryToGetAContactThatIsNotMine()
        {
            try
            {
                var token = _scenarioContext.Get<string>("UserAccessToken");
                _client.SetBearerToken(token);
                var getAnotherUsersContactWithIdResponse = _client.GetAsync($"api/Contact/getContactById?id=2").GetAwaiter().GetResult();
                _scenarioContext.Add("getAnotherUsersContactWithIdResponse", getAnotherUsersContactWithIdResponse);
                _scenarioContext.Get<HttpResponseMessage>("getAnotherUsersContactWithIdResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToSeeAContactByID", exception);
            }
        }
        #endregion

        #region Get Contact By Email
        [When(@"I request to get contacts by email")]
        public void WhenIRequestToGetContactsByEmail()
        {
            try
            {
                string token;
                if (_scenarioContext.ContainsKey("AdminAccessToken"))
                {
                    token = _scenarioContext.Get<string>("AdminAccessToken");
                }
                else if (_scenarioContext.ContainsKey("SuperUserAccessToken"))
                {
                    token = _scenarioContext.Get<string>("SuperUserAccessToken");
                }
                else
                {
                    token = _scenarioContext.Get<string>("UserAccessToken");
                }
                _client.SetBearerToken(token);
                var getContactstWithEmailResponse = _client.GetAsync($"api/Contact/getContactsByEmail?email=User1Email@test.test").GetAwaiter().GetResult();
                _scenarioContext.Add("getContactstWithEmailResponse", getContactstWithEmailResponse);
                _scenarioContext.Get<HttpResponseMessage>("getContactstWithEmailResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToSeeContactsByEmail", exception);
            }
        }

        [Then(@"I should recieve the contacts")]
        public void ThenIShouldRecieveTheContacts()
        {
            try
            {
                HttpResponseMessage? result;
                if (_scenarioContext.ContainsKey("getContactstWithEmailResponse"))
                {
                    result = _scenarioContext.Get<HttpResponseMessage>("getContactstWithEmailResponse");
                }
                else
                {
                    result = _scenarioContext.Get<HttpResponseMessage>("getContactsWithSimilarNameToGivenNameResponse");
                }
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenIShouldRecieveTheContacts", exception);
            }
        }

        [Then(@"If I try to get contacts that are not mine")]
        public void ThenIfITryToGetContactsThatAreNotMine()
        {
            try
            {
                var token = _scenarioContext.Get<string>("UserAccessToken");
                _client.SetBearerToken(token);
                var getAnotherUsersContactWithEmailResponse = _client.GetAsync("api/Contact/getContactsByEmail?email=User2Email@test.test").GetAwaiter().GetResult();
                _scenarioContext.Add("getAnotherUsersContactWithEmailResponse", getAnotherUsersContactWithEmailResponse);
                _scenarioContext.Get<HttpResponseMessage>("getAnotherUsersContactWithEmailResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in ThenIfITryToGetContactsThatAreNotMine", exception);
            }
        }
        #endregion

        #region Get Contacts With Similar Name To Given Name
        [When(@"I request to get contacts with a similar name to a given name")]
        public void WhenIRequestToGetContactsWithASimilarNameToAGivenName()
        {
            try
            {
                string token;
                if (_scenarioContext.ContainsKey("AdminAccessToken"))
                {
                    token = _scenarioContext.Get<string>("AdminAccessToken");
                }
                else if (_scenarioContext.ContainsKey("SuperUserAccessToken"))
                {
                    token = _scenarioContext.Get<string>("SuperUserAccessToken");
                }
                else
                {
                    token = _scenarioContext.Get<string>("UserAccessToken");
                }
                _client.SetBearerToken(token);
                var getContactsWithSimilarNameToGivenNameResponse = _client.GetAsync($"api/Contact/getContactsByName?name=TestName1").GetAwaiter().GetResult();
                _scenarioContext.Add("getContactsWithSimilarNameToGivenNameResponse", getContactsWithSimilarNameToGivenNameResponse);
                _scenarioContext.Get<HttpResponseMessage>("getContactsWithSimilarNameToGivenNameResponse").Should().NotBeNull();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in WhenIRequestToGetContactsWithASimilarNameToAGivenName", exception);
            }
        }
        #endregion
    }
}
