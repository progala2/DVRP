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

        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation = "Solutions.xsd";

        /// <summary>
        /// Creates SoultionsMessage instance.
        /// </summary>
        public SolutionsMessage()
        {
            Solutions = new List<Solution>();
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
        /// The Common data which was previously sent to all Computational Nodes.
        /// </summary>
        [XmlElement(DataType = "base64Binary", Order = 2)]
        public byte[] CommonData { get; set; }

        /// <summary>
        /// The solutions.
        /// </summary>
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

        /// <summary>
        /// Determines whether CommonData property should be serialized.
        /// </summary>
        /// <returns>True if CommonData property should be serialized; false otherwise.</returns>
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
            /// <summary>
            /// The id of subproblem given by TaskManager – no TaskId for final/merged solution.
            /// </summary>
            [XmlElement(Order = 0, ElementName = "TaskId")]
            public ulong? PartialProblemId { get; set; }

            /// <summary>
            /// The indicator that the computations ended because of timeout.
            /// </summary>
            [XmlElement(Order = 1)]
            public bool TimeoutOccured { get; set; }

            /// <summary>
            /// The information about the status of result (Partial/Final) or status of computations (Ongoing).
            /// </summary>
            [XmlElement(Order = 2)]
            public SolutionType Type { get; set; }

            /// <summary>
            /// Total amount of time used by all threads in system for computing the solution / during the ongoingcomputations (in ms).
            /// </summary>
            [XmlElement(Order = 3)]
            public ulong ComputationsTime { get; set; }

            /// <summary>
            /// The solution data.
            /// </summary>
            [XmlElement(Order = 4, DataType = "base64Binary")]
            public byte[] Data { get; set; }

            /// <summary>
            /// Determines whether PartialProblemId property should be serialized.
            /// </summary>
            /// <returns>True if CommonDataPartialProblemId property should be serialized; false otherwise.</returns>
            public bool ShouldSerializePartialProblemId()
            {
                return PartialProblemId.HasValue;
            }

            /// <summary>
            /// Determines whether Data property should be serialized.
            /// </summary>
            /// <returns>True if Data property should be serialized; false otherwise.</returns>
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