using System.Configuration;
using System.Net;

namespace _15pl04.Ucc.CommunicationServer
{
    public class ServerConfig
    {
        public enum ServerMode
        {
            Primary = 1,
            Backup = 2
        }

        public ServerConfig(string[] consoleArgs)
        {
            // TODO ! proper constructor

            var appSettings = ConfigurationManager.AppSettings;

            //int listeningPort = int.Parse(appSettings["listeningPort"]);
            //int backupMode = int.Parse(appSettings["backupMode"]);
            // uint timeout = uint.Parse(appSettings["timeout"]);
            var backupMode = 0;
            uint timeout = 0;


            Address = new IPEndPoint(new IPAddress(new byte[] {127, 0, 0, 1}), 0);
            Mode = backupMode == 0 ? ServerMode.Primary : ServerMode.Backup;
            CommunicationTimeout = timeout;
        }

        public ServerMode Mode { get; set; }
        public IPEndPoint Address { get; set; }
        public uint CommunicationTimeout { get; set; }
    }
}