namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class ContactNotDeletedException : Exception
    {
        public ContactNotDeletedException(int id) : base("Contact with id : " + id + " was not deleted") { }
    }
}
