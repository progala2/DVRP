﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using Dvrp.Ucc.Commons.Components;
using Dvrp.Ucc.Commons.Messaging.Models.Base;

namespace Dvrp.Ucc.Commons.Messaging.Models
{
    /// <summary>
    /// Object representation of the No Operation message.
    /// </summary>
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "NoOperation")]
    public class NoOperationMessage : Message
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation = "NoOperation.xsd";

        /// <summary>
        /// Creates NoOperationMessage instance.
        /// </summary>
        public NoOperationMessage()
        {
            BackupServers = new List<ServerInfo>();
        }

        /// <summary>
        /// The list of backup servers.
        /// </summary>
        [XmlArray(Order = 0, ElementName = "BackupCommunicationServers")]
        [XmlArrayItem("BackupCommunicationServer", IsNullable = true)]
        public List<ServerInfo> BackupServers { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public override MessageClass MessageType => MessageClass.NoOperation;

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>String value that represents this object.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" BackupServers{");
            builder.Append(string.Join(",", BackupServers));
            builder.Append('}');

            return builder.ToString();
        }
    }
}