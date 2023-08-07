﻿namespace PortfolioBackend.Exceptions
{
    [Serializable]
    public class UnavailableEmailException : Exception
    {
        public UnavailableEmailException() : base("Email is already being used.") { }
    }
}
