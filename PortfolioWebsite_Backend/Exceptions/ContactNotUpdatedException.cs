namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class ContactNotUpdatedException : Exception
    {
        public ContactNotUpdatedException() : base("Failure to update or locate contact in database") { }
    }
}
