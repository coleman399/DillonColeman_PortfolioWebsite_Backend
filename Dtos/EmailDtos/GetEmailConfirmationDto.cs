using MimeKit;

namespace PortfolioWebsite_Backend.Dtos.EmailDtos
{
    public class GetEmailConfirmationDto
    {
        public MimeMessage? Email { get; set; }
    }
}
