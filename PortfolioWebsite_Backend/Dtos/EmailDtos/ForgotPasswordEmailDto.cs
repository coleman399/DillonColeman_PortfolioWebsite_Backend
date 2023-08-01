namespace PortfolioWebsite_Backend.Dtos.EmailDtos
{
    public class ForgotPasswordEmailDto
    {
        public required List<string> To { get; set; }
        public string? ReplyTo { get; set; }
        public string Subject { get; set; } = "Reset Password";
        public string Body { get; set; } = string.Empty;
    }
}
