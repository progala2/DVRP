using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    /// <summary>
    /// Object representation of the Partial Problems message.
    /// </summary>
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "SolvePartialProblems")]
    public class PartialProblemsMessage : Message
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation = "PartialProblems.xsd";

        /// <summary>
        /// Creates PartialProblems instance.
        /// </summary>
        public PartialProblemsMessage()
        {
            PartialProblems = new List<PartialProblem>();
        }

        /// <summary>
        /// The problem type name as given by TaskSolver and Client.
        /// </summary>
        [XmlElement(Order = 0)]
        public string ProblemType { get; set; }

        /// <summary>
        /// The ID of the problem instance assigned by the server.
        /// </summary>
        [XmlElement(Order = 1, ElementName = "Id")]
        public ulong ProblemInstanceId { get; set; }

        /// <summary>
        /// The data to be sent to all Computational Nodes.
        /// </summary>
        [XmlElement(Order = 2, DataType = "base64Binary")]
        public byte[]? CommonData { get; set; }

        /// <summary>
        /// Optional time limit – set by Client (in ms).
        /// </summary>
        [XmlElement(Order = 3)]
        public ulong? SolvingTimeout { get; set; }

        /// <summary>
        /// The partial problems.
        /// </summary>
        [XmlArray(Order = 4)]
        [XmlArrayItem("PartialProblem", IsNullable = false)]
        public List<PartialProblem> PartialProblems { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public override MessageClass MessageType => MessageClass.SolvePartialProblems;

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

            builder.Append(" ProblemInstanceId(" + ProblemInstanceId + ")");
            builder.Append(" ProblemType(" + ProblemType + ")");

            if (SolvingTimeout.HasValue)
                builder.Append(" SolvingTimeout(" + SolvingTimeout.Value + ")");

            builder.Append(" PartialProblems{");
            builder.Append(string.Join(",", PartialProblems));
            builder.Append("}");

            return builder.ToString();
        }

        /// <summary>
        /// Object representation of a partial problem in Partial Problems message.
        /// </summary>
        [Serializable]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
        public class PartialProblem
        {
            /// <summary>
            /// The Id of subproblem given by TaskManager.
            /// </summary>
            [XmlElement(ElementName = "TaskId", Order = 0)]
            public ulong PartialProblemId { get; set; }

            /// <summary>
            /// The Data specific for the given subproblem.
            /// </summary>
            [XmlElement(DataType = "base64Binary", Order = 1)]
            public byte[] Data { get; set; }

            /// <summary>
            /// The ID of the TM that is dividing the problem.
            /// </summary>
            [XmlElement(ElementName = "NodeID", Order = 2)]
            public ulong TaskManagerId { get; set; }

            /// <summary>
            /// Gets string representation.
            /// </summary>
            /// <returns>String value that represents this object.</returns>
            public override string ToString()
            {
                var builder = new StringBuilder();

                builder.Append("Id(" + PartialProblemId + ")");
                builder.Append(" TaskManagerId(" + TaskManagerId + ")");

                return builder.ToString();
            }
        }
    }
}