using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Dvrp.Ucc.Commons.Components
{
    /// <summary>
    /// Server-specific information.
    /// </summary>
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class ServerInfo
    {
        /// <summary>
        /// Server IP address.
        /// </summary>
        [XmlAttribute("address", DataType = "anyURI")]
        public string IpAddress { get; set; }
        /// <summary>
        /// Listening port.
        /// </summary>
        [XmlAttribute("port")]
        public ushort Port { get; set; }

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>String value that represents this object.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(IpAddress);
            builder.Append(":");
            builder.Append(Port.ToString("0000"));

            return builder.ToString();
        }
    }
}