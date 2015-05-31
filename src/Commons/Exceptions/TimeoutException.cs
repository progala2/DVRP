using System;
using System.Runtime.Serialization;

namespace _15pl04.Ucc.Commons.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when communication timout has been reached.
    /// </summary>
    public class TimeoutException : Exception
    {
        public TimeoutException()
        {
        }

        public TimeoutException(string message)
            : base(message)
        {
        }

        public TimeoutException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // This constructor is needed for serialization.
        protected TimeoutException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}