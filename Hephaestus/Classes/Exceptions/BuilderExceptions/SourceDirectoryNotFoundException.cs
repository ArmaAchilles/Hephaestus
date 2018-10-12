using System;

namespace Hephaestus.Classes.Exceptions.BuilderExceptions
{
    public class SourceDirectoryNotFoundException : Exception
    {
        public SourceDirectoryNotFoundException()
        {
            //
        }

        public SourceDirectoryNotFoundException(string message) : base(message)
        {
            //
        }

        public SourceDirectoryNotFoundException(string message, Exception inner) : base(message, inner)
        {
            //
        }
    }
}
