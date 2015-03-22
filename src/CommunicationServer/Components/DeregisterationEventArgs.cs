using System;

namespace _15pl04.Ucc.CommunicationServer
{
    internal class DeregisterationEventArgs : EventArgs
    {
        public ulong Id { get; set; }
        public ComponentInfo ComponentInfo { get; set; }
    }
}
