namespace PortfolioWebsite_Backend.Dtos.EmailDtos
{
    public class AccountDeletedEmailDto
    {
        public required List<string> To { get; set; }
        public string? ReplyTo { get; set; }
        public string Subject { get; set; } = "Account Deleted";
        public string Body { get; set; } = "Your account has been deleted.";
    }
}
