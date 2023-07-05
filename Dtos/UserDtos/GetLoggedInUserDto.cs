namespace PortfolioWebsite_Backend.Dtos.UserDtos
{
    public class GetLoggedInUserDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? Role { get; set; }
    }
}
