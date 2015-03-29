using System;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Messaging
{
    public delegate void MessageEventHandler(object sender, MessageEventArgs e);


    public class MessageEventArgs : EventArgs
    {
        public Message Message { get; private set; }

        public MessageEventArgs(Message message)
        {
            Message = message;
        }
    }
}
