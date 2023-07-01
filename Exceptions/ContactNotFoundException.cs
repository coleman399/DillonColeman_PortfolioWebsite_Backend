namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class ContactNotFoundException : Exception
    {
        public ContactNotFoundException(int id) : base("No Contact with Id : " + id)
        {
        }
    }
}