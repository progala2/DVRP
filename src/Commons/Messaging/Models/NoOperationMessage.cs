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

        [XmlArray(Order = 0)]
        [XmlArrayItem("BackupCommunicationServer", IsNullable = true)]
        public List<BackupServerInfo> BackupServers { get; set; }

        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.NoOperation; }
        }

        public NoOperationMessage()
        {
            BackupServers = new List<BackupServerInfo>();
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" BackupServers={");
            foreach (var backup in BackupServers)
            {
                builder.Append(backup.IpAddress);
                builder.Append(":");
                builder.Append(backup.Port.ToString("0000"));
                builder.Append("|");
            }
            builder.Append("}");

            return builder.ToString();
        }
    }
}
