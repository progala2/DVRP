using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    /// <summary>
    /// Object representation of the No Oper message.
    /// </summary>
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "NoOperation")]
    public class NoOperationMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation",
            Namespace = "http://www.w3.org/2001/XMLSchema-instance")] public string NoNamespaceSchemaLocation =
                "NoOperation.xsd";

        public NoOperationMessage()
        {
            BackupServers = new List<ServerInfo>();
        }

        [XmlArray(Order = 0, ElementName = "BackupCommunicationServers")]
        [XmlArrayItem("BackupCommunicationServer", IsNullable = true)]
        public List<ServerInfo> BackupServers { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.NoOperation; }
        }

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>String value that represents this object.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" BackupServers{");
            builder.Append(string.Join(",", BackupServers));
            builder.Append("}");

            return builder.ToString();
        }
    }
}