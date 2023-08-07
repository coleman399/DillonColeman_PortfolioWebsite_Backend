namespace PortfolioBackend.Exceptions
{
    [Serializable]
    public class InvalidEmailException : Exception
    {
        public InvalidEmailException(string invalidEmail) : base("Invalid Email : " + invalidEmail) { }
    }
}

