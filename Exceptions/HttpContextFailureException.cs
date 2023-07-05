namespace PortfolioWebsite_Backend.Exceptions
{
    public class HttpContextFailureException : Exception
    {
        public HttpContextFailureException() : base("HTTP Context injection failure.")
        {
        }
    }
}
