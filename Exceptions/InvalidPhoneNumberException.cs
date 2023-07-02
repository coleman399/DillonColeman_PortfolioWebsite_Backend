namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class InvalidPhoneNumberException : Exception
    {
        public InvalidPhoneNumberException(string invalidPhoneNumber) : base("Invalid Phone Number : " + invalidPhoneNumber) { }
    }
}
