using System;
using System.Runtime.Serialization;

namespace _15pl04.Ucc.Commons.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when there is no response from the server.
    /// </summary>
    public class NoResponseException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public NoResponseException()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public NoResponseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public NoResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// This constructor is needed for serialization.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected NoResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}