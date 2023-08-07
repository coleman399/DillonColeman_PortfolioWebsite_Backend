namespace PortfolioBackend.Exceptions
{
    [Serializable]
    public class ContactsFailedToDeleteException : Exception
    {
        public ContactsFailedToDeleteException() : base("Failure to delete contacts from database") { }
    }
}
