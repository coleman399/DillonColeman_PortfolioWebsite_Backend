namespace PortfolioWebsite_Backend.Exceptions
{
    public class RefreshTokenInvalidException : Exception
    {
        public RefreshTokenInvalidException()
        {
        }
        public RefreshTokenInvalidException(string? message) : base(message)
        {
        }
    }
}
