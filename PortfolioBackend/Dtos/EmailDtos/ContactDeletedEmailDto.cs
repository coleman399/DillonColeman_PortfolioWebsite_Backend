namespace PortfolioBackend.Dtos.EmailDtos
{
    public class ContactDeletedEmailDto
    {
        public required List<string> To { get; set; }
        public string? ReplyTo { get; set; }
        public string Subject { get; set; } = "Contact Deleted";
        public string Body { get; set; } = "*Thank you message* Your contact has been deleted.";
    }
}
