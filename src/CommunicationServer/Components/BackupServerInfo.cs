using System.Net;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.CommunicationServer.Components.Base;

namespace _15pl04.Ucc.CommunicationServer.Components
{
    public class BackupServerInfo : ComponentInfo
    {
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

        public IPEndPoint Address { get; private set; }
    }
}