namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class InvalidUserNameException : Exception
    {
        public InvalidUserNameException(string invalidUserName) : base("Invalid User Name : " + invalidUserName) { }
    }
}

