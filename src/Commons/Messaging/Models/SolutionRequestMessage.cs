using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using Dvrp.Ucc.Commons.Messaging.Models.Base;

namespace Dvrp.Ucc.Commons.Messaging.Models
{
    /// <summary>
    /// Object representation of the Solution Request message.
    /// </summary>
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "SolutionRequest")]
    public class SolutionRequestMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation = "SolutionRequest.xsd";

        /// <summary>
        /// The ID of the problem instance assigned by the server.
        /// </summary>
        [XmlElement(Order = 0, ElementName = "Id")]
        public ulong ProblemInstanceId { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public override MessageClass MessageType => MessageClass.SolutionRequest;

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>String value that represents this object.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" ProblemInstanceId(" + ProblemInstanceId + ")");

            return builder.ToString();
        }
    }
}