using System;
using System.Runtime.Serialization;

namespace Hephaestus.Classes.Exceptions
{
    public abstract class ExceptionBase : Exception
    {
        public ExceptionBase()
        {
        }

        public ExceptionBase(string message) : base(message)
        {
            Console.Error.WriteLine(message);
        }

        public ExceptionBase(string message, Exception innerException) : base(message, innerException)
        {
            Console.Error.WriteLine(message);
        }
    }
}