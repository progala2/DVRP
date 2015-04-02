using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class ServerInfo
    {
        [XmlAttribute("address", DataType = "anyURI")]
        public string IpAddress { get; set; }

        [XmlAttribute("port")]
        public ushort Port { get; set; }


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
