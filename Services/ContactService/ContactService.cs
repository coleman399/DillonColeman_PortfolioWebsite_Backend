using F23.StringSimilarity;

namespace PortfolioWebsite_Backend.Services.ContactService
{
    public class ContactService : IContactService
    {
        private readonly IMapper _mapper;
        private readonly ContactContext _context;

        public ContactService(IMapper mapper, ContactContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ContactServiceResponse<List<GetContactDto>>> GetContacts()
        {
            var serviceResponse = new ContactServiceResponse<List<GetContactDto>>();
            try
            {
                //  Return all contacts in response
                var dbContacts = await _context.Contacts.ToListAsync();
                serviceResponse.Data = dbContacts.Select(c => _mapper.Map<GetContactDto>(c)).ToList();
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

        public async Task<ContactServiceResponse<GetContactDto>> GetContactById(int id)
        {
            var serviceResponse = new ContactServiceResponse<GetContactDto>();
            try
            {
                // Check if contact exists
                var dbContacts = await _context.Contacts.ToListAsync();
                var foundContact = dbContacts.FirstOrDefault(c => c.Id == id)! ?? throw new ContactNotFoundException(id);

                // return contact with response
                serviceResponse.Data = _mapper.Map<GetContactDto>(foundContact);
                serviceResponse.Message = $"Contact with id {id} found.";
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

        public async Task<ContactServiceResponse<GetContactDto>> GetContactByEmail(string email)
        {
            var serviceResponse = new ContactServiceResponse<GetContactDto>();
            try
            {
                // Check if contact exists
                var dbContacts = await _context.Contacts.ToListAsync();
                var foundContact = dbContacts.FirstOrDefault(c => c.Email == email)! ?? throw new ContactNotFoundException(email);

                // return contact with response
                serviceResponse.Data = _mapper.Map<GetContactDto>(foundContact);
                serviceResponse.Message = $"Contact with email {email} found.";
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

        // TODO: Should return all contacts with similar name
        public async Task<ContactServiceResponse<List<GetContactDto>>> GetContactsWithSimilarNameTo(string name)
        {
            var serviceResponse = new ContactServiceResponse<List<GetContactDto>>();
            try
            {
                // Compare names
                List<GetContactDto> similarlyNamedContacts = new();
                var dbContacts = await _context.Contacts.ToListAsync();
                dbContacts.ForEach(c =>
                {
                    var l = new Levenshtein();
                    if (l.Distance(c.Name, name) <= 2) similarlyNamedContacts.Add(_mapper.Map<GetContactDto>(c));
                });

                // return contacts with response
                if (similarlyNamedContacts.Count == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"No contacts with name similar to {name} found.";
                }
                else
                {
                    serviceResponse.Data = similarlyNamedContacts;
                    serviceResponse.Message = $"Contacts with similar name to {name} found.";
                }
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


        // TODO: Should not return all contacts, only the one that was added
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

                // Check if email is already in use
                await _context.Contacts.ForEachAsync(c =>
                {
                    if (c.Email == newContact.Email) throw new UnavailableEmailException();
                });

                // Save contact to database
                Contact createdContact = _mapper.Map<Contact>(newContact);
                _context.Contacts.Add(createdContact);
                _context.SaveChanges();

                // Check if contact was created
                var dbContacts = await _context.Contacts.ToListAsync();
                var foundContact = dbContacts.FirstOrDefault(c => c.Email == createdContact.Email)! ?? throw new ContactNotSavedException();

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

        // Creating a new contact when it should be updating an existing one
        public async Task<ContactServiceResponse<GetContactDto>> UpdateContact(int id, UpdateContactDto updateContact)
        {
            var serviceResponse = new ContactServiceResponse<GetContactDto>();
            try
            {
                // Email is always required, check if it's valid
                if (!RegexFilters.IsValidEmail(updateContact.Email!)) throw new InvalidEmailException(updateContact.Email!);

                // Check if phone number was provided, if so, check if it's valid
                if (updateContact.Phone != null)
                {
                    if (!RegexFilters.IsValidPhoneNum(updateContact.Phone!)) throw new InvalidPhoneNumberException(updateContact.Phone!);
                }

                // Check if contact exists
                var dbContacts = await _context.Contacts.ToListAsync();
                var foundContact = dbContacts.FirstOrDefault(c => c.Id == id) ?? throw new ContactNotFoundException(id);

                // Check if email is already in use
                dbContacts.ForEach(c =>
                {
                    if (c.Id != id && c.Email == updateContact.Email) throw new UnavailableEmailException();
                });

                // Update contact
                _context.Contacts.Update(_mapper.Map(updateContact, foundContact));
                _context.SaveChanges();

                // return contact with response
                serviceResponse.Data = _mapper.Map<GetContactDto>(foundContact);
                serviceResponse.Message = $"Updated contact with id {id}.";
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

        public async Task<ContactServiceResponse<DeleteContactDto>> DeleteContact(int id)
        {
            var serviceResponse = new ContactServiceResponse<DeleteContactDto>();
            try
            {
                // Check if contact exists
                var contact = _context.Contacts.FirstOrDefault(c => c.Id == id)! ?? throw new ContactNotFoundException(id);

                // Delete contact
                _context.Contacts.Remove(contact);
                _context.SaveChanges();

                // verify contact was deleted
                var dbContacts = await _context.Contacts.ToListAsync();
                var foundContact = dbContacts.FirstOrDefault(c => c.Id == id);

                if (foundContact == null)
                {
                    //return id and response
                    serviceResponse.Data = _mapper.Map<DeleteContactDto>(foundContact);
                    serviceResponse.Message = $"Contact with id {id} has been deleted.";
                }
                else
                {
                    throw new ContactNotDeletedException(id);
                }
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
    }
}