using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortfolioWebsite_Backend.Helpers.Constants;
using PortfolioWebsite_Backend.Models.ContactModel;
using PortfolioWebsite_Backend.Models.UserModel;

namespace PortfolioWebsite_Backend_UnitTests.Helpers
{
    public class DependencyInjection
    {

        public static IConfiguration GetConfiguration(WebApplicationFactory<Program> _factory)
        {
            var configuration = _factory.Services.GetRequiredService<IConfiguration>();
            return configuration;
        }

        public static WebApplicationFactory<Program> FactoryProvider()
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
                UserName = "TestAdmin1",
                Email = "AdminEmail@test.test",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword1"),
                Role = Roles.Admin.ToString(),
                AccessToken = string.Empty,
            });
            context.Users.Add(new User()
            {
                UserName = "TestAdmin2",
                Email = "Admin2Email@test.test",
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
    }
}
