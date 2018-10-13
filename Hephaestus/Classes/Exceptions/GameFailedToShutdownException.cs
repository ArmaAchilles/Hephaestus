using System;
using Hephaestus.Classes.Exceptions;

namespace Hephaestus.Classes
{
    public class GameFailedToShutdownException : ExceptionBase
    {
        public GameFailedToShutdownException()
        {
        }

        public GameFailedToShutdownException(string message) : base(message)
        {
        }

        public GameFailedToShutdownException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}