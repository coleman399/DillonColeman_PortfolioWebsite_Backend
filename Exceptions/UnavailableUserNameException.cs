namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class UnavailableUserNameException : Exception
    {
        public UnavailableUserNameException() : base("User name is already being used.") { }
    }
}
