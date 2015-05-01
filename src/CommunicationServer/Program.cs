using System;
using System.Configuration;
using System.Net;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Utilities;

namespace _15pl04.Ucc.CommunicationServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = new ServerConfig(args);

            var appSettings = ConfigurationManager.AppSettings;
            var address = "127.0.0.1"; //Dns.GetHostName();
            config.Address = IPEndPointParser.Parse(address, appSettings["listeningPort"]);
            config.CommunicationTimeout = uint.Parse(appSettings["timeout"]);
            config.Mode = ServerConfig.ServerMode.Primary;

            Console.WriteLine("Server address: " + config.Address.ToString());
            Console.WriteLine("Timeout: " + config.CommunicationTimeout);

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
