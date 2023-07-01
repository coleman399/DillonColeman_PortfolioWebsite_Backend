namespace DillonColeman_PortfolioWebsite.Services.ContactService
{
    public interface IContactService
    {

        public Task<ContactServiceResponse<GetContactDto>> GetContactById(int id);

        public Task<ContactServiceResponse<List<GetContactDto>>> GetContacts();

        public Task<ContactServiceResponse<List<GetContactDto>>> AddContact(AddContactDto contact);

        public Task<ContactServiceResponse<GetContactDto>> UpdateContact(int id, UpdateContactDto contact);

        public Task<ContactServiceResponse<List<GetContactDto>>> DeleteContact(int id);
    }
}