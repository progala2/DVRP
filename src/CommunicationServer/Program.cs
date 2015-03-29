using System;
using _15pl04.Ucc.Commons;

namespace _15pl04.Ucc.CommunicationServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = new ServerConfig(args);

            config.Address = IPEndPointParser.Parse("127.0.0.1:12345");
            config.CommunicationTimeout = 5000;
            config.Mode = ServerConfig.ServerMode.Primary;

            var communicationServer = new CommunicationServer(config);

            communicationServer.Start();

            while (Console.ReadLine() != "exit")
            {
                // input handling
            }

            communicationServer.Stop();
        }
    }
}
