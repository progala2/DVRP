using System;
using System.Runtime.Serialization;

namespace _15pl04.Ucc.Commons.Exceptions
{
    /// <summary>
    /// Description of NoResponseException
    /// </summary>
    public class NoResponseException : Exception, ISerializable
    {
        public NoResponseException()
            : base()
        {
        }

        public NoResponseException(string message)
            : base(message)
        {
        }

        public NoResponseException(string message, NoResponseException innerException)
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