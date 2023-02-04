using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using Dvrp.Ucc.Commons.Components;
using Dvrp.Ucc.Commons.Messaging.Models.Base;

namespace Dvrp.Ucc.Commons.Messaging.Models
{
    /// <summary>
    /// Object representation of the Register Response message.
    /// </summary>
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "RegisterResponse")]
    public class RegisterResponseMessage : Message
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation = "RegisterResponse.xsd";

        /// <summary>
        /// Creates RegisterResponseMessage instance.
        /// </summary>
        public RegisterResponseMessage()
        {
            BackupServers = new List<ServerInfo>();
        }

        /// <summary>
        /// The ID assigned by the Communication Server.
        /// </summary>
        [XmlElement(Order = 0, ElementName = "Id")]
        public ulong AssignedId { get; set; }

        /// <summary>
        /// The communication timeout configured on Communication Server.
        /// </summary>
        [XmlElement(Order = 1, ElementName = "Timeout")]
        public uint CommunicationTimeout { get; set; }

        /// <summary>
        /// The information about backup servers.
        /// </summary>
        [XmlArray(Order = 2, ElementName = "BackupCommunicationServers")]
        [XmlArrayItem("BackupCommunicationServer")]
        public List<ServerInfo> BackupServers { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public override MessageClass MessageType => MessageClass.RegisterResponse;

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>String value that represents this object.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" AssignedId(" + AssignedId + ")");
            builder.Append(" Timeout(" + CommunicationTimeout + ")");

            builder.Append(" BackupServers{");
            builder.Append(string.Join(",", BackupServers));

            builder.Append('}');

            return builder.ToString();
        }
    }
}