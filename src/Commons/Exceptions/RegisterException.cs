using System;
using System.Runtime.Serialization;

namespace _15pl04.Ucc.Commons.Exceptions
{
    /// <summary>
    /// Description of RegisterException
    /// </summary>
    public class RegisterException : Exception, ISerializable
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
