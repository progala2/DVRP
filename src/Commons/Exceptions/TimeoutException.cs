using System;
using System.Runtime.Serialization;

namespace _15pl04.Ucc.Commons.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when communication timeout has been reached.
    /// </summary>
    public class TimeoutException : Exception
    {
	    /// <inheritdoc />
	    public TimeoutException()
        {
        }

	    /// <inheritdoc />
	    public TimeoutException(string message)
            : base(message)
        {
        }

	    /// <inheritdoc />
	    public TimeoutException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// This constructor is needed for serialization.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected TimeoutException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}