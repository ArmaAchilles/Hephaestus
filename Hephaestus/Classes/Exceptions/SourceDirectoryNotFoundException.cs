using System;

namespace Hephaestus.Classes.Exceptions
{
    internal class SourceDirectoryNotFoundException : ExceptionBase
    {
        public SourceDirectoryNotFoundException()
        {
        }

        public SourceDirectoryNotFoundException(string message) : base(message)
        {
        }

        public SourceDirectoryNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}