namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class InvalidEmailException : FormatException
    {
        public InvalidEmailException(string invalidEmail) : base("Invalid Email : " + invalidEmail) { }
    }
}

