using System.Configuration;
using System.Net;

namespace _15pl04.Ucc.Commons.Config
{
    /// <summary>
    ///     Configuration element representing end point address.
    /// </summary>
    public class IPEndPointConfigurationElement : ConfigurationElement
    {
        private const string AddressPropertyString = "address";
        private const string PortPropertyString = "port";

        /// <summary>
        ///     The address as IPv4/IPv6/host name.
        /// </summary>
        [ConfigurationProperty(AddressPropertyString), DefaultSettingValue("127.0.0.1")]
        public string Address
        {
            get => (string)this[AddressPropertyString];
            set => this[AddressPropertyString] = value;
        }
        /// <summary>
        ///     The port.
        /// </summary>
        [ConfigurationProperty(PortPropertyString), DefaultSettingValue("12345")]
        [IntegerValidator(ExcludeRange = false, MinValue = IPEndPoint.MinPort, MaxValue = IPEndPoint.MaxPort)]
        public int Port
        {
            get => (int)this[PortPropertyString];
            set => this[PortPropertyString] = value;
        }

        /// <summary>
        ///     The information wheter this configuration element is readonly.
        /// </summary>
        public override bool IsReadOnly()
        {
            return false;
        }
    }
}