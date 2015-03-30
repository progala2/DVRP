using System;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Messaging
{
    public class MessageExceptionEventArgs : MessageEventArgs
    {
        public Exception Exception { get; private set; }

        public MessageExceptionEventArgs(Message message, Exception exception)
            : base(message)
        {
            Exception = exception;
        }
    }
}
