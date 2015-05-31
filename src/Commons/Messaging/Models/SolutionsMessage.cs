using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    /// <summary>
    /// Object representation of the Solutions message.
    /// </summary>
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "Solutions")]
    public class SolutionsMessage : Message
    {
        /// <summary>
        /// Possible solution types/states specified by this message.
        /// </summary>
        [Serializable]
        [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
        public enum SolutionType
        {
            Ongoing,
            Partial,
            Final
        }

        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation",
            Namespace = "http://www.w3.org/2001/XMLSchema-instance")] public string NoNamespaceSchemaLocation =
                "Solutions.xsd";

        public SolutionsMessage()
        {
            Solutions = new List<Solution>();
        }

        [XmlElement(Order = 0)]
        public string ProblemType { get; set; }

        [XmlElement(Order = 1, ElementName = "Id")]
        public ulong ProblemInstanceId { get; set; }

        [XmlElement(DataType = "base64Binary", Order = 2)]
        public byte[] CommonData { get; set; }

        [XmlArray(Order = 3)]
        [XmlArrayItem("Solution", IsNullable = false)]
        public List<Solution> Solutions { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.Solutions; }
        }

        public bool ShouldSerializeCommonData()
        {
            return CommonData != null;
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

            builder.Append(" Solutions{");
            builder.Append(string.Join(",", Solutions));
            builder.Append("}");

            return builder.ToString();
        }

        /// <summary>
        /// Object representation of a solution as in Solutions message.
        /// </summary>
        [Serializable]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
        public class Solution
        {
            [XmlElement(Order = 0, ElementName = "TaskId")]
            public ulong? PartialProblemId { get; set; }

            [XmlElement(Order = 1)]
            public bool TimeoutOccured { get; set; }

            [XmlElement(Order = 2)]
            public SolutionType Type { get; set; }

            [XmlElement(Order = 3)]
            public ulong ComputationsTime { get; set; }

            [XmlElement(Order = 4, DataType = "base64Binary")]
            public byte[] Data { get; set; }

            public bool ShouldSerializePartialProblemId()
            {
                return PartialProblemId.HasValue;
            }

            public bool ShouldSerializeData()
            {
                return Data != null;
            }

            /// <summary>
            /// Gets string representation.
            /// </summary>
            /// <returns>String value that represents this object.</returns>
            public override string ToString()
            {
                var builder = new StringBuilder();

                builder.Append("Type(" + Type + ")");

                if (PartialProblemId.HasValue)
                    builder.Append(" Id(" + PartialProblemId + ")");

                builder.Append(" TimeoutOccured(" + TimeoutOccured + ")");
                builder.Append(" ComputationsTime(" + ComputationsTime + ")");

                return builder.ToString();
            }
        }
    }
}