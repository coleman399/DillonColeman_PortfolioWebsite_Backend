namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class InvalidPhoneNumberException : FormatException
    {
        public InvalidPhoneNumberException(string invalidPhoneNumber) : base("Invalid Phone Number : " + invalidPhoneNumber) { }
    }
}
