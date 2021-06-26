using System;

namespace DoberDogBot.Domain.Exceptions
{
    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    public class StreamerDomainException : Exception
    {
        public StreamerDomainException()
        { }

        public StreamerDomainException(string message)
            : base(message)
        { }

        public StreamerDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
