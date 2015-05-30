using System.Net;
using _15pl04.Ucc.CommunicationServer.Messaging.Base;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    /// <summary>
    /// Information about received data and TCP end point that provided it.
    /// </summary>
    public class TcpDataProviderMetadata : Metadata
    {
        /// <summary>
        /// Address of the end point that sent the data.
        /// </summary>
        public IPEndPoint SenderAddress { get; set; }
    }
}