namespace PortfolioWebsite_Backend.Services.EmailService
{
    public interface IEmailService
    {
        public Task<EmailServiceResponse<GetEmailConfirmationDto>> SendAccountHasBeenUpdatedNotification(AccountUpdatedEmailDto email);
        public Task<EmailServiceResponse<GetEmailConfirmationDto>> SendAccountHasBeenCreatedNotification(AccountCreatedEmailDto email);
        public Task<EmailServiceResponse<GetEmailConfirmationDto>> SendAccountHasBeenDeletedNotification(AccountDeletedEmailDto email);
        public Task<EmailServiceResponse<GetEmailConfirmationDto>> SendContactHasBeenUpdatedNotification(ContactUpdatedEmailDto email);
        public Task<EmailServiceResponse<GetEmailConfirmationDto>> SendContactHasBeenCreatedNotification(ContactCreatedEmailDto email);
        public Task<EmailServiceResponse<GetEmailConfirmationDto>> SendContactHasBeenDeletedNotification(ContactDeletedEmailDto email);
        public Task<EmailServiceResponse<GetEmailConfirmationDto>> SendForgetPassword(ForgotPasswordEmailDto email);
    }
}
