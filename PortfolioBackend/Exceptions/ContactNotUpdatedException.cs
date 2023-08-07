namespace PortfolioBackend.Exceptions
{
    [Serializable]
    public class ContactNotUpdatedException : Exception
    {
        public ContactNotUpdatedException() : base("Failure to update or locate contact in database") { }
    }
}
