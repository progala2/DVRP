using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Messaging;
using System;

namespace _15pl04.Ucc.CommunicationServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = new ServerConfig(args);

            var communicationServer = new CommunicationServer(config);
        }
    }
}
