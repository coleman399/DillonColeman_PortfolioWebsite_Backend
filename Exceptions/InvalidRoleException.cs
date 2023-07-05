namespace PortfolioWebsite_Backend.Exceptions
{
    public class InvalidRoleException : Exception
    {
        public InvalidRoleException(string invalidRole) : base("Invalid Role: " + invalidRole + ". Valid roles include Admin and User.") { }
    }
}
