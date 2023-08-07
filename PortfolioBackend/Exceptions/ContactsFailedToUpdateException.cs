namespace PortfolioBackend.Exceptions
{
    [Serializable]
    public class ContactsFailedToUpdateException : Exception
    {
        public ContactsFailedToUpdateException() : base("Failure to update contacts in database") { }

        public ContactsFailedToUpdateException(string message) : base(message) { }
    }
}
