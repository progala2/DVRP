using System;
using System.Configuration;

namespace _15pl04.Ucc.Commons.Config
{
    public class ComponentConfigurationSection : ConfigurationSection
    {
        private const string PrimaryServerPropertyString = "primaryServer";
        private const string TaskSolversPathPropertyString = "taskSolversPath";

        [ConfigurationProperty(PrimaryServerPropertyString)]
        public IPEndPointConfigurationElement PrimaryServer
        {
            get { return (IPEndPointConfigurationElement) this[PrimaryServerPropertyString]; }
            set { this[PrimaryServerPropertyString] = value; }
        }

        [ConfigurationProperty(TaskSolversPathPropertyString), DefaultSettingValue("")]
        public string TaskSolversPath
        {
            get { return (string) this[TaskSolversPathPropertyString]; }
            set { this[TaskSolversPathPropertyString] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public static ComponentConfigurationSection LoadConfig(string componentConfigurationSectionName,
            string[] commandLineParameters)
        {
            var config =
                (ComponentConfigurationSection) ConfigurationManager.GetSection(componentConfigurationSectionName);

            // possible overriting with command line parameters
            if (commandLineParameters != null && commandLineParameters.Length > 0)
            {
                var n = commandLineParameters.Length;
                var i = 0;
                Exception exception = null;
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

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: {0} [-address [IPv4 address or IPv6 address or host name]] [-port [port number]]",
                AppDomain.CurrentDomain.FriendlyName);
        }
    }
}