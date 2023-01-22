using System;
using System.Configuration;

namespace _15pl04.Ucc.Commons.Config
{
    /// <summary>
    ///     Configuration section for components connecting to the server.
    /// </summary>
    public class ComponentConfigurationSection : ConfigurationSection
    {
        private const string PrimaryServerPropertyString = "primaryServer";
        private const string TaskSolversPathPropertyString = "taskSolversPath";

        /// <summary>
        ///     The address of the primary server.
        /// </summary>
        [ConfigurationProperty(PrimaryServerPropertyString)]
        public IPEndPointConfigurationElement PrimaryServer
        {
            get => (IPEndPointConfigurationElement)this[PrimaryServerPropertyString];
            set => this[PrimaryServerPropertyString] = value;
        }

        /// <summary>
        ///     The path to directory contains task solvers.
        /// </summary>
        [ConfigurationProperty(TaskSolversPathPropertyString), DefaultSettingValue("")]
        public string TaskSolversPath
        {
            get => (string)this[TaskSolversPathPropertyString];
            set => this[TaskSolversPathPropertyString] = value;
        }

        /// <summary>
        ///     The information wheter this configuration section is readonly.
        /// </summary>
        public override bool IsReadOnly()
        {
            return false;
        }

        /// <summary>
        ///     Loads component configuration section by name using configuration manager. Also tries to override
        ///     them with command line parameters.
        /// </summary>
        /// <param name="componentConfigurationSectionName">The name of configuration section to load parameters from.</param>
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
        public static ComponentConfigurationSection LoadConfig(string componentConfigurationSectionName,
            string[] commandLineParameters)
        {
            var config =
                (ComponentConfigurationSection)ConfigurationManager.GetSection(componentConfigurationSectionName);

            if (config == null)
                throw new InvalidOperationException($"Section {componentConfigurationSectionName} does not exists.");

            // possible overriting with command line parameters
            if (commandLineParameters is { Length: > 0 })
            {
                var n = commandLineParameters.Length;
                var i = 0;
                Exception? exception = null;
                try
                {
                    if (n - i >= 2 && commandLineParameters[i].ToLower() == "-address")
                    {
                        config.PrimaryServer.Address = commandLineParameters[i + 1];
                        i += 2;
                    }
                    if (n - i >= 2 && commandLineParameters[i].ToLower() == "-port")
                    {
                        config.PrimaryServer.Port = int.Parse(commandLineParameters[i + 1]);
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
            Console.WriteLine("Usage: {0} [-address [IPv4 address or IPv6 address or host name]] [-port [port number]]",
                AppDomain.CurrentDomain.FriendlyName);
        }
    }
}