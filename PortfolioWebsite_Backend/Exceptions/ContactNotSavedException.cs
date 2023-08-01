namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class ContactNotSavedException : Exception
    {
        public ContactNotSavedException() : base("Failure to save or locate created contact in database") { }
    }
}
