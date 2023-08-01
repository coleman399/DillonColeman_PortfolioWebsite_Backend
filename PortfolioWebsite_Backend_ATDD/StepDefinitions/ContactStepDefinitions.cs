using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PortfolioWebsite_Backend.Dtos.ContactDtos;
using PortfolioWebsite_Backend.Dtos.UserDtos;
using PortfolioWebsite_Backend.Helpers.Constants;
using PortfolioWebsite_Backend.Models.ContactModel;
using PortfolioWebsite_Backend.Models.UserModel;
using System.Net;
using System.Net.Http.Json;

namespace PortfolioWebsite_Backend_Testing.StepDefinitions
{
    [Binding]
    public class ContactStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public ContactStepDefinitions(ScenarioContext scenarioContext)
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

                    var dbContactContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ContactContext>));
                    services.Remove(dbContactContextDescriptor!);
                    var dbUserContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UserContext>));
                    services.Remove(dbUserContextDescriptor!);

                    services.AddDbContext<ContactContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestingDB");
                    }).AddDbContext<UserContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestingDB");
                    });

                    // Seeding database 
                    var serviceProvider = services.BuildServiceProvider();
                    using var scope = serviceProvider.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var userContext = scopedServices.GetRequiredService<UserContext>();
                    userContext.Database.EnsureDeleted();
                    userContext.Database.EnsureCreated();
                    try
                    {
                        SeedUserData(userContext);
                        userContext.SaveChanges();
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Error in seeding user data", exception);
                    }
                    var contactContext = scopedServices.GetRequiredService<ContactContext>();
                    contactContext.Database.EnsureDeleted();
                    contactContext.Database.EnsureCreated();
                    try
                    {
                        SeedContactData(contactContext);
                        contactContext.SaveChanges();
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Error in seeding contact data", exception);
                    }
                });
            });
            return factory;

        }

        private static void SeedUserData(UserContext context)
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

        private static void SeedContactData(ContactContext context)
        {
            /*  * Retrieve data from the database *
                var responseContent = _factory.Services.CreateScope();
                var db = responseContent.ServiceProvider.GetRequiredService<ContactContext>();
                var contactList = db.Contacts.ToList();
            */

            context.Contacts.Add(new Contact()
            {
                Name = "TestName1",
                Email = "User1Email@test.test"
            });
            context.Contacts.Add(new Contact()
            {
                Name = "TestName2",
                Email = "User2Email@test.test"
            });
            context.Contacts.Add(new Contact()
            {
                Name = "TestName3",
                Email = "User3Email@test.test"
            });
        }

        [TearDown]
        public void Cleanup()
        {
            _factory.Dispose();
            _factory = FactoryReset();
            _client = _factory.CreateClient();
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
                var userLoginResponse = result.Content.ReadFromJsonAsync<UserServiceResponse<GetLoggedInUserDto>>().Result;
                _scenarioContext.Add("userLoginResponse", userLoginResponse);
                _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse").Data!.Token.Should().NotBeNull();
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
                var response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse");
                _client.SetBearerToken(response.Data!.Token);
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

        [Given(@"that I am logged in as a SuperUser")]
        public void GivenThatIAmLoggedInAsASuperUser()
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

        [When(@"I request to see all contacts")]
        public void WhenIRequestToSeeAllContacts()
        {
            try
            {
                var response = new UserServiceResponse<GetLoggedInUserDto>();
                if (_scenarioContext.ContainsKey("adminLoginResponse"))
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("adminLoginResponse");
                }
                else
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("superUserLoginResponse");
                }
                _client.SetBearerToken(response.Data!.Token);
                var getContactsResponse = _client.GetAsync("api/Contact/getContacts").GetAwaiter().GetResult();
                _scenarioContext.Add("getContactsResponse", getContactsResponse);
                _scenarioContext.Get<HttpResponseMessage>("getContactsResponse").Should().NotBeNull();
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
                result.StatusCode.Should().Be(HttpStatusCode.OK);
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
                var userLoginResponse = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse");
                var serviceContainer = _factory.Services.CreateScope();
                var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
                var contactList = contactDb.Contacts.ToList();
                var userDb = serviceContainer.ServiceProvider.GetRequiredService<UserContext>();
                var userList = userDb.Users.ToList();
                var user = userList.FirstOrDefault(u => u.AccessToken == userLoginResponse.Data?.Token);
                var contactToUpdate = contactList.FirstOrDefault(contact => contact.Email == user?.Email);
                var updateContact = new UpdateContactDto()
                {
                    Name = "TestContactUpdated",
                    Email = user?.Email!,
                    Phone = "0987654321",
                    Message = "TestMessageUpdated"
                };
                var content = JsonContent.Create(updateContact);
                _client.SetBearerToken(userLoginResponse.Data?.Token);
                var updateContactResponse = _client.PutAsync($"api/Contact/updateContact?id={contactToUpdate?.Id}", content).GetAwaiter().GetResult();
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
                var userLoginResponse = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse");
                var test = _factory.Services.CreateScope();
                var contactDb = test.ServiceProvider.GetRequiredService<ContactContext>();
                var contactList = contactDb.Contacts.ToList();
                var userDb = test.ServiceProvider.GetRequiredService<UserContext>();
                var userList = userDb.Users.ToList();
                var user = userList.FirstOrDefault(u => u.AccessToken == userLoginResponse.Data?.Token);
                var contactToUpdate = contactList.FirstOrDefault(contact => contact.Email != user?.Email);
                var updateContact = new UpdateContactDto()
                {
                    Name = "TestContactUpdated",
                    Email = user?.Email!,
                    Phone = "0987654321",
                    Message = "TestMessageUpdated"
                };
                var content = JsonContent.Create(updateContact);
                _client.SetBearerToken(userLoginResponse.Data?.Token);
                var updateAnotherUsersContactResponse = _client.PutAsync($"api/Contact/updateContact?id={contactToUpdate?.Id}", content).GetAwaiter().GetResult();
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
                UserServiceResponse<GetLoggedInUserDto> response;
                if (_scenarioContext.ContainsKey("adminLoginResponse"))
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("adminLoginResponse");
                }
                else
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("superUserLoginResponse");
                }
                var test = _factory.Services.CreateScope();
                var contactDb = test.ServiceProvider.GetRequiredService<ContactContext>();
                var contactList = contactDb.Contacts.ToList();
                var userDb = test.ServiceProvider.GetRequiredService<UserContext>();
                var userList = userDb.Users.ToList();
                var user = userList.FirstOrDefault(u => u.AccessToken == response.Data?.Token);
                var contactToUpdate = contactList.FirstOrDefault(contact => contact.Email != user?.Email);
                var updateContact = new UpdateContactDto()
                {
                    Name = "TestContactUpdated",
                    Email = user?.Email!,
                    Phone = "0987654321",
                    Message = "TestMessageUpdated"
                };
                var content = JsonContent.Create(updateContact);
                _client.SetBearerToken(response.Data?.Token);
                var updateContactResponse = _client.PutAsync($"api/Contact/updateContact?id={contactToUpdate?.Id}", content).GetAwaiter().GetResult();
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
                var userLoginResponse = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse");
                var test = _factory.Services.CreateScope();
                var contactDb = test.ServiceProvider.GetRequiredService<ContactContext>();
                var contactList = contactDb.Contacts.ToList();
                var userDb = test.ServiceProvider.GetRequiredService<UserContext>();
                var userList = userDb.Users.ToList();
                var user = userList.FirstOrDefault(u => u.AccessToken == userLoginResponse.Data?.Token);
                var contactToDelete = contactList.FirstOrDefault(contact => contact.Email == user?.Email);
                _client.SetBearerToken(userLoginResponse.Data?.Token);
                var deleteContactResponse = _client.DeleteAsync($"api/Contact/deleteContact?id={contactToDelete?.Id}").GetAwaiter().GetResult();
                _scenarioContext.Add("deleteContactResponse", deleteContactResponse);
                _scenarioContext.Get<HttpResponseMessage>("deleteContactResponse").Should().NotBeNull();
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
                result.StatusCode.Should().Be(HttpStatusCode.OK);
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
                var userLoginResponse = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse");
                var test = _factory.Services.CreateScope();
                var contactDb = test.ServiceProvider.GetRequiredService<ContactContext>();
                var contactList = contactDb.Contacts.ToList();
                var userDb = test.ServiceProvider.GetRequiredService<UserContext>();
                var userList = userDb.Users.ToList();
                var user = userList.FirstOrDefault(u => u.AccessToken == userLoginResponse.Data?.Token);
                var contactToDelete = contactList.FirstOrDefault(contact => contact.Email != user?.Email);
                _client.SetBearerToken(userLoginResponse.Data?.Token);
                var deleteAnotherUsersContactResponse = _client.DeleteAsync($"api/Contact/deleteContact?id={contactToDelete?.Id}").GetAwaiter().GetResult();
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
                UserServiceResponse<GetLoggedInUserDto> response;
                if (_scenarioContext.ContainsKey("adminLoginResponse"))
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("adminLoginResponse");
                }
                else
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("superUserLoginResponse");
                }
                var test = _factory.Services.CreateScope();
                var contactDb = test.ServiceProvider.GetRequiredService<ContactContext>();
                var contactList = contactDb.Contacts.ToList();
                var userDb = test.ServiceProvider.GetRequiredService<UserContext>();
                var userList = userDb.Users.ToList();
                var user = userList.FirstOrDefault(u => u.AccessToken == response.Data?.Token);
                var contactToDelete = contactList.FirstOrDefault(contact => contact.Email != user?.Email);
                _client.SetBearerToken(response.Data?.Token);
                var deleteContactResponse = _client.DeleteAsync($"api/Contact/deleteContact?id={contactToDelete?.Id}").GetAwaiter().GetResult();
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
                var test = _factory.Services.CreateScope();
                var contactDb = test.ServiceProvider.GetRequiredService<ContactContext>();
                var contactList = contactDb.Contacts.ToList();
                var userDb = test.ServiceProvider.GetRequiredService<UserContext>();
                var userList = userDb.Users.ToList();
                bool AdminOrSuperUser;
                Contact? contactToGet;
                UserServiceResponse<GetLoggedInUserDto> response;
                if (_scenarioContext.ContainsKey("adminLoginResponse"))
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("adminLoginResponse");
                    AdminOrSuperUser = true;
                }
                else if (_scenarioContext.ContainsKey("superUserLoginResponse"))
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("superUserLoginResponse");
                    AdminOrSuperUser = true;
                }
                else
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse");
                    AdminOrSuperUser = false;
                }
                var user = userList.FirstOrDefault(u => u.AccessToken == response.Data?.Token);
                if (AdminOrSuperUser)
                {
                    contactToGet = contactList.FirstOrDefault(contact => contact.Email != user?.Email);
                }
                else
                {
                    contactToGet = contactList.FirstOrDefault(contact => contact.Email == user?.Email);
                }
                _client.SetBearerToken(response.Data?.Token);
                var getContactWithIdResponse = _client.GetAsync($"api/Contact/getContactById?id={contactToGet?.Id}").GetAwaiter().GetResult();
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
                var test = _factory.Services.CreateScope();
                var contactDb = test.ServiceProvider.GetRequiredService<ContactContext>();
                var contactList = contactDb.Contacts.ToList();
                var userDb = test.ServiceProvider.GetRequiredService<UserContext>();
                var userList = userDb.Users.ToList();
                var userLoginResponse = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse");
                var user = userList.FirstOrDefault(u => u.AccessToken == userLoginResponse.Data?.Token);
                var contactToGet = contactList.FirstOrDefault(contact => contact.Email != user?.Email);
                _client.SetBearerToken(user?.AccessToken);
                var getAnotherUsersContactWithIdResponse = _client.GetAsync($"api/Contact/getContactById?id={contactToGet?.Id}").GetAwaiter().GetResult();
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
                var test = _factory.Services.CreateScope();
                var contactDb = test.ServiceProvider.GetRequiredService<ContactContext>();
                var contactList = contactDb.Contacts.ToList();
                var userDb = test.ServiceProvider.GetRequiredService<UserContext>();
                var userList = userDb.Users.ToList();
                bool AdminOrSuperUser;
                Contact? contactToGet;
                UserServiceResponse<GetLoggedInUserDto> response;
                if (_scenarioContext.ContainsKey("adminLoginResponse"))
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("adminLoginResponse");
                    AdminOrSuperUser = true;
                }
                else if (_scenarioContext.ContainsKey("superUserLoginResponse"))
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("superUserLoginResponse");
                    AdminOrSuperUser = true;
                }
                else
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse");
                    AdminOrSuperUser = false;
                }
                var user = userList.FirstOrDefault(u => u.AccessToken == response.Data?.Token);
                if (AdminOrSuperUser)
                {
                    contactToGet = contactList.FirstOrDefault(contact => contact.Email != user?.Email);
                }
                else
                {
                    contactToGet = contactList.FirstOrDefault(contact => contact.Email == user?.Email);
                }
                _client.SetBearerToken(response.Data?.Token);
                var getContactstWithEmailResponse = _client.GetAsync($"api/Contact/getContactsByEmail?email={contactToGet?.Email}").GetAwaiter().GetResult();
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
                var test = _factory.Services.CreateScope();
                var contactDb = test.ServiceProvider.GetRequiredService<ContactContext>();
                var contactList = contactDb.Contacts.ToList();
                var userDb = test.ServiceProvider.GetRequiredService<UserContext>();
                var userList = userDb.Users.ToList();
                var userLoginResponse = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse");
                var user = userList.FirstOrDefault(u => u.AccessToken == userLoginResponse.Data?.Token);
                var contactToGet = contactList.FirstOrDefault(contact => contact.Email != user?.Email);
                _client.SetBearerToken(user?.AccessToken);
                var getAnotherUsersContactWithEmailResponse = _client.GetAsync($"api/Contact/getContactsByEmail?email={contactToGet?.Email}").GetAwaiter().GetResult();
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
                var test = _factory.Services.CreateScope();
                var contactDb = test.ServiceProvider.GetRequiredService<ContactContext>();
                var contactList = contactDb.Contacts.ToList();
                var userDb = test.ServiceProvider.GetRequiredService<UserContext>();
                var userList = userDb.Users.ToList();
                bool AdminOrSuperUser;
                Contact? contactToGet;
                UserServiceResponse<GetLoggedInUserDto> response;
                if (_scenarioContext.ContainsKey("adminLoginResponse"))
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("adminLoginResponse");
                    AdminOrSuperUser = true;
                }
                else if (_scenarioContext.ContainsKey("superUserLoginResponse"))
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("superUserLoginResponse");
                    AdminOrSuperUser = true;
                }
                else
                {
                    response = _scenarioContext.Get<UserServiceResponse<GetLoggedInUserDto>>("userLoginResponse");
                    AdminOrSuperUser = false;
                }
                var user = userList.FirstOrDefault(u => u.AccessToken == response.Data?.Token);
                if (AdminOrSuperUser)
                {
                    contactToGet = contactList.FirstOrDefault(contact => contact.Email != user?.Email);
                }
                else
                {
                    contactToGet = contactList.FirstOrDefault(contact => contact.Email == user?.Email);
                }
                _client.SetBearerToken(response.Data?.Token);
                var getContactsWithSimilarNameToGivenNameResponse = _client.GetAsync($"api/Contact/getContactsByName?name={contactToGet?.Name}").GetAwaiter().GetResult();
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
