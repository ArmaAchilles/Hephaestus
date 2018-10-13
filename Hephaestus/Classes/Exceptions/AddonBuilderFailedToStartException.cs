using System;

namespace Hephaestus.Classes.Exceptions
{
    public class AddonBuilderFailedToStartException : ExceptionBase
    {
        public AddonBuilderFailedToStartException()
        {
        }

        public AddonBuilderFailedToStartException(string message) : base(message)
        {
        }

        public AddonBuilderFailedToStartException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}