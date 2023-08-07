namespace PortfolioBackend.Dtos.EmailDtos
{
    public class AccountUpdatedEmailDto
    {
        public required List<string> To { get; set; }
        public string? ReplyTo { get; set; }
        public string Subject { get; set; } = "Account Updated";
        public string Body { get; set; } = "Your account has been updated.";
    }
}
