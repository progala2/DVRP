using _15pl04.Ucc.Commons.Messaging.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "NoOperation")]
    public class NoOperationMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "NoOperation.xsd";


        public NoOperationMessage()
        {
            BackupCommunicationServers = new List<BackupCommunicationServer>();
        }

        [XmlArray(Order = 0)]
        [XmlArrayItem("BackupCommunicationServer", IsNullable = true)]
        public List<BackupCommunicationServer> BackupCommunicationServers { get; set; }

        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.NoOperation; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            sb.Append("BackupCommunicationServers={");
            foreach (var backup in BackupCommunicationServers)
                sb.Append(backup.ToString());
            sb.Append("}]");
            return sb.ToString();
        }
    }

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class BackupCommunicationServer
    {
        [XmlAttribute("address", DataType = "anyURI")]
        public string IpAddress { get; set; }

        [XmlAttribute("port")]
        public ushort Port { get; set; }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            sb.Append("Address=" + IpAddress + ";");
            sb.Append("Port=" + Port);
            sb.Append("]");
            return sb.ToString();
        }
    }
}
