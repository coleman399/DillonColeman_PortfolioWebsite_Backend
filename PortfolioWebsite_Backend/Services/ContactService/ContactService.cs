using F23.StringSimilarity;
using Moq;
using PortfolioWebsite_Backend.Helpers.Constants;
using System.Security.Claims;

namespace PortfolioWebsite_Backend.Services.ContactService
{
    public class ContactService : IContactService
    {
        private readonly bool _performanceTesting = false;
        private readonly ContactContext _contactContext;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContactService(ContactContext contactContext, IMapper mapper, IUserService userService, IEmailService emailService, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _contactContext = contactContext;
            _userService = userService;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            if (_webHostEnvironment.EnvironmentName.Equals(Constants.PERFORMANCE_TESTING))
            {
                _performanceTesting = true;
                _contactContext = SetContacts();
            }
        }
        private static ContactContext SetContacts()
        {
            var data = new List<Contact>()
            {
                new Contact()
                {
                    Id = 1,
                    Name = "TestName1",
                    Email = "User1Email@test.test"
                }, new Contact()
                {
                    Id = 2,
                    Name = "TestName2",
                    Email = "User2Email@test.test"
                }, new Contact()
                {
                    Id = 3,
                    Name = "TestName3",
                    Email = "User3Email@test.test"
                }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Contact>>();
            mockSet.SetReturnsDefault(data);
            mockSet.As<IQueryable<Contact>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Contact>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Contact>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Contact>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<ContactContext>();
            mockContext.Setup(m => m.Contacts).Returns(mockSet.Object);

            return mockContext.Object;
        }

        public async Task<ContactServiceResponse<List<GetContactDto>>> GetContacts()
        {
            var serviceResponse = new ContactServiceResponse<List<GetContactDto>>() { Data = null };
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    _userService.TokenCheck();

                    List<Contact> dbContacts;
                    if (_performanceTesting)
                    {
                        dbContacts = _contactContext.Contacts.ToList();
                    }
                    else
                    {
                        dbContacts = await _contactContext.Contacts.ToListAsync();
                    }
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.Admin.ToString()))
                    {
                        // Update response with all contacts
                        serviceResponse.Data = dbContacts.Select(c => _mapper.Map<GetContactDto>(c)).ToList();
                    }
                    else
                    {
                        // Update response with all contacts that share the same email as the user
                        serviceResponse.Data = dbContacts.Select(c => _mapper.Map<GetContactDto>(c)).Where(c => c.Email == _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email)).ToList();
                    }
                }
                else
                {
                    throw new HttpContextFailureException();
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = exception.Message;
            }
            return serviceResponse;
        }

        public async Task<ContactServiceResponse<GetContactDto>> GetContactById(int id)
        {
            var serviceResponse = new ContactServiceResponse<GetContactDto>() { Data = null };
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    _userService.TokenCheck();

                    // Check if contact exists
                    List<Contact> dbContacts;
                    if (_performanceTesting)
                    {
                        dbContacts = _contactContext.Contacts.ToList();
                    }
                    else
                    {
                        dbContacts = await _contactContext.Contacts.ToListAsync();
                    }
                    var foundContact = dbContacts.FirstOrDefault(c => c.Id == id) ?? throw new ContactNotFoundException();
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.Admin.ToString()) || _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.SuperUser.ToString()))
                    {
                        // Update response
                        serviceResponse.Data = _mapper.Map<GetContactDto>(foundContact);
                        serviceResponse.Message = $"Contact with id {id} found.";
                    }
                    else
                    {
                        // Check if contact belongs to user
                        var userEmail = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email.ToString());
                        if (foundContact.Email != userEmail)
                        {
                            return serviceResponse;
                        }
                        else
                        {
                            // update response
                            var userContactsDto = _mapper.Map<GetContactDto>(foundContact);
                            serviceResponse.Data = userContactsDto;
                            serviceResponse.Message = $"Contact with id {id} found.";
                        }
                    }
                }
                else
                {
                    throw new HttpContextFailureException();
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = exception.Message;
            }
            return serviceResponse;
        }

        public async Task<ContactServiceResponse<List<GetContactDto>>> GetContactsByEmail(string email)
        {
            var serviceResponse = new ContactServiceResponse<List<GetContactDto>>() { Data = null };
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    _userService.TokenCheck();


                    List<Contact> dbContacts;
                    if (_performanceTesting)
                    {
                        dbContacts = _contactContext.Contacts.ToList();
                    }
                    else
                    {
                        dbContacts = await _contactContext.Contacts.ToListAsync();
                    }

                    // Check if contacts using the queried email exists
                    var foundContacts = dbContacts.Where(c => c.Email == email).ToList() ?? throw new ContactNotFoundException(email);

                    // Check role then update response
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.Admin.ToString()) || _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.SuperUser.ToString()))
                    {
                        // update response
                        var foundContactsDto = foundContacts.Select(c => _mapper.Map<GetContactDto>(c)).ToList();
                        serviceResponse.Data = foundContactsDto;
                        serviceResponse.Message = $"Contact with email {email} found.";
                    }
                    else
                    {
                        // Check if user is allowed to view contacts with queried email
                        var userEmail = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email.ToString());
                        if (email != userEmail)
                        {
                            return serviceResponse;
                        }

                        // Search for contacts with user's email
                        var userContacts = foundContacts.Where(c => c.Email == email && c.Email == userEmail).ToList() ?? throw new ContactNotFoundException(email);
                        if (userContacts.Count == 0)
                        {
                            throw new ContactNotFoundException(email);
                        }

                        // update response
                        var userContactsDto = userContacts.Select(c => _mapper.Map<GetContactDto>(c)).ToList();

                        serviceResponse.Data = userContactsDto;
                        serviceResponse.Message = $"Contact(s) with email {email} found.";
                    }
                }
                else
                {
                    throw new HttpContextFailureException();
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = exception.Message;
            }
            return serviceResponse;
        }

        public async Task<ContactServiceResponse<List<GetContactDto>>> GetContactsWithSimilarNameTo(string name)
        {
            var serviceResponse = new ContactServiceResponse<List<GetContactDto>>() { Data = null };
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    _userService.TokenCheck();

                    List<Contact> dbContacts;
                    if (_performanceTesting)
                    {
                        dbContacts = _contactContext.Contacts.ToList();
                    }
                    else
                    {
                        dbContacts = await _contactContext.Contacts.ToListAsync();
                    }

                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.Admin.ToString()) || _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.SuperUser.ToString()))
                    {
                        // Compare names
                        List<GetContactDto> similarlyNamedContacts = new();
                        dbContacts.ForEach(c =>
                        {
                            var l = new Levenshtein();
                            if (l.Distance(c.Name, name) <= 2) similarlyNamedContacts.Add(_mapper.Map<GetContactDto>(c));
                        });

                        // Update response
                        if (similarlyNamedContacts.Count == 0)
                        {
                            serviceResponse.Data = similarlyNamedContacts;
                            serviceResponse.Message = $"No contacts with name similar to {name} found.";
                        }
                        else
                        {
                            serviceResponse.Data = similarlyNamedContacts;
                            serviceResponse.Message = $"Contacts with similar name to {name} found.";
                        }
                    }
                    else
                    {
                        // Compare names
                        List<GetContactDto> userSimilarlyNamedContacts = new();
                        dbContacts.ForEach(c =>
                        {
                            var l = new Levenshtein();
                            if (l.Distance(c.Name, name) <= 2 && c.Email == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email.ToString()))
                            {
                                userSimilarlyNamedContacts.Add(_mapper.Map<GetContactDto>(c));
                            }
                        });

                        // Verify that user is allowed to view contacts with similar name
                        foreach (var contact in userSimilarlyNamedContacts)
                        {
                            if (contact.Email != _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email.ToString()))
                            {
                                userSimilarlyNamedContacts.Remove(contact);
                            }
                        }

                        // Update response
                        if (userSimilarlyNamedContacts.Count == 0)
                        {
                            serviceResponse.Message = $"No contacts with name similar to {name} found.";
                        }
                        else
                        {
                            serviceResponse.Data = userSimilarlyNamedContacts;
                            serviceResponse.Message = $"Contacts with similar name to {name} found.";
                        }
                    }
                }
                else
                {
                    throw new HttpContextFailureException();
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = exception.Message;
            }
            return serviceResponse;
        }

        public async Task<ContactServiceResponse<GetContactDto>> AddContact(AddContactDto newContact)
        {
            var serviceResponse = new ContactServiceResponse<GetContactDto>() { Data = null };
            try
            {
                // Email is always required, check if it's valid
                if (!RegexFilters.IsValidEmail(newContact.Email)) throw new InvalidEmailException(newContact.Email!);

                // Check if phone number was provided, if so, check if it's valid
                if (newContact.Phone != null)
                {
                    if (!RegexFilters.IsValidPhoneNum(newContact.Phone))
                    {
                        throw new InvalidPhoneNumberException(newContact.Phone);
                    }
                }

                // Save contact to database
                var createdContact = _mapper.Map<Contact>(newContact);
                _contactContext.Contacts.Add(createdContact);
                _contactContext.SaveChanges();
                List<Contact> dbContacts;
                Contact foundContact;
                if (_performanceTesting)
                {
                    dbContacts = _contactContext.Contacts.ToList();
                    foundContact = createdContact;
                }
                else
                {
                    // Verify contact was created
                    dbContacts = await _contactContext.Contacts.ToListAsync();
                    foundContact = dbContacts.FirstOrDefault(c => c.Email == createdContact.Email) ?? throw new ContactNotSavedException();
                }

                // Email confirmation
                List<string> sendTo = new() { createdContact.Email };
                var email = new ContactCreatedEmailDto()
                {
                    To = sendTo
                };
                await _emailService.SendContactHasBeenCreatedNotification(email);

                // return contact with response
                serviceResponse.Data = _mapper.Map<GetContactDto>(foundContact);
                serviceResponse.Message = "Contact added successfully";
                return serviceResponse;
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = exception.Message;
                return serviceResponse;
            }
        }

        public async Task<ContactServiceResponse<GetContactDto>> UpdateContact(int id, UpdateContactDto updateContact)
        {
            var serviceResponse = new ContactServiceResponse<GetContactDto>() { Data = null };
            try
            {
                // Email is always required, check if it's valid
                if (!RegexFilters.IsValidEmail(updateContact.Email!)) throw new InvalidEmailException(updateContact.Email!);

                // Verify phone number was provided, if so, check if it's valid
                if (updateContact.Phone != null)
                {
                    if (!RegexFilters.IsValidPhoneNum(updateContact.Phone!)) throw new InvalidPhoneNumberException(updateContact.Phone!);
                }

                if (_httpContextAccessor.HttpContext != null)
                {
                    _userService.TokenCheck();

                    List<Contact> dbContacts;
                    if (_performanceTesting)
                    {
                        dbContacts = _contactContext.Contacts.ToList();
                    }
                    else
                    {
                        dbContacts = await _contactContext.Contacts.ToListAsync();
                    }

                    // Verify contact exists
                    var dbContact = dbContacts.FirstOrDefault(c => c.Id == id) ?? throw new ContactNotFoundException(id);

                    // Check role
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.User.ToString()))
                    {
                        // Users should not be able to update other users's contacts or their email unless they are updating their account with the same email
                        if (dbContact.Email != _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email) || updateContact.Email != _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email))
                        {
                            return serviceResponse;
                        }
                    }

                    // Update contact
                    var updatedContact = _mapper.Map(updateContact, dbContact);
                    _contactContext.Contacts.Update(updatedContact);
                    _contactContext.SaveChanges();

                    if (_performanceTesting)
                    {
                        dbContacts = _contactContext.Contacts.ToList();
                        dbContact = dbContacts.FirstOrDefault(c => c.Id == id) ?? throw new ContactNotFoundException();
                    }
                    else
                    {
                        // Verify contact was updated
                        dbContacts = await _contactContext.Contacts.ToListAsync();
                        dbContact = dbContacts.FirstOrDefault(c => c.Id == id) ?? throw new ContactNotFoundException();
                        if (updateContact.Email != dbContact.Email || updateContact.Name != dbContact.Name || updateContact.Message != dbContact.Message)
                        {
                            throw new ContactNotUpdatedException();
                        }
                    }


                    // Email confirmation
                    List<string> sendTo = new() { dbContact.Email };
                    var email = new ContactUpdatedEmailDto()
                    {
                        To = sendTo
                    };
                    await _emailService.SendContactHasBeenUpdatedNotification(email);

                    // Update response
                    serviceResponse.Data = _mapper.Map<GetContactDto>(dbContact);
                    serviceResponse.Message = "Contact updated successfully.";
                }
                else
                {
                    throw new HttpContextFailureException();
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = exception.Message;
            }
            return serviceResponse;
        }

        public async Task<ContactServiceResponse<DeleteContactDto>> DeleteContact(int id)
        {
            var serviceResponse = new ContactServiceResponse<DeleteContactDto>() { Data = null };
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    _userService.TokenCheck();

                    var dbContact = _contactContext.Contacts.FirstOrDefault(c => c.Id == id) ?? throw new ContactNotFoundException(id);

                    // Check role
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.User.ToString()))
                    {
                        // Check if user is authorized to delete contact, if not, throw exception
                        if (dbContact.Email != _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email.ToString()))
                        {
                            return serviceResponse;
                        }
                    }

                    // Delete contact
                    var userEmailAddress = dbContact.Email;
                    _contactContext.Contacts.Remove(dbContact);
                    _contactContext.SaveChanges();

                    if (_performanceTesting)
                    {
                        dbContact = _contactContext.Contacts.FirstOrDefault(c => c.Id == id)!;
                    }
                    else
                    {
                        // verify contact was deleted
                        dbContact = await _contactContext.Contacts.FirstOrDefaultAsync(c => c.Id == id)!;
                        if (dbContact != null) throw new ContactNotDeletedException(id);
                    }


                    // Email confirmation
                    List<string> sendTo = new() { userEmailAddress };
                    var email = new ContactDeletedEmailDto()
                    {
                        To = sendTo
                    };
                    await _emailService.SendContactHasBeenDeletedNotification(email);

                    // update response

                    serviceResponse.Data = new DeleteContactDto() { Id = id, Email = userEmailAddress };
                    serviceResponse.Message = $"Contact with id {id} has been deleted.";
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = exception.Message;
            }
            return serviceResponse;
        }
    }
}