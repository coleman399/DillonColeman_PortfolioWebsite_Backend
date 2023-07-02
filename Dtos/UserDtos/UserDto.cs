namespace PortfolioWebsite_Backend.Dtos.UserDtos
{
    public class UserDto
    {
        public required string UserName { get; set; }
        public required string PasswordHash { get; set; }
    }
}
