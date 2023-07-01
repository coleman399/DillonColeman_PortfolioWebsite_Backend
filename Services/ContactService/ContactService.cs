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

        public async Task<ContactServiceResponse<List<GetContactDto>>> AddContact(AddContactDto newContact)
        {
            if (RegexFilters.IsValidEmail(newContact.Email!) == false)
            {
                throw new InvalidEmailException(newContact.Email!);
            }
            if (RegexFilters.IsValidPhoneNum(newContact.Phone!) == false)
            {
                throw new InvalidPhoneNumberException(newContact.Phone!);
            }
            var serviceResponse = new ContactServiceResponse<List<GetContactDto>>();
            _context.Contacts.Add(_mapper.Map<Contact>(newContact));
            _context.SaveChanges();
            var dbContacts = await _context.Contacts.ToListAsync();
            serviceResponse.Data = dbContacts.Select(c => _mapper.Map<GetContactDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ContactServiceResponse<List<GetContactDto>>> GetContacts()
        {
            var dbContacts = await _context.Contacts.ToListAsync();
            var serviceResponse = new ContactServiceResponse<List<GetContactDto>>();
            serviceResponse.Data = dbContacts.Select(c => _mapper.Map<GetContactDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ContactServiceResponse<GetContactDto>> GetContactById(int id)
        {
            var dbContacts = await _context.Contacts.ToListAsync();
            var serviceResponse = new ContactServiceResponse<GetContactDto>();
            var foundContact = dbContacts.FirstOrDefault(c => c.Id == id)!;
            serviceResponse.Data = _mapper.Map<GetContactDto>(foundContact);
            return serviceResponse;
        }

        public async Task<ContactServiceResponse<GetContactDto>> UpdateContact(int id, UpdateContactDto contact)
        {
            Contact updatedContact = _context.Contacts.FirstOrDefault(c => c.Id == id)! ?? throw new ContactNotFoundException(id);
            _context.Contacts.Update(_mapper.Map<Contact>(contact));
            _context.SaveChanges();
            updatedContact = _mapper.Map<Contact>(contact);
            var dbContacts = await _context.Contacts.ToListAsync();
            var serviceResponse = new ContactServiceResponse<GetContactDto>();
            serviceResponse.Data = _mapper.Map<GetContactDto>(updatedContact);
            return serviceResponse;
        }

        public async Task<ContactServiceResponse<List<GetContactDto>>> DeleteContact(int id)
        {
            _context.Contacts.Remove(_mapper.Map<Contact>(_context.Contacts.FirstOrDefault(c => c.Id == id) ?? throw new ContactNotFoundException(id)));
            _context.SaveChanges();
            var serviceResponse = new ContactServiceResponse<List<GetContactDto>>();
            var dbContacts = await _context.Contacts.ToListAsync();
            serviceResponse.Data = dbContacts.Select(c => _mapper.Map<GetContactDto>(c)).ToList();
            return serviceResponse;
        }
    }
}