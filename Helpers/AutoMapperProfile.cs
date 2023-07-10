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
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.AccessToken, opt => opt.Ignore());
            CreateMap<User, RegisterUserDto>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.PasswordHash));
            CreateMap<GetUserDto, User>();
            CreateMap<User, GetUserDto>();
            CreateMap<RegisterUserDto, GetUserDto>();
            CreateMap<GetUserDto, RegisterUserDto>();
            CreateMap<LoginUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
            CreateMap<User, LoginUserDto>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.PasswordHash));
            CreateMap<User, GetLoggedInUserDto>()
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.AccessToken));
            CreateMap<GetLoggedInUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<User, UpdateUserDto>();
            CreateMap<DeleteUserDto, User>();
            CreateMap<User, DeleteUserDto>();
            CreateMap<GetUserDto, UpdateUserDto>();
            CreateMap<UpdateUserDto, GetUserDto>();
            CreateMap<GetUserDto, DeleteUserDto>();
            CreateMap<DeleteUserDto, GetUserDto>();
            CreateMap<AccountCreatedEmailDto, Email>();
            CreateMap<AccountUpdatedEmailDto, Email>();
            CreateMap<AccountDeletedEmailDto, Email>();
            CreateMap<ContactCreatedEmailDto, Email>();
            CreateMap<ContactUpdatedEmailDto, Email>();
            CreateMap<ContactDeletedEmailDto, Email>();
            CreateMap<Email, GetEmailConfirmationDto>();

        }
    }
}
