using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.Components.Base;
using System.Net;

namespace _15pl04.Ucc.CommunicationServer.Components
{
    public class BackupServerInfo : ComponentInfo
    {
        public IPEndPoint Address { get; private set; }

        public BackupServerInfo(ServerInfo serverInfo, int numberOfThreads)
            : base(ComponentType.CommunicationServer, numberOfThreads)
        {
            Address = new IPEndPoint(
                IPAddress.Parse(serverInfo.IpAddress), 
                serverInfo.Port);
        }

        public BackupServerInfo(IPEndPoint endPoint, int numberOfThreads)
            : base(ComponentType.CommunicationServer, numberOfThreads)
        {
            Address = endPoint;
        }
    }
}
