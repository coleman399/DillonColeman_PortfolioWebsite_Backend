namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class HttpContextFailureException : Exception
    {
        public HttpContextFailureException() : base("HTTP Context injection failure.")
        {
        }

        public HttpContextFailureException(string message) : base(message)
        {
        }
    }
}
