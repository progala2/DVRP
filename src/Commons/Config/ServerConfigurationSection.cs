using System;
using System.Configuration;
using System.Net;

namespace _15pl04.Ucc.Commons.Config
{
    public class ServerConfigurationSection : ConfigurationSection
    {
        private const string AddressPropertyString = "address";
        private const string PortPropertyString = "port";
        private const string TimeoutPropertyString = "timeout";
        private const string IsBackupPropertyString = "isBackup";
        private const string MasterServerPropertyString = "masterServer";

        [ConfigurationProperty(AddressPropertyString), DefaultSettingValue("127.0.0.1")]
        public string Address
        {
            get { return (string)this[AddressPropertyString]; }
            set { this[AddressPropertyString] = value; }
        }

        [ConfigurationProperty(PortPropertyString), DefaultSettingValue("12345")]
        [IntegerValidator(ExcludeRange = false, MinValue = IPEndPoint.MinPort, MaxValue = IPEndPoint.MaxPort)]
        public int Port
        {
            get { return (int)this[PortPropertyString]; }
            set { this[PortPropertyString] = value; }
        }

        [ConfigurationProperty(TimeoutPropertyString)]
        public uint Timeout
        {
            get { return (uint)this[TimeoutPropertyString]; }
            set { this[TimeoutPropertyString] = value; }
        }

        [ConfigurationProperty(IsBackupPropertyString)]
        public bool IsBackup
        {
            get { return (bool)this[IsBackupPropertyString]; }
            set { this[IsBackupPropertyString] = value; }
        }

        [ConfigurationProperty(MasterServerPropertyString, IsRequired = false)]
        public IPEndPointConfigurationElement MasterServer
        {
            get { return (IPEndPointConfigurationElement)this[MasterServerPropertyString]; }
            set { this[MasterServerPropertyString] = value; }
        }


        public override bool IsReadOnly()
        {
            return false;
        }



        public static ServerConfigurationSection LoadConfig(string serverConfigurationSectionName,
            string[] commandLineParameters)
        {
            var config = (ServerConfigurationSection)ConfigurationManager.GetSection(serverConfigurationSectionName);

            // possible overriting with command line parameters
            if (commandLineParameters != null && commandLineParameters.Length > 0)
            {
                int n = commandLineParameters.Length;
                int i = 0;
                Exception exception = null;
                try
                {
                    if (n - i >= 2 && commandLineParameters[i].ToLower() == "-port")
                    {
                        config.Port = int.Parse(commandLineParameters[i + 1]);
                        i += 2;
                    }
                    if (n - i >= 5
                        && commandLineParameters[i].ToLower() == "-backup"
                        && commandLineParameters[i + 1].ToLower() == "-maddress"
                        && commandLineParameters[i + 3].ToLower() == "-mport")
                    {
                        config.IsBackup = true;
                        config.MasterServer.Address = commandLineParameters[i + 2];
                        config.MasterServer.Port = int.Parse(commandLineParameters[i + 4]);
                        i += 5;
                    }
                    if (n - i >= 2 && commandLineParameters[i].ToLower() == "-t")
                    {
                        config.Timeout = uint.Parse(commandLineParameters[i + 1]);
                        i += 2;
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                if (exception != null || i != n)
                {
                    PrintUsage();
                    throw new InvalidOperationException("Incorrect command line parameters.", exception);
                }
            }

            return config;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: {0} [-port [port number]] [-backup –maddress [IPv4 address or IPv6 address or host name] –mport [portNumber]] [-t [time in seconds]]",
                AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
