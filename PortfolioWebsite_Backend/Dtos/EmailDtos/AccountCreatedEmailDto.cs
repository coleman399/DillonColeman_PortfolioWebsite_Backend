namespace PortfolioWebsite_Backend.Dtos.EmailDtos
{
    public class AccountCreatedEmailDto
    {
        public required List<string> To { get; set; }
        public string? ReplyTo { get; set; }
        public string Subject { get; set; } = "Account Created";
        public string Body { get; set; } = "Your account has been created.";
    }
}