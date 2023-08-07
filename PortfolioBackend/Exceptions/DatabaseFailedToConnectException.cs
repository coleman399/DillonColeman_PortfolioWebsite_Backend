namespace PortfolioBackend.Exceptions
{
    public class DatabaseFailedToConnectException : Exception
    {
        public DatabaseFailedToConnectException() { }

        public DatabaseFailedToConnectException(string message) : base(message) { }
    }
}
