using MailKit.Net.Smtp;
using MailKit.Security;
using Newtonsoft.Json.Linq;

namespace PortfolioBackend.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public EmailService(IMapper mapper, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            if (_webHostEnvironment.IsProduction())
            {
                var emailSettingsAsString = _configuration.GetSection("EmailSettings").Value!;
                var emailSettingsAsJson = JObject.Parse(emailSettingsAsString);
                var emailSettings = emailSettingsAsJson.ToObject<EmailConfiguration>()!;
                _emailConfig = emailSettings;
            }
            else
            {
                _emailConfig = new EmailConfiguration
                {
                    From = _configuration["EmailSettings:From"],
                    DisplayName = _configuration["EmailSettings:DisplayName"],
                    Host = _configuration["EmailSettings:Host"],
                    Port = int.Parse(_configuration["EmailSettings:Port"]!),
                    UserName = _configuration["EmailSettings:Username"],
                    Password = _configuration["EmailSettings:Password"]
                };
            }

        }

        private async Task<EmailServiceResponse<GetEmailConfirmationDto>> SendEmail(Email email)
        {
            var serviceResponse = new EmailServiceResponse<GetEmailConfirmationDto>();
            try
            {
                // Initialize a new instance of the MimeKit.MimeMessage class
                var mail = new MimeMessage();
                var ct = new CancellationToken();

                #region Sender / Receiver
                // Sender
                mail.From.Add(new MailboxAddress(_emailConfig.DisplayName, email.From ?? _emailConfig.From));
                mail.Sender = new MailboxAddress(email.DisplayName ?? _emailConfig.DisplayName, email.From ?? _emailConfig.From);

                // Receiver
                foreach (string mailAddress in email.To)
                    mail.To.Add(MailboxAddress.Parse(mailAddress));

                // Set Reply to if specified in mail data
                if (!string.IsNullOrEmpty(email.ReplyTo))
                    mail.ReplyTo.Add(new MailboxAddress(email.ReplyToName, email.ReplyTo));

                // BCC
                // Check if a BCC was supplied in the request
                if (email.Bcc != null)
                {
                    // Get only addresses where value is not null or with whitespace. x = value of address
                    foreach (string mailAddress in email.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }

                // CC
                // Check if a CC address was supplied in the request
                if (email.Cc != null)
                {
                    foreach (string mailAddress in email.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }
                #endregion

                #region Content

                // Add Content to Mime Message
                var body = new BodyBuilder();
                mail.Subject = email.Subject;
                body.HtmlBody = email.Body;
                mail.Body = body.ToMessageBody();

                #endregion

                #region Send Mail

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailConfig.Host, _emailConfig.Port, SecureSocketOptions.Auto, ct);
                await smtp.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password, ct);
                await smtp.SendAsync(mail, ct);
                await smtp.DisconnectAsync(true, ct);

                #endregion

                // Update and return response
                serviceResponse.Data = new GetEmailConfirmationDto() { Email = mail };
                serviceResponse.Message = "Email sent successfully";
            }
            catch (Exception)
            {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = "Failed to send email";
            }
            return serviceResponse;
        }

        public async Task<EmailServiceResponse<GetEmailConfirmationDto>> SendAccountHasBeenCreatedNotification(AccountCreatedEmailDto email)
        {
            return await SendEmail(_mapper.Map<Email>(email));
        }

        public async Task<EmailServiceResponse<GetEmailConfirmationDto>> SendAccountHasBeenDeletedNotification(AccountDeletedEmailDto email)
        {
            return await SendEmail(_mapper.Map<Email>(email));
        }

        public async Task<EmailServiceResponse<GetEmailConfirmationDto>> SendAccountHasBeenUpdatedNotification(AccountUpdatedEmailDto email)
        {
            return await SendEmail(_mapper.Map<Email>(email));
        }

        public async Task<EmailServiceResponse<GetEmailConfirmationDto>> SendContactHasBeenCreatedNotification(ContactCreatedEmailDto email)
        {
            return await SendEmail(_mapper.Map<Email>(email));
        }

        public async Task<EmailServiceResponse<GetEmailConfirmationDto>> SendContactHasBeenDeletedNotification(ContactDeletedEmailDto email)
        {
            return await SendEmail(_mapper.Map<Email>(email));
        }

        public async Task<EmailServiceResponse<GetEmailConfirmationDto>> SendContactHasBeenUpdatedNotification(ContactUpdatedEmailDto email)
        {
            return await SendEmail(_mapper.Map<Email>(email));
        }

        public async Task<EmailServiceResponse<GetEmailConfirmationDto>> SendForgetPassword(ForgotPasswordEmailDto email)
        {
            return await SendEmail(_mapper.Map<Email>(email));
        }
    }
}
