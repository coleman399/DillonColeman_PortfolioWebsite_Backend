﻿namespace PortfolioWebsite_Backend.Exceptions
{
    [Serializable]
    public class UserFailedToUpdateException : Exception
    {
        public UserFailedToUpdateException() : base("Failure to update user in database") { }
    }
}
