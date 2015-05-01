using System.Net;
using _15pl04.Ucc.CommunicationServer.Messaging.Base;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    public class TcpDataProviderMetadata : Metadata
    {
        public IPEndPoint SenderAddress { get; set; }
    }
}