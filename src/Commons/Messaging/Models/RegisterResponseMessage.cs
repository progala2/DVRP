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
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "RegisterResponse")]
    public class RegisterResponseMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "RegisterResponse.xsd";


        [XmlElement(Order = 0, ElementName = "Id")]
        public ulong AssignedId { get; set; }

        [XmlElement(Order = 1, ElementName = "Timeout")]
        public uint CommunicationTimeout { get; set; }

        [XmlArray(Order = 2, ElementName = "BackupCommunicationServers")]
        [XmlArrayItem("BackupCommunicationServer")]
        public List<BackupServerInfo> BackupServers { get; set; }

        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.RegisterResponse; }
        }



        public RegisterResponseMessage()
        {
            BackupServers = new List<BackupServerInfo>();
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" Id=" + AssignedId);
            builder.Append("|Timeout" + CommunicationTimeout);

            builder.Append("|BackupServers={");
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
