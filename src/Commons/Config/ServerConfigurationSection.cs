using System;
using System.Configuration;
using System.Net;

namespace Dvrp.Ucc.Commons.Config
{
    /// <summary>
    ///     Configuration section for server.
    /// </summary>
    public class ServerConfigurationSection : ConfigurationSection
    {
        private const string AddressPropertyString = "address";
        private const string PortPropertyString = "port";
        private const string TimeoutPropertyString = "timeout";
        private const string IsBackupPropertyString = "isBackup";
        private const string MasterServerPropertyString = "masterServer";

        /// <summary>
        ///     The address of the server. It is IPv4/IPv6/host name.
        /// </summary>
        [ConfigurationProperty(AddressPropertyString), DefaultSettingValue("127.0.0.1")]
        public string Address
        {
            get => (string)this[AddressPropertyString];
            set => this[AddressPropertyString] = value;
        }

        /// <summary>
        ///     The port of the server.
        /// </summary>
        [ConfigurationProperty(PortPropertyString), DefaultSettingValue("12345")]
        [IntegerValidator(ExcludeRange = false, MinValue = IPEndPoint.MinPort, MaxValue = IPEndPoint.MaxPort)]
        public int Port
        {
            get => (int)this[PortPropertyString];
            set => this[PortPropertyString] = value;
        }

        /// <summary>
        ///     Timeout for communication with server clients.
        /// </summary>
        [ConfigurationProperty(TimeoutPropertyString)]
        public uint Timeout
        {
            get => (uint)this[TimeoutPropertyString];
            set => this[TimeoutPropertyString] = value;
        }

        /// <summary>
        ///     The information wheter server is in backup mode.
        /// </summary>
        [ConfigurationProperty(IsBackupPropertyString)]
        public bool IsBackup
        {
            get => (bool)this[IsBackupPropertyString];
            set => this[IsBackupPropertyString] = value;
        }

        /// <summary>
        ///     The master server address.
        /// </summary>
        [ConfigurationProperty(MasterServerPropertyString, IsRequired = false)]
        public IpEndPointConfigurationElement MasterServer
        {
            get => (IpEndPointConfigurationElement)this[MasterServerPropertyString];
            set => this[MasterServerPropertyString] = value;
        }

        /// <summary>
        ///     The information wheter this configuration section is readonly.
        /// </summary>
        public override bool IsReadOnly()
        {
            return false;
        }

        /// <summary>
        ///     Loads server configuration section by name using configuration manager. Also tries to override
        ///     them with command line parameters.
        /// </summary>
        /// <param name="serverConfigurationSectionName">The name of configuration section to load parameters from.</param>
        /// <param name="commandLineParameters">
        ///     The optional command line parameters. They should be in form:
        ///     [-address [IPv4 address or IPv6 address or host name]] [-port [port number]]
        /// </param>
        /// <returns> A component configuration section with loaded parameters.</returns>
        /// <exception cref="System.Configuration.ConfigurationErrorsException"> Thrown when a configuration file could not be loaded.</exception>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown when either section with given name does not exists or if given non empty command line parameters
        ///     have incorrect form.
        /// </exception>
        public static ServerConfigurationSection LoadConfig(string serverConfigurationSectionName,
            string[] commandLineParameters)
        {
            var config = (ServerConfigurationSection)ConfigurationManager.GetSection(serverConfigurationSectionName);

            if (config == null)
                throw new InvalidOperationException($"Section {serverConfigurationSectionName} does not exists.");

            // possible overriting with command line parameters
            if (commandLineParameters is { Length: > 0 })
            {
                var n = commandLineParameters.Length;
                var i = 0;
                Exception? exception = null;
                try
                {
                    if (n >= 2 && commandLineParameters[i].ToLower() == "-port")
                    {
                        config.Port = int.Parse(commandLineParameters[1]);
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

        /// <summary>
        /// Prints console line usage.
        /// </summary>
        private static void PrintUsage()
        {
            Console.WriteLine(
                "Usage: {0} [-port [port number]] [-backup –maddress [IPv4 address or IPv6 address or host name] –mport [portNumber]] [-t [time in seconds]]",
                AppDomain.CurrentDomain.FriendlyName);
        }
    }
}