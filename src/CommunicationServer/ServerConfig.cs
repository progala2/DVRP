using System.Collections.Specialized;
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

            NameValueCollection appSettings = ConfigurationManager.AppSettings;

            //int listeningPort = int.Parse(appSettings["listeningPort"]);
            //int backupMode = int.Parse(appSettings["backupMode"]);
           // uint timeout = uint.Parse(appSettings["timeout"]);
            int backupMode = 0;
            uint timeout = 0;


            Address = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 0);
            if (backupMode == 0)
                Mode = ServerMode.Primary;
            else
                Mode = ServerMode.Backup;
            CommunicationTimeout = timeout;
        }

        public ServerMode Mode { get; set; }

        public IPEndPoint Address { get; set; }

        public uint CommunicationTimeout { get; set; }
    }
}
