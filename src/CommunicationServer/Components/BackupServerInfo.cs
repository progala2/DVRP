using System.Net;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.CommunicationServer.Components.Base;

namespace _15pl04.Ucc.CommunicationServer.Components
{
    /// <summary>
    /// Information about backup server.
    /// </summary>
    public class BackupServerInfo : ComponentInfo
    {
        /// <summary>
        /// Creates BackupServerInfo instance.
        /// </summary>
        /// <param name="serverInfo">Communication Server specific information.</param>
        /// <param name="numberOfThreads">Number of threads provided by the backup.</param>
        public BackupServerInfo(ServerInfo serverInfo, int numberOfThreads)
            : base(ComponentType.CommunicationServer, numberOfThreads)
        {
            Address = new IPEndPoint(
                IPAddress.Parse(serverInfo.IpAddress),
                serverInfo.Port);
        }

        /// <summary>
        /// Creates BackupServerInfo instance.
        /// </summary>
        /// <param name="endPoint">IPEndPoint data of the backup.</param>
        /// <param name="numberOfThreads">Number of threads provided by the backup.</param>
        public BackupServerInfo(IPEndPoint endPoint, int numberOfThreads)
            : base(ComponentType.CommunicationServer, numberOfThreads)
        {
            Address = endPoint;
        }

        /// <summary>
        /// Backup server's address.
        /// </summary>
        public IPEndPoint Address { get; private set; }
    }
}