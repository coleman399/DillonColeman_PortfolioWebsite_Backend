﻿namespace PortfolioBackend.Exceptions
{
    [Serializable]
    public class UnavailableUserNameException : Exception
    {
        public UnavailableUserNameException() : base("User name is already being used.") { }
    }
}
