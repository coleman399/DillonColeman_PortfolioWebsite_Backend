namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class InvalidPhoneNumberException : Exception
    {
        public InvalidPhoneNumberException(string invalidPhoneNumber) : base("Invalid Phone Number: " + invalidPhoneNumber + ". Please provide a 10 digit phone number. Valid dividing characters include [-.].") { }
    }
}
