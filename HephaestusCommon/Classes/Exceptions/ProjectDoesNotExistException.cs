using System;

namespace HephaestusCommon.Classes.Exceptions
{
    public class ProjectDoesNotExistException : Exception
    {
        public ProjectDoesNotExistException()
        {
        }

        public ProjectDoesNotExistException(string message) : base(message)
        {
        }

        public ProjectDoesNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}