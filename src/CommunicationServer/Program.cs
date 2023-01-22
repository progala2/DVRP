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
                var config = ServerConfigurationSection.LoadConfig("serverConfig", args);

                var serverAddress = IpEndPointParser.Parse(config.Address, config.Port);
                var timeout = config.Timeout;
                var isBackup = config.IsBackup;

                /*IPEndPoint masterServerAddress;
                if (isBackup)
                    masterServerAddress = IpEndPointParser.Parse(config.MasterServer.Address, config.MasterServer.Port);*/

                //_logger.Info("Server address: " + serverAddress);

                var serverConfig = new ServerConfig(isBackup ? ServerConfig.ServerMode.Backup : ServerConfig.ServerMode.Primary, serverAddress, timeout);

                communicationServer = new CommunicationServer(serverConfig);
            }
            catch (Exception ex)
            {
                var errorText = $"{ex.GetType().FullName}:{ex.Message}";
                if (ex.InnerException != null)
                    errorText += $"|({ex.InnerException.GetType().FullName}:{ex.InnerException.Message})";
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