using System;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Messaging
{
    /// <summary>
    /// Represents the event arguments containing message.
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the MessageEventArgs class using the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message which is event argument.</param>
        public MessageEventArgs(Message message)
        {
            Message = message;
        }

        /// <summary>
        /// The message which is event argument.
        /// </summary>
        public Message Message { get; }
    }
}