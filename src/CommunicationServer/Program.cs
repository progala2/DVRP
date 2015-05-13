using System;
using System.Net;
using _15pl04.Ucc.Commons.Config;
using _15pl04.Ucc.Commons.Utilities;

namespace _15pl04.Ucc.CommunicationServer
{
    public class Program
    {
        private static void Main(string[] args)
        {
            CommunicationServer communicationServer;
            try
            {
                ServerConfigurationSection config = ServerConfigurationSection.LoadConfig("serverConfig", args);

                IPEndPoint serverAddress = IpEndPointParser.Parse(config.Address, config.Port);
                uint timeout = config.Timeout;
                bool isBackup = config.IsBackup;

                IPEndPoint masterServerAddress;
                if (isBackup)
                    masterServerAddress = IpEndPointParser.Parse(config.MasterServer.Address, config.MasterServer.Port);

                //_logger.Info("Server address: " + serverAddress);

                var serverConfig = new ServerConfig()
                {
                    Mode = isBackup ? ServerConfig.ServerMode.Backup : ServerConfig.ServerMode.Primary,
                    Address = serverAddress,
                    CommunicationTimeout = timeout
                };

                communicationServer = new CommunicationServer(serverConfig);
            }
            catch (Exception ex)
            {
                var errorText = string.Format("{0}:{1}", ex.GetType().FullName, ex.Message);
                if (ex.InnerException != null)
                    errorText += string.Format("|({0}:{1})", ex.InnerException.GetType().FullName, ex.InnerException.Message);
                //_logger.Error(errorText);
                return;
            }

            communicationServer.Start();

            while (Console.ReadLine() != "exit")
            {
                // input handling
            }

            communicationServer.Stop();
        }
    }
}