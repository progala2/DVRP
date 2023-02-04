using System;
using Dvrp.Ucc.Commons.Messaging.Models.Base;

namespace Dvrp.Ucc.Commons.Messaging
{
    /// <summary>
    /// Represents the event arguments containing message and exception.
    /// </summary>
    public class MessageExceptionEventArgs : MessageEventArgs
    {
	    /// <summary>
	    /// Creates a new instance of the MessageExceptionEventArgs class using the specified <paramref name="message"/> and
	    /// <paramref name="exception"/>.
	    /// </summary>
	    /// <param name="message"></param>
	    /// <param name="exception">The exception which is event argument.</param>
	    public MessageExceptionEventArgs(Message message, Exception exception)
            : base(message)
        {
            Exception = exception;
        }

        /// <summary>
        /// The exception which is event argument.
        /// </summary>
        public Exception Exception { get; }
    }
}