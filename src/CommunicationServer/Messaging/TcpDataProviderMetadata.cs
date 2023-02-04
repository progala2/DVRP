using System.Net;
using Dvrp.Ucc.CommunicationServer.Messaging.Base;

namespace Dvrp.Ucc.CommunicationServer.Messaging
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