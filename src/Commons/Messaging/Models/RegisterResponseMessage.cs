using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "RegisterResponse")]
    public class RegisterResponseMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation",
            Namespace = "http://www.w3.org/2001/XMLSchema-instance")] public string NoNamespaceSchemaLocation =
                "RegisterResponse.xsd";

        public RegisterResponseMessage()
        {
            BackupServers = new List<ServerInfo>();
        }

        [XmlElement(Order = 0, ElementName = "Id")]
        public ulong AssignedId { get; set; }

        [XmlElement(Order = 1, ElementName = "Timeout")]
        public uint CommunicationTimeout { get; set; }

        [XmlArray(Order = 2, ElementName = "BackupCommunicationServers")]
        [XmlArrayItem("BackupCommunicationServer")]
        public List<ServerInfo> BackupServers { get; set; }

        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.RegisterResponse; }
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" AssignedId(" + AssignedId + ")");
            builder.Append(" Timeout(" + CommunicationTimeout + ")");

            builder.Append(" BackupServers{");
            builder.Append(string.Join(",", BackupServers));
            ;
            builder.Append("}");

            return builder.ToString();
        }
    }
}