namespace PortfolioBackend.Dtos.EmailDtos
{
    public class ContactCreatedEmailDto
    {
        public required List<string> To { get; set; }
        public string? ReplyTo { get; set; }
        public string Subject { get; set; } = "Contact Created";
        public string Body { get; set; } = "*Thank you message* Your contact has been created.";
    }
}
