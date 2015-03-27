using System;
using System.Runtime.Serialization;

namespace _15pl04.Ucc.Commons.Exceptions
{
    public class TimeoutException : Exception
    {
        public TimeoutException()
            : base()
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
