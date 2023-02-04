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
    /// Object representation of the Solutions message.
    /// </summary>
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "Status")]
    public class StatusMessage : Message
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation = "Status.xsd";

        /// <summary>
        /// Creates StatusMessage instance.
        /// </summary>
        public StatusMessage()
        {
            Threads = new List<ThreadStatus>();
        }

        /// <summary>
        /// The ID of node assigned by server.
        /// </summary>
        [XmlElement(Order = 0, ElementName = "Id")]
        public ulong ComponentId { get; set; }

        /// <summary>
        /// The list of statuses for different threads.
        /// </summary>
        [XmlArray(Order = 1)]
        [XmlArrayItem("Thread", IsNullable = false)]
        public List<ThreadStatus> Threads { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public override MessageClass MessageType => MessageClass.Status;

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>String value that represents this object.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" ComponentId(" + ComponentId + ")");

            builder.Append(" Threads{");
            builder.Append(string.Join(",", Threads));
            builder.Append('}');

            return builder.ToString();
        }
    }
}