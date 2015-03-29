using System;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Computations
{
    public delegate void MessageHandlingExceptionHandler(object sender, MessageHandlingExceptionEventArgs e);


    public class MessageHandlingExceptionEventArgs : MessageEventArgs
    {
        public Exception Exception { get; private set; }

        public MessageHandlingExceptionEventArgs(Message message, Exception exception)
            : base(message)
        {
            Exception = exception;
        }
    }
}
