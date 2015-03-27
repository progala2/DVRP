using _15pl04.Ucc.Commons.Messaging.Models;
using System;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    internal class MessageReceptionEventArgs : EventArgs
    {
        public Message Message { get; set; }
    }
}
