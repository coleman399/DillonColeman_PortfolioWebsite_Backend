namespace PortfolioWebsite_Backend.Dtos.UserDtos
{
    public class GetForgotPasswordUserDto
    {
        public string? Token { get; set; }
        public string Message = "An email has been sent to your email address with instructions on how to reset your password.";
    }
}
