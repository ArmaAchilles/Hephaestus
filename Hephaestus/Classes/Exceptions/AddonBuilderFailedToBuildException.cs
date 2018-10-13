using System;
using Hephaestus.Classes.Exceptions;

namespace Hephaestus.Classes
{
    internal class AddonBuilderFailedToBuildException : ExceptionBase
    {
        public AddonBuilderFailedToBuildException()
        {
        }

        public AddonBuilderFailedToBuildException(string message) : base(message)
        {
        }

        public AddonBuilderFailedToBuildException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}