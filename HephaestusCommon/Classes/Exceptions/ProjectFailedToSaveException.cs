using System;

namespace HephaestusCommon.Classes.Exceptions
{
    public class ProjectFailedToSaveException : Exception
    {
        public ProjectFailedToSaveException()
        {
        }

        public ProjectFailedToSaveException(string message) : base(message)
        {
        }

        public ProjectFailedToSaveException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}