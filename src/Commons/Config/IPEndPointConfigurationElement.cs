using System.Configuration;
using System.Net;

namespace _15pl04.Ucc.Commons.Config
{
    public class IPEndPointConfigurationElement : ConfigurationElement
    {
        private const string AddressPropertyString = "address";
        private const string PortPropertyString = "port";

        [ConfigurationProperty(AddressPropertyString), DefaultSettingValue("127.0.0.1")]
        public string Address
        {
            get { return (string) this[AddressPropertyString]; }
            set { this[AddressPropertyString] = value; }
        }

        [ConfigurationProperty(PortPropertyString), DefaultSettingValue("12345")]
        [IntegerValidator(ExcludeRange = false, MinValue = IPEndPoint.MinPort, MaxValue = IPEndPoint.MaxPort)]
        public int Port
        {
            get { return (int) this[PortPropertyString]; }
            set { this[PortPropertyString] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}