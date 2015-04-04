using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Messaging;
using System;

namespace _15pl04.Ucc.CommunicationServer
{
    internal class CommunicationServer
    {
        public ServerConfig Config { get; private set; }

        public CommunicationServer(ServerConfig config)
        {
            Config = config;
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void SwitchToPrimaryMode()
        {

        }
    }
}
