using System.Net;

namespace _15pl04.Ucc.CommunicationServer.Components
{
    public class BackupServerInfo : ComponentInfo
    {
        public IPEndPoint Address { get; set; }

        public BackupServerInfo(IPEndPoint address)
            : base(Commons.ComponentType.CommunicationServer)
        {
            Address = address;
        }

    }
}
