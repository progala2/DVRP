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
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "SolveRequest")]
    public class SolveRequestMessage : Message
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation = "SolveRequest.xsd";

        /// <summary>
        /// The name of the type as given by TaskSolver.
        /// </summary>
        [XmlElement(Order = 0)]
        public string? ProblemType { get; set; }

        /// <summary>
        /// The optional time restriction for solving the problem (in ms).
        /// </summary>
        [XmlElement(Order = 1)]
        public ulong? SolvingTimeout { get; set; }

        /// <summary>
        /// The serialized problem data.
        /// </summary>
        [XmlElement(Order = 2, ElementName = "Data", DataType = "base64Binary")]
        public byte[]? ProblemData { get; set; }

        /// <summary>
        /// The ID of the problem instance assigned by the server.
        /// </summary>
        [XmlElement(Order = 3, ElementName = "Id")]
        public ulong? ProblemInstanceId { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public override MessageClass MessageType => MessageClass.SolveRequest;

        /// <summary>
        /// Determines whether ProblemInstanceId property should be serialized.
        /// </summary>
        /// <returns>True if ProblemInstanceId property should be serialized; false otherwise.</returns>
        public bool ShouldSerializeProblemInstanceId()
        {
            return ProblemInstanceId.HasValue;
        }

        /// <summary>
        /// Determines whether SolvingTimeout property should be serialized.
        /// </summary>
        /// <returns>True if SolvingTimeout property should be serialized; false otherwise.</returns>
        public bool ShouldSerializeSolvingTimeout()
        {
            return SolvingTimeout.HasValue;
        }

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>String value that represents this object.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            if (ProblemInstanceId.HasValue)
                builder.Append(" ProblemInstanceId(" + ProblemInstanceId.Value + ")");

            builder.Append(" ProblemType(" + ProblemType + ")");

            if (SolvingTimeout.HasValue)
                builder.Append(" SolvingTimeout(" + SolvingTimeout.Value + ")");
            else
                builder.Append(" SolvingTimeout(none)");

            return builder.ToString();
        }
    }
}