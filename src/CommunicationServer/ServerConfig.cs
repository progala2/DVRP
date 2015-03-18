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
            /*
             * Ustalenie konfiguracji na podstawie App.config i argumentów z konsoli.
             */ 
        }

        public ServerMode Mode { get; set; }

        public IPEndPoint Address { get; set; }

        public ulong Timeout { get; set; }
    }
}
