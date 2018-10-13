using System;

namespace Hephaestus.Classes.Exceptions
{
    internal class GameFailedToStartException : ExceptionBase
    {
        public GameFailedToStartException()
        {
        }

        public GameFailedToStartException(string message) : base(message)
        {
        }

        public GameFailedToStartException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}