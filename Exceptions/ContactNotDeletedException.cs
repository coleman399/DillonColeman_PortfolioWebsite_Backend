namespace PortfolioWebsite_Backend.Exceptions
{
    public class ContactNotDeletedException : Exception
    {
        public ContactNotDeletedException(int id) : base("Contact with id : " + id + " could not be deleted") { }
    }
}
