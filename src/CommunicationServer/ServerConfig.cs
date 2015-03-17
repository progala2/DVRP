using System.Net;

namespace _15pl04.Ucc.CommunicationServer
{
    public enum ServerMode
    {
        Primary = 1,
        Backup = 2
    }

    class ServerConfig
    {
        // configuration options goes here

        public ServerMode Mode { get; set; }

        public IPEndPoint Address { get; set; }

        // ...
    }
}
