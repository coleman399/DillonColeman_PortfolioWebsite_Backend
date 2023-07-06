namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(int id) : base($"No user found with id {id}.")
        {
        }
    }
}
