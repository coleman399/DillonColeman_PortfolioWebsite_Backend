namespace PortfolioWebsite_Backend.Dtos.UserDtos
{
    public class GetResetPasswordUserDto
    {
        public required string Token { get; set; }
        public string Message = "Please reset your password.";
    }
}
