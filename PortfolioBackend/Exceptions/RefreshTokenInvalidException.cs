namespace PortfolioBackend.Exceptions
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
