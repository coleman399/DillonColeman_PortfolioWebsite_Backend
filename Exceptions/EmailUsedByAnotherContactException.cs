namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class EmailUsedByAnotherContactException : Exception
    {
        public EmailUsedByAnotherContactException() : base("Email is already being used by another contact.") { }
    }
}
