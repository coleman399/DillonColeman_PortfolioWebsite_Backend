namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class UnavailableEmailException : Exception
    {
        public UnavailableEmailException() : base("Email is already being used.") { }
    }
}
