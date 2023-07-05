namespace PortfolioWebsite_Backend.Helpers
{
    // For more information visit: https://docs.automapper.org/en/stable/Configuration.html
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // CreateMap<TSource, TDestination>();
            CreateMap<GetContactDto, Contact>();
            CreateMap<Contact, GetContactDto>();
            CreateMap<AddContactDto, Contact>();
            CreateMap<Contact, AddContactDto>();
            CreateMap<UpdateContactDto, Contact>();
            CreateMap<Contact, UpdateContactDto>();
            CreateMap<GetContactDto, UpdateContactDto>();
            CreateMap<UpdateContactDto, GetContactDto>();
            CreateMap<AddContactDto, GetContactDto>();
            CreateMap<GetContactDto, AddContactDto>();
            CreateMap<GetContactDto, DeleteContactDto>();
            CreateMap<DeleteContactDto, GetContactDto>();
            CreateMap<RegisterUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
            CreateMap<User, RegisterUserDto>();
            CreateMap<GetUserDto, User>();
            CreateMap<User, GetUserDto>();
            CreateMap<RegisterUserDto, GetUserDto>();
            CreateMap<GetUserDto, RegisterUserDto>();
            CreateMap<LoginUserDto, User>();
            CreateMap<User, LoginUserDto>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.PasswordHash));
            CreateMap<User, GetLoggedInUserDto>()
                .ForMember(dest => dest.Token, opt => opt.NullSubstitute(""));
            CreateMap<GetLoggedInUserDto, User>();

        }
    }
}
