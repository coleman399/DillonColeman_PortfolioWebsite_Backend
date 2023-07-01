namespace DillonColeman_PortfolioWebsite.Services.ContactService
{
    public interface IContactService
    {
        public Task<ContactServiceResponse<List<GetContactDto>>> GetContacts();

        public Task<ContactServiceResponse<GetContactDto>> GetContactById(int id);

        public Task<ContactServiceResponse<GetContactDto>> GetContactByEmail(string email);

        public Task<ContactServiceResponse<GetContactDto>> GetContactByName(string name);

        public Task<ContactServiceResponse<List<GetContactDto>>> AddContact(AddContactDto addContact);

        public Task<ContactServiceResponse<GetContactDto>> UpdateContact(int id, UpdateContactDto updateContact);

        public Task<ContactServiceResponse<List<GetContactDto>>> DeleteContact(int id);
    }
}