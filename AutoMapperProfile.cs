namespace DillonColeman_PortfolioWebsite
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<GetContactDto, Contact>();
            CreateMap<Contact, GetContactDto>();
            CreateMap<AddContactDto, Contact>();
            CreateMap<Contact, AddContactDto>();
            CreateMap<UpdateContactDto, Contact>();
            CreateMap<Contact, UpdateContactDto>();
        }
    }
}
