using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using Dvrp.Ucc.Commons.Messaging.Models.Base;

namespace Dvrp.Ucc.Commons.Messaging.Models
{
    /// <summary>
    /// Object representation of the Solve Request message.
    /// </summary>
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "SolveRequestResponse")]
    public class SolveRequestResponseMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation = "SolveRequestResponse.xsd";

        /// <summary>
        /// The ID of the problem instance assigned by the server.
        /// </summary>
        [XmlElement(Order = 0, ElementName = "Id")]
        public ulong AssignedId { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public override MessageClass MessageType => MessageClass.SolveRequestResponse;

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>String value that represents this object.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" AssignedId(" + AssignedId + ")");

            return builder.ToString();
        }
    }
}