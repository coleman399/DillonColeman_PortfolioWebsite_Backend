using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework.Internal;
using PortfolioWebsite_Backend.Dtos.ContactDtos;
using PortfolioWebsite_Backend.Dtos.UserDtos;
using PortfolioWebsite_Backend.Models.ContactModel;
using PortfolioWebsite_Backend.Models.UserModel;
using PortfolioWebsite_Backend_UnitTests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace PortfolioWebsite_Backend_UnitTests
{
    [TestFixture]
    public class ContactService_UnitTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;


        [SetUp]
        public void Setup()
        {
            _factory = DependencyInjection.FactoryProvider();
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
        public void AddContact_Positive()
        {
            var addContact = new AddContactDto()
            {
                Name = "TestUser1",
                Email = "User1Email@test.test",
                Phone = "1234567890",
                Message = "Test Message"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Contact/addContact")
            {
                Content = JsonContent.Create(addContact)
            };
            var addContactResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(addContactResponse.IsSuccessStatusCode);
        }

        [Test]
        public void AddContact_Positive_NoName_NoMessage()
        {
            var addContact = new AddContactDto()
            {
                Name = "",
                Email = "User1Email@test.test",
                Phone = "1234567890",
                Message = ""
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Contact/addContact")
            {
                Content = JsonContent.Create(addContact)
            };
            var addContactResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(addContactResponse.IsSuccessStatusCode);
        }

        [Test]
        public void AddContact_Negative_InvalidEmail()
        {
            var addContact = new AddContactDto()
            {
                Name = "TestUser1",
                Email = "User1Email",
                Phone = "1234567890",
                Message = "Test Message"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Contact/addContact")
            {
                Content = JsonContent.Create(addContact)
            };
            var addContactResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(addContactResponse.StatusCode.Equals(HttpStatusCode.BadRequest));
        }

        [Test]
        public void AddContact_Negative_InvalidPhone()
        {
            var addContact = new AddContactDto()
            {
                Name = "TestUser1",
                Email = "User1Email@test.test",
                Phone = "",
                Message = "Test Message"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Contact/addContact")
            {
                Content = JsonContent.Create(addContact)
            };
            var addContactResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(addContactResponse.StatusCode.Equals(HttpStatusCode.BadRequest));
        }

        [Test]
        public void GetContacts_Positive_User()
        {
            var token = LoginAsUser();
            _client.SetBearerToken(token);
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Contact/getContacts");
            var getContactsResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(getContactsResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContacts_Positive_Admin()
        {
            var token = LoginAsAdmin();
            _client.SetBearerToken(token);
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Contact/getContacts");
            var getContactsResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(getContactsResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContacts_Positive_SuperUser()
        {
            var token = LoginAsSuperUser();
            _client.SetBearerToken(token);
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Contact/getContacts");
            var getContactsResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(getContactsResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContacts_Negative_NotLoggedIn()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Contact/getContacts");
            var getContactsResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(getContactsResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void UpdateContact_Positive_User()
        {
            var token = LoginAsUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToUpdate = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var updateContact = new UpdateContactDto()
            {
                Name = "TestContactUpdated",
                Email = "User1Email@test.test",
                Phone = "0987654321",
                Message = "TestMessageUpdated"
            };
            var content = JsonContent.Create(updateContact);
            var updateContactResponse = _client.PutAsync($"api/Contact/updateContact?id={contactToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateContactResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateContact_Positive_AdminUpdateUserContact()
        {
            var token = LoginAsAdmin();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToUpdate = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var updateContact = new UpdateContactDto()
            {
                Name = "TestContactUpdated",
                Email = "User1Email@test.test",
                Phone = "0987654321",
                Message = "TestMessageUpdated"
            };
            var content = JsonContent.Create(updateContact);
            var updateContactResponse = _client.PutAsync($"api/Contact/updateContact?id={contactToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateContactResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateContact_Positive_SuperUserUpdateUserContact()
        {
            var token = LoginAsSuperUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToUpdate = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var updateContact = new UpdateContactDto()
            {
                Name = "TestContactUpdated",
                Email = "User1Email@test.test",
                Phone = "0987654321",
                Message = "TestMessageUpdated"
            };
            var content = JsonContent.Create(updateContact);
            var updateContactResponse = _client.PutAsync($"api/Contact/updateContact?id={contactToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateContactResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateContact_Negative_UserUpdateAnotherUserContact()
        {
            var token = LoginAsUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToUpdate = contactList.FirstOrDefault(contact => contact.Email.Equals("User2Email@test.test"))!;
            var updateContact = new UpdateContactDto()
            {
                Name = "TestContactUpdated",
                Email = "User1Email@test.test",
                Phone = "0987654321",
                Message = "TestMessageUpdated"
            };
            var content = JsonContent.Create(updateContact);
            var updateContactResponse = _client.PutAsync($"api/Contact/updateContact?id={contactToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateContactResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void UpdateContact_Negative_NotLoggedIn()
        {
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToUpdate = contactList.FirstOrDefault(contact => contact.Email.Equals("User2Email@test.test"))!;
            var updateContact = new UpdateContactDto()
            {
                Name = "TestContactUpdated",
                Email = "User1Email@test.test",
                Phone = "0987654321",
                Message = "TestMessageUpdated"
            };
            var content = JsonContent.Create(updateContact);
            var updateContactResponse = _client.PutAsync($"api/Contact/updateContact?id={contactToUpdate.Id}", content).GetAwaiter().GetResult();
            Assert.That(updateContactResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void DeleteContact_Positive_User()
        {
            var token = LoginAsUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToDelete = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var deleteContactResponse = _client.DeleteAsync($"api/Contact/deleteContact?id={contactToDelete.Id}").GetAwaiter().GetResult();
            Assert.That(deleteContactResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteContact_Positive_AdminDeleteUserContact()
        {
            var token = LoginAsAdmin();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToDelete = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var deleteContactResponse = _client.DeleteAsync($"api/Contact/deleteContact?id={contactToDelete.Id}").GetAwaiter().GetResult();
            Assert.That(deleteContactResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteContact_Positive_SuperUserDeleteUserContact()
        {
            var token = LoginAsSuperUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToDelete = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var deleteContactResponse = _client.DeleteAsync($"api/Contact/deleteContact?id={contactToDelete.Id}").GetAwaiter().GetResult();
            Assert.That(deleteContactResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteContact_Negative_UserDeleteAnotherUserContact()
        {
            var token = LoginAsUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToDelete = contactList.FirstOrDefault(contact => contact.Email.Equals("User2Email@test.test"))!;
            var deleteContactResponse = _client.DeleteAsync($"api/Contact/deleteContact?id={contactToDelete.Id}").GetAwaiter().GetResult();
            Assert.That(deleteContactResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void DeleteContact_Negative_NotLoggedIn()
        {
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToDelete = contactList.FirstOrDefault(contact => contact.Email.Equals("User2Email@test.test"))!;
            var deleteContactResponse = _client.DeleteAsync($"api/Contact/deleteContact?id={contactToDelete.Id}").GetAwaiter().GetResult();
            Assert.That(deleteContactResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void GetContactById_Positive_User()
        {
            var token = LoginAsUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var getContactByIdResponse = _client.GetAsync($"api/Contact/getContactById?id={contactToGet.Id}").GetAwaiter().GetResult();
            Assert.That(getContactByIdResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactById_Positive_Admin()
        {
            var token = LoginAsAdmin();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var getContactByIdResponse = _client.GetAsync($"api/Contact/getContactById?id={contactToGet.Id}").GetAwaiter().GetResult();
            Assert.That(getContactByIdResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactById_Positive_SuperUser()
        {
            var token = LoginAsSuperUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var getContactByIdResponse = _client.GetAsync($"api/Contact/getContactById?id={contactToGet.Id}").GetAwaiter().GetResult();
            Assert.That(getContactByIdResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactById_Negative_UserGetAnotherUserContact()
        {
            var token = LoginAsUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User2Email@test.test"))!;
            var getContactByIdResponse = _client.GetAsync($"api/Contact/getContactById?id={contactToGet.Id}").GetAwaiter().GetResult();
            Assert.That(getContactByIdResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void GetContactById_Negative_NotLoggedIn()
        {
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User2Email@test.test"))!;
            var getContactByIdResponse = _client.GetAsync($"api/Contact/getContactById?id={contactToGet.Id}").GetAwaiter().GetResult();
            Assert.That(getContactByIdResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void GetContactByEmail_Positive_User()
        {
            var token = LoginAsUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var getContactByEmailResponse = _client.GetAsync($"api/Contact/getContactsByEmail?email={contactToGet.Email}").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactByEmail_Positive_Admin()
        {
            var token = LoginAsAdmin();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var getContactByEmailResponse = _client.GetAsync($"api/Contact/getContactsByEmail?email={contactToGet.Email}").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactByEmail_Positive_SuperUser()
        {
            var token = LoginAsSuperUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var getContactByEmailResponse = _client.GetAsync($"api/Contact/getContactsByEmail?email={contactToGet.Email}").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactByEmail_Negative_UserGetAnotherUserContact()
        {
            var token = LoginAsUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User2Email@test.test"))!;
            var getContactByEmailResponse = _client.GetAsync($"api/Contact/getContactsByEmail?email={contactToGet.Email}").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void GetContactByEmail_Negative_NotLoggedIn()
        {
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User2Email@test.test"))!;
            var getContactByEmailResponse = _client.GetAsync($"api/Contact/getContactsByEmail?email={contactToGet.Email}").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void GetContactsWithSimilarNameToGivenName_Positive_User()
        {
            var token = LoginAsUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var getContactByEmailResponse = _client.GetAsync($"api/Contact/getContactsByName?name={contactToGet.Name}").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactsWithSimilarNameToGivenName_Positive_MakeSureUserOnlyRecievesTheirContacts()
        {
            var token = LoginAsUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var getContactByEmailResponse = _client.GetAsync($"api/Contact/getContactsByName?name={contactToGet.Name}").GetAwaiter().GetResult();
            var responseContent = getContactByEmailResponse.Content.ReadFromJsonAsync<ContactServiceResponse<List<GetContactDto>>>().GetAwaiter().GetResult()!;
            var contacts = responseContent.Data!;
            foreach (var contact in contacts)
            {
                Assert.That(contact.Email!.Equals("User1Email@test.test"));
            }
        }

        [Test]
        public void GetContactsWithSimilarNameToGivenName_Positive_Admin()
        {
            var token = LoginAsAdmin();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var getContactByEmailResponse = _client.GetAsync($"api/Contact/getContactsByName?name={contactToGet.Name}").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactsWithSimilarNameToGivenName_Positive_SuperUser()
        {
            var token = LoginAsSuperUser();
            _client.SetBearerToken(token);
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User1Email@test.test"))!;
            var getContactByEmailResponse = _client.GetAsync($"api/Contact/getContactsByName?name={contactToGet.Name}").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactsWithSimilarNameToGivenName_Negative_NotLoggedIn()
        {
            var serviceContainer = _factory.Services.CreateScope();
            var contactDb = serviceContainer.ServiceProvider.GetRequiredService<ContactContext>();
            var contactList = contactDb.Contacts.ToList();
            var contactToGet = contactList.FirstOrDefault(contact => contact.Email.Equals("User2Email@test.test"))!;
            var getContactByEmailResponse = _client.GetAsync($"api/Contact/getContactsByName?name={contactToGet.Name}").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }
    }
}
