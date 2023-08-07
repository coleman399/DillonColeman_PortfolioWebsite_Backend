using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using PortfolioBackend.Dtos.ContactDtos;
using PortfolioBackend.Helpers;
using PortfolioBackend.Models.ContactModel;
using System.Net;
using System.Net.Http.Json;

namespace PortfolioBackend_UnitTests
{
    [TestFixture]
    public class ContactService_UnitTests
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
            Assert.That(addContactResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
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
            Assert.That(addContactResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void GetContacts_Positive_User()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Contact/getContacts");
            var getContactsResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(getContactsResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContacts_Positive_Admin()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Contact/getContacts");
            var getContactsResponse = _client.SendAsync(request).GetAwaiter().GetResult();
            Assert.That(getContactsResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContacts_Positive_SuperUser()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
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
            Assert.That(getContactsResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void UpdateContact_Positive_User()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var updateContact = new UpdateContactDto()
            {
                Name = "TestContactUpdated",
                Email = "User1Email@test.test",
                Phone = "0987654321",
                Message = "TestMessageUpdated"
            };
            var content = JsonContent.Create(updateContact);
            var updateContactResponse = _client.PutAsync("api/Contact/updateContact?id=1", content).GetAwaiter().GetResult();
            Assert.That(updateContactResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateContact_Positive_AdminUpdateUserContact()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var updateContact = new UpdateContactDto()
            {
                Name = "TestContactUpdated",
                Email = "User1Email@test.test",
                Phone = "0987654321",
                Message = "TestMessageUpdated"
            };
            var content = JsonContent.Create(updateContact);
            var updateContactResponse = _client.PutAsync("api/Contact/updateContact?id=1", content).GetAwaiter().GetResult();
            Assert.That(updateContactResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateContact_Positive_SuperUserUpdateUserContact()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var updateContact = new UpdateContactDto()
            {
                Name = "TestContactUpdated",
                Email = "User1Email@test.test",
                Phone = "0987654321",
                Message = "TestMessageUpdated"
            };
            var content = JsonContent.Create(updateContact);
            var updateContactResponse = _client.PutAsync($"api/Contact/updateContact?id=1", content).GetAwaiter().GetResult();
            Assert.That(updateContactResponse.IsSuccessStatusCode);
        }

        [Test]
        public void UpdateContact_Negative_UserUpdateAnotherUserContact()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var updateContact = new UpdateContactDto()
            {
                Name = "TestContactUpdated",
                Email = "User1Email@test.test",
                Phone = "0987654321",
                Message = "TestMessageUpdated"
            };
            var content = JsonContent.Create(updateContact);
            var updateContactResponse = _client.PutAsync($"api/Contact/updateContact?id=2", content).GetAwaiter().GetResult();
            Assert.That(updateContactResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void UpdateContact_Negative_NotLoggedIn()
        {
            var updateContact = new UpdateContactDto()
            {
                Name = "TestContactUpdated",
                Email = "User1Email@test.test",
                Phone = "0987654321",
                Message = "TestMessageUpdated"
            };
            var content = JsonContent.Create(updateContact);
            var updateContactResponse = _client.PutAsync($"api/Contact/updateContact?id=1", content).GetAwaiter().GetResult();
            Assert.That(updateContactResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void DeleteContact_Positive_User()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var deleteContactResponse = _client.DeleteAsync("api/Contact/deleteContact?id=1").GetAwaiter().GetResult();
            Assert.That(deleteContactResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteContact_Positive_AdminDeleteUserContact()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN!;
            _client.SetBearerToken(token);
            var deleteContactResponse = _client.DeleteAsync("api/Contact/deleteContact?id=1").GetAwaiter().GetResult();
            Assert.That(deleteContactResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteContact_Positive_SuperUserDeleteUserContact()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var deleteContactResponse = _client.DeleteAsync("api/Contact/deleteContact?id=1").GetAwaiter().GetResult();
            Assert.That(deleteContactResponse.IsSuccessStatusCode);
        }

        [Test]
        public void DeleteContact_Negative_UserDeleteAnotherUserContact()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var deleteContactResponse = _client.DeleteAsync("api/Contact/deleteContact?id=2").GetAwaiter().GetResult();
            Assert.That(deleteContactResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void DeleteContact_Negative_NotLoggedIn()
        {
            var deleteContactResponse = _client.DeleteAsync("api/Contact/deleteContact?id=1").GetAwaiter().GetResult();
            Assert.That(deleteContactResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void GetContactById_Positive_User()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var getContactByIdResponse = _client.GetAsync("api/Contact/getContactById?id=1").GetAwaiter().GetResult();
            Assert.That(getContactByIdResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactById_Positive_Admin()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var getContactByIdResponse = _client.GetAsync($"api/Contact/getContactById?id=1").GetAwaiter().GetResult();
            Assert.That(getContactByIdResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactById_Positive_SuperUser()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var getContactByIdResponse = _client.GetAsync("api/Contact/getContactById?id=1").GetAwaiter().GetResult();
            Assert.That(getContactByIdResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactById_Negative_UserGetAnotherUserContact()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var getContactByIdResponse = _client.GetAsync("api/Contact/getContactById?id=2").GetAwaiter().GetResult();
            Assert.That(getContactByIdResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void GetContactById_Negative_NotLoggedIn()
        {
            var getContactByIdResponse = _client.GetAsync($"api/Contact/getContactById?id=2").GetAwaiter().GetResult();
            Assert.That(getContactByIdResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void GetContactByEmail_Positive_User()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var getContactByEmailResponse = _client.GetAsync($"api/Contact/getContactsByEmail?email=User1Email@test.test").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactByEmail_Positive_Admin()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN!;
            _client.SetBearerToken(token);
            var getContactByEmailResponse = _client.GetAsync("api/Contact/getContactsByEmail?email=User1Email@test.test").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactByEmail_Positive_SuperUser()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var getContactByEmailResponse = _client.GetAsync("api/Contact/getContactsByEmail?email=User1Email@test.test").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactByEmail_Negative_UserGetAnotherUserContact()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var getContactByEmailResponse = _client.GetAsync("api/Contact/getContactsByEmail?email=User2Email@test.test").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void GetContactByEmail_Negative_NotLoggedIn()
        {
            var getContactByEmailResponse = _client.GetAsync("api/Contact/getContactsByEmail?email=User1Email@test.test").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void GetContactsWithSimilarNameToGivenName_Positive_User()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var getContactByEmailResponse = _client.GetAsync("api/Contact/getContactsByName?name=TestName1").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactsWithSimilarNameToGivenName_Positive_MakeSureUserOnlyRecievesTheirContacts()
        {
            var token = Constants.TEST_USER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var getContactByEmailResponse = _client.GetAsync($"api/Contact/getContactsByName?name=TestName1").GetAwaiter().GetResult();
            var responseContent = getContactByEmailResponse.Content.ReadFromJsonAsync<ContactServiceResponse<List<GetContactDto>>>().GetAwaiter().GetResult()!;
            var contacts = responseContent.Data!;
            foreach (var contact in contacts)
            {
                Assert.That(contact.Email!, Is.EqualTo("User1Email@test.test"));
            }
        }

        [Test]
        public void GetContactsWithSimilarNameToGivenName_Positive_Admin()
        {
            var token = Constants.TEST_ADMIN_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var getContactByEmailResponse = _client.GetAsync("api/Contact/getContactsByName?name=TestName1").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactsWithSimilarNameToGivenName_Positive_SuperUser()
        {
            var token = Constants.TEST_SUPERUSER_ACCESS_TOKEN;
            _client.SetBearerToken(token);
            var getContactByEmailResponse = _client.GetAsync($"api/Contact/getContactsByName?name=TestName1").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.IsSuccessStatusCode);
        }

        [Test]
        public void GetContactsWithSimilarNameToGivenName_Negative_NotLoggedIn()
        {
            var getContactByEmailResponse = _client.GetAsync($"api/Contact/getContactsByName?name=TestName1").GetAwaiter().GetResult();
            Assert.That(getContactByEmailResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }
}
