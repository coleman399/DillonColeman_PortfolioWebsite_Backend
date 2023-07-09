namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class UserNotFoundException : Exception
    {

        public UserNotFoundException() : base("No user found.")
        {
        }
        public UserNotFoundException(int id) : base($"No user found with id {id}.")
        {
        }

        public UserNotFoundException(string message) : base(message)
        {
        }
    }
}
