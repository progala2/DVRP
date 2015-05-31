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
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation",
            Namespace = "http://www.w3.org/2001/XMLSchema-instance")] public string NoNamespaceSchemaLocation =
                "PartialProblems.xsd";

        public PartialProblemsMessage()
        {
            PartialProblems = new List<PartialProblem>();
        }

        [XmlElement(Order = 0)]
        public string ProblemType { get; set; }

        [XmlElement(Order = 1, ElementName = "Id")]
        public ulong ProblemInstanceId { get; set; }

        [XmlElement(Order = 2, DataType = "base64Binary")]
        public byte[] CommonData { get; set; }

        [XmlElement(Order = 3)]
        public ulong? SolvingTimeout { get; set; }

        [XmlArray(Order = 4)]
        [XmlArrayItem("PartialProblem", IsNullable = false)]
        public List<PartialProblem> PartialProblems { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.SolvePartialProblems; }
        }

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
            [XmlElement(ElementName = "TaskId", Order = 0)]
            public ulong PartialProblemId { get; set; }

            [XmlElement(DataType = "base64Binary", Order = 1)]
            public byte[] Data { get; set; }

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