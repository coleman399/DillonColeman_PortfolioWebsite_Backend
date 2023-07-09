namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class InvalidRoleException : Exception
    {
        public InvalidRoleException(string invalidRole) : base("Invalid Role: " + invalidRole + ". Valid roles include Admin and User.") { }
    }
}
