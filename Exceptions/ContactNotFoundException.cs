namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class ContactNotFoundException : ArgumentException
    {
        public ContactNotFoundException(int id) : base("No Contact with Id : " + id) { }
    }
}