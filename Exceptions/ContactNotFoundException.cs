namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class ContactNotFoundException : Exception
    {
        public ContactNotFoundException(int id) : base("No Contact with Id : " + id) { }

        public ContactNotFoundException(string input) : base($"Contact could not be found using {input}") { }

    }
}