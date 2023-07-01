namespace PortfolioWebsite_Backend.Models.UserModel
{
    public class User
    {
        public int Id { get; set; }
        public required string UserName { get; set; } = string.Empty;
        public required string PasswordHash { get; set; } = string.Empty;
    }
}
