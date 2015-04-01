using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class BackupServerInfo
    {
        [XmlAttribute("address", DataType = "anyURI")]
        public string IpAddress { get; set; }

        [XmlAttribute("port")]
        public ushort Port { get; set; }
    }
}
