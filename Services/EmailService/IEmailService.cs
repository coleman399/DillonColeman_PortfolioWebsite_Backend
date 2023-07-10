namespace PortfolioWebsite_Backend.Services.EmailService
{
    public interface IEmailService
    {
        public Task<EmailServiceResponse<Email>> SendEmail(Email email);

        public Task<EmailServiceResponse<GetEmailConfirmationDto>> SendAccountHasBeenUpdatedNotification(AccountUpdatedEmailDto email);
    }
}
