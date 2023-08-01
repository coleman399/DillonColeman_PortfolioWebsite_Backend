namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class InvalidEmailException : Exception
    {
        public InvalidEmailException(string invalidEmail) : base("Invalid Email : " + invalidEmail) { }
    }
}

