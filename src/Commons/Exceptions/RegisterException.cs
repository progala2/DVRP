using System;
using System.Runtime.Serialization;

namespace _15pl04.Ucc.Commons.Exceptions
{
    public class RegisterException : Exception
    {
        public RegisterException()
            : base()
        {
        }

        public RegisterException(string message)
            : base(message)
        {
        }

        public RegisterException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // This constructor is needed for serialization.
        protected RegisterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
