namespace DillonColeman_PortfolioWebsite.Services.ContactService
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
                var contact = _context.Contacts.FirstOrDefault(c => c.Id == id)! ?? throw new ContactNotFoundException(id);

                var dbContacts = await _context.Contacts.ToListAsync();
                var foundContact = dbContacts.FirstOrDefault(c => c.Id == id)!;
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
                var contact = _context.Contacts.FirstOrDefault(c => c.Email == email)! ?? throw new ContactNotFoundException(email);

                var dbContacts = await _context.Contacts.ToListAsync();
                var foundContact = dbContacts.FirstOrDefault(c => c.Email == email)!;
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

        public async Task<ContactServiceResponse<GetContactDto>> GetContactByName(string name)
        {
            var serviceResponse = new ContactServiceResponse<GetContactDto>();
            try
            {
                // Check if contact exists
                var contact = _context.Contacts.FirstOrDefault(c => c.Name == name)! ?? throw new ContactNotFoundException(name);

                var dbContacts = await _context.Contacts.ToListAsync();
                var foundContact = dbContacts.FirstOrDefault(c => c.Name == name)!;
                serviceResponse.Data = _mapper.Map<GetContactDto>(foundContact);
                serviceResponse.Message = $"Contact with Name {name} found.";
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

        public async Task<ContactServiceResponse<List<GetContactDto>>> AddContact(AddContactDto newContact)
        {
            var serviceResponse = new ContactServiceResponse<List<GetContactDto>>();
            try
            {

                // Check if email is already in use
                var contact = _context.Contacts.FirstOrDefault(c => c.Email == newContact.Email) ?? throw new EmailUsedByAnotherContactException();

                // Email is always required, check if it's valid
                if (!RegexFilters.IsValidEmail(contact.Email!)) throw new InvalidEmailException(newContact.Email!);

                // Check if phone number was provided, if so, check if it's valid
                if (newContact.Phone != null)
                {
                    if (!RegexFilters.IsValidPhoneNum(newContact.Phone!)) throw new InvalidPhoneNumberException(newContact.Phone!);
                }
                _context.Contacts.Add(_mapper.Map<Contact>(newContact));
                _context.SaveChanges();
                var dbContacts = await _context.Contacts.ToListAsync();
                serviceResponse.Data = dbContacts.Select(c => _mapper.Map<GetContactDto>(c)).ToList();
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
            var serviceResponse = new ContactServiceResponse<GetContactDto>();
            try
            {
                // Check if contact exists
                var contact = _context.Contacts.FirstOrDefault(c => c.Id == id)! ?? throw new ContactNotFoundException(id);

                // Check if email is already in use
                contact = _context.Contacts.FirstOrDefault(c => c.Email == updateContact.Email && c.Id != id)! ?? throw new EmailUsedByAnotherContactException();

                // Email is always required, check if it's valid
                if (!RegexFilters.IsValidEmail(updateContact.Email!)) throw new InvalidEmailException(updateContact.Email!);

                // Check if phone number was provided, if so, check if it's valid
                if (updateContact.Phone != null)
                {
                    if (!RegexFilters.IsValidPhoneNum(updateContact.Phone!)) throw new InvalidPhoneNumberException(updateContact.Phone!);
                }

                contact = _mapper.Map<Contact>(updateContact);
                _context.Contacts.Update(contact);
                _context.SaveChanges();
                var dbContacts = await _context.Contacts.ToListAsync();
                var foundContact = dbContacts.FirstOrDefault(c => c.Id == id)!;
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

        public async Task<ContactServiceResponse<List<GetContactDto>>> DeleteContact(int id)
        {
            var serviceResponse = new ContactServiceResponse<List<GetContactDto>>();
            try
            {
                // Check if contact exists
                var contact = _context.Contacts.FirstOrDefault(c => c.Id == id)! ?? throw new ContactNotFoundException(id);

                _context.Contacts.Remove(contact);
                _context.SaveChanges();
                var dbContacts = await _context.Contacts.ToListAsync();
                serviceResponse.Data = dbContacts.Select(c => _mapper.Map<GetContactDto>(c)).ToList();
                serviceResponse.Message = $"Contact with id {id} has been deleted.";
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