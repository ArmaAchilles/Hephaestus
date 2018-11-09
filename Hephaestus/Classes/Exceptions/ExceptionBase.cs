using System;

namespace Hephaestus.Classes.Exceptions
{
    public class ExceptionBase : Exception
    {
        protected ExceptionBase()
        {
        }

        protected ExceptionBase(string message) : base(message)
        {
            Console.Error.WriteLine(message);
        }

        protected ExceptionBase(string message, Exception innerException) : base(message, innerException)
        {
            Console.Error.WriteLine(message);
        }
    }
}