using System;
using Hephaestus.Classes.Exceptions;

namespace Hephaestus.Classes.Exceptions
{
    internal class InvalidCommandException : ExceptionBase
    {
        public InvalidCommandException()
        {
        }

        public InvalidCommandException(string message) : base(message)
        {
        }

        public InvalidCommandException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}