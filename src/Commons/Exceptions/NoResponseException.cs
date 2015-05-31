using System;
using System.Runtime.Serialization;

namespace _15pl04.Ucc.Commons.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when there is no response from the server.
    /// </summary>
    public class NoResponseException : Exception
    {
        public NoResponseException()
        {
        }

        public NoResponseException(string message)
            : base(message)
        {
        }

        public NoResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // This constructor is needed for serialization.
        protected NoResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}