﻿using F23.StringSimilarity;
using System.Security.Claims;

namespace PortfolioWebsite_Backend.Services.ContactService
{
    public class ContactService : IContactService
    {
        private readonly IMapper _mapper;
        private readonly ContactContext _contactContext;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContactService(IMapper mapper, ContactContext contactContext, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _contactContext = contactContext;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ContactServiceResponse<List<GetContactDto>>> GetContacts()
        {

            var serviceResponse = new ContactServiceResponse<List<GetContactDto>>() { Success = true, Data = null };
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    _userService.TokenCheck();

                    var dbContacts = await _contactContext.Contacts.ToListAsync();
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
                serviceResponse.Message = exception.Message;
            }
            return serviceResponse;
        }

        public async Task<ContactServiceResponse<GetContactDto>> GetContactById(int id)
        {
            var serviceResponse = new ContactServiceResponse<GetContactDto>() { Success = false, Data = null };
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    _userService.TokenCheck();

                    // Check if contact exists
                    var dbContacts = await _contactContext.Contacts.ToListAsync();
                    var foundContact = dbContacts.FirstOrDefault(c => c.Id == id) ?? throw new ContactNotFoundException();
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.Admin.ToString()))
                    {
                        // Update response
                        serviceResponse.Success = true;
                        serviceResponse.Data = _mapper.Map<GetContactDto>(foundContact);
                        serviceResponse.Message = $"Contact with id {id} found.";
                    }
                    else
                    {
                        // Check if contact belongs to user
                        serviceResponse.Success = true;
                        var userEmail = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email.ToString());
                        if (foundContact.Email != userEmail)
                        {
                            throw new UnauthorizedAccessException("You are not authorized to view contacts with this email.");
                        }

                        // update response
                        var userContactsDto = _mapper.Map<GetContactDto>(foundContact);
                        serviceResponse.Data = userContactsDto;
                        serviceResponse.Message = $"Contact with id {id} found.";
                    }
                }
                else
                {
                    throw new HttpContextFailureException();
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Message = exception.Message;
            }
            return serviceResponse;
        }

        public async Task<ContactServiceResponse<List<GetContactDto>>> GetContactsByEmail(string email)
        {
            var serviceResponse = new ContactServiceResponse<List<GetContactDto>>() { Success = false, Data = null };
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    _userService.TokenCheck();

                    // Check if contacts using the queried email exists
                    var dbContacts = await _contactContext.Contacts.ToListAsync();
                    var foundContacts = dbContacts.Where(c => c.Email == email).ToList() ?? throw new ContactNotFoundException(email);

                    // Check role then update response
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.Admin.ToString()))
                    {
                        // update response
                        var foundContactsDto = foundContacts.Select(c => _mapper.Map<GetContactDto>(c)).ToList();
                        serviceResponse.Success = true;
                        serviceResponse.Data = foundContactsDto;
                        serviceResponse.Message = $"Contact with email {email} found.";
                    }
                    else
                    {
                        // Check if user is allowed to view contacts with queried email
                        var userEmail = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email.ToString());
                        if (email != userEmail)
                        {
                            serviceResponse.Success = true;
                            throw new UnauthorizedAccessException("You are not authorized to view contacts with this email.");
                        }

                        // Search for contacts with user's email
                        var userContacts = foundContacts.Where(c => c.Email == email && c.Email == userEmail).ToList() ?? throw new ContactNotFoundException(email);

                        // update response
                        var userContactsDto = userContacts.Select(c => _mapper.Map<GetContactDto>(c)).ToList();
                        serviceResponse.Success = true;
                        serviceResponse.Data = userContactsDto;
                        serviceResponse.Message = $"Contact with email {email} found.";
                    }
                }
                else
                {
                    throw new HttpContextFailureException();
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Message = exception.Message;

            }
            return serviceResponse;
        }

        public async Task<ContactServiceResponse<List<GetContactDto>>> GetContactsWithSimilarNameTo(string name)
        {
            var serviceResponse = new ContactServiceResponse<List<GetContactDto>>();
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    _userService.TokenCheck();

                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.Admin.ToString()))
                    {
                        // Compare names
                        List<GetContactDto> similarlyNamedContacts = new();
                        var dbContacts = await _contactContext.Contacts.ToListAsync();
                        dbContacts.ForEach(c =>
                        {
                            var l = new Levenshtein();
                            if (l.Distance(c.Name, name) <= 2) similarlyNamedContacts.Add(_mapper.Map<GetContactDto>(c));
                        });

                        // Update response
                        if (similarlyNamedContacts.Count == 0)
                        {
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
                        var dbContacts = await _contactContext.Contacts.ToListAsync();
                        dbContacts.ForEach(c =>
                        {
                            var l = new Levenshtein();
                            if (l.Distance(c.Name, name) <= 2 && c.Email == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email.ToString()))
                            {
                                userSimilarlyNamedContacts.Add(_mapper.Map<GetContactDto>(c));
                            }
                        });

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
                serviceResponse.Data = null;
                serviceResponse.Message = exception.Message;
            }
            return serviceResponse;
        }

        public async Task<ContactServiceResponse<GetContactDto>> AddContact(AddContactDto newContact)
        {
            var serviceResponse = new ContactServiceResponse<GetContactDto>();
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
                Contact createdContact = _mapper.Map<Contact>(newContact);
                _contactContext.Contacts.Add(createdContact);
                _contactContext.SaveChanges();

                // Verify contact was created
                var dbContacts = await _contactContext.Contacts.ToListAsync();
                var foundContact = dbContacts.FirstOrDefault(c => c.Email == createdContact.Email) ?? throw new ContactNotSavedException();

                // return contact with response
                serviceResponse.Data = _mapper.Map<GetContactDto>(foundContact);
                serviceResponse.Message = "Contact added successfully";
                return serviceResponse;
            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Data = null;
                serviceResponse.Message = exception.Message;
                return serviceResponse;
            }
        }

        public async Task<ContactServiceResponse<GetContactDto>> UpdateContact(int id, UpdateContactDto updateContact)
        {
            var serviceResponse = new ContactServiceResponse<GetContactDto>() { Success = false, Data = null };
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

                    // Verify contact exists
                    var dbContacts = await _contactContext.Contacts.ToListAsync();
                    var dbContact = dbContacts.FirstOrDefault(c => c.Id == id) ?? throw new ContactNotFoundException(id);

                    // Check role
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.User.ToString()))
                    {
                        // Check if user is authorized to update contact, if not, throw exception
                        if (dbContact.Email != _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email))
                        {
                            serviceResponse.Success = true;
                            throw new UnauthorizedAccessException("You are not authorized to update this contact.");
                        }
                    }

                    // Update contact
                    _contactContext.Contacts.Update(_mapper.Map(updateContact, dbContact));
                    _contactContext.SaveChanges();

                    // Verify contact was updated
                    dbContacts = await _contactContext.Contacts.ToListAsync();
                    dbContact = dbContacts.FirstOrDefault(c => c.Id == id) ?? throw new ContactNotUpdatedException();

                    // Update response
                    serviceResponse.Success = true;
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
                serviceResponse.Message = exception.Message;
            }
            return serviceResponse;
        }

        public async Task<ContactServiceResponse<DeleteContactDto>> DeleteContact(int id)
        {
            var serviceResponse = new ContactServiceResponse<DeleteContactDto>() { Success = false, Data = null };
            try
            {
                // Check if contact exists
                var contact = _contactContext.Contacts.FirstOrDefault(c => c.Id == id) ?? throw new ContactNotFoundException(id);

                if (_httpContextAccessor.HttpContext != null)
                {
                    _userService.TokenCheck();

                    // Check role
                    if (_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!.Equals(Roles.User.ToString()))
                    {
                        // Check if user is authorized to delete contact, if not, throw exception
                        if (contact.Email != _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email.ToString()))
                        {
                            serviceResponse.Success = true;
                            throw new UnauthorizedAccessException();
                        }
                    }

                    // Delete contact
                    _contactContext.Contacts.Remove(contact);
                    _contactContext.SaveChanges();

                    // verify contact was deleted
                    var dbContact = await _contactContext.Contacts.FirstOrDefaultAsync(c => c.Id == id);
                    if (dbContact != null) throw new ContactNotDeletedException(id);

                    // update response
                    serviceResponse.Success = true;
                    serviceResponse.Data = new DeleteContactDto() { Id = id, Email = contact.Email };
                    serviceResponse.Message = $"Contact with id {id} has been deleted.";
                }
            }
            catch (Exception exception)
            {
                serviceResponse.Message = exception.Message;
            }
            return serviceResponse;
        }
    }
}