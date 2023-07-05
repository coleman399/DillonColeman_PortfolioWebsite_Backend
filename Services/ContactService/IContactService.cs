namespace PortfolioWebsite_Backend.Services.ContactService
{
    public interface IContactService
    {
        public Task<ContactServiceResponse<List<GetContactDto>>> GetContacts();
        public Task<ContactServiceResponse<GetContactDto>> GetContactById(int id);
        public Task<ContactServiceResponse<GetContactDto>> GetContactByEmail(string email);
        public Task<ContactServiceResponse<List<GetContactDto>>> GetContactsWithSimilarNameTo(string name);
        public Task<ContactServiceResponse<GetContactDto>> AddContact(AddContactDto addContact);
        public Task<ContactServiceResponse<GetContactDto>> UpdateContact(int id, UpdateContactDto updateContact);
        public Task<ContactServiceResponse<DeleteContactDto>> DeleteContact(int id);
    }
}