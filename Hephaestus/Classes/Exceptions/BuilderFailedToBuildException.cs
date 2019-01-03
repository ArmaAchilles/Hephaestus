using System;

namespace Hephaestus.Classes.Exceptions
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