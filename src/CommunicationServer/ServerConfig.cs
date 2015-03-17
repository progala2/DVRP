using System.Net;

namespace _15pl04.Ucc.CommunicationServer
{
    class ServerConfig
    {
        // configuration options goes here

        public ServerMode Mode { get; set; }

        public IPEndPoint Address { get; set; }

        // ...
    }
}
