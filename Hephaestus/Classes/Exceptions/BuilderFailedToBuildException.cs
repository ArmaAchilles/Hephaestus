using System;
using Hephaestus.Classes.Exceptions;

namespace Hephaestus.Classes
{
    internal class BuilderFailedToBuildException : ExceptionBase
    {
        public BuilderFailedToBuildException()
        {
        }

        public BuilderFailedToBuildException(string message) : base(message)
        {
        }

        public BuilderFailedToBuildException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}