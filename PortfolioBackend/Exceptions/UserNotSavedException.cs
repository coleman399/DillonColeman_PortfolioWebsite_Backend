﻿namespace PortfolioBackend.Exceptions
{
    [Serializable]
    public class UserNotSavedException : Exception
    {
        public UserNotSavedException() : base("Failure to save or locate created user in database") { }
    }
}
