using System;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Messaging
{
    public class MessageExceptionEventArgs : MessageEventArgs
    {
        public MessageExceptionEventArgs(Message message, Exception exception)
            : base(message)
        {
            Exception = exception;
        }

        public Exception Exception { get; private set; }
    }
}