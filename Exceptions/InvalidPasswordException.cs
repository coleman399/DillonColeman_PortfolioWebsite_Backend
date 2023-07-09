namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException(string invalidPassword) : base("Invalid Password : " + invalidPassword + ". Password must be at least 8 characters in length and contain 1 lowercase letter, 1 uppercase letter, and 1 digit.") { }
    }
}
