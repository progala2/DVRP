using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Dvrp.Ucc.Commons.Messaging.Models.Base;

namespace Dvrp.Ucc.Commons.Messaging.Models
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
            /// <summary>
            /// 
            /// </summary>
            Ongoing,
            /// <summary>
            /// 
            /// </summary>
            Partial,
            /// <summary>
            /// 
            /// </summary>
            Final
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation = "Solutions.xsd";

        /// <summary>
        /// Creates SolutionsMessage instance.
        /// </summary>
        public SolutionsMessage(string problemType)
        {
	        ProblemType = problemType;
	        Solutions = new List<Solution>();
        }

        /// <summary>
        /// The problem type name as given by TaskSolver and Client.
        /// </summary>
        public string ProblemType { get; set; }

        /// <summary>
        /// The ID of the problem instance assigned by the server.
        /// </summary>
        [JsonPropertyName("Id")]
        public ulong ProblemInstanceId { get; set; }

        /// <summary>
        /// The Common data which was previously sent to all Computational Nodes.
        /// </summary>
        public byte[]? CommonData { get; set; }

        /// <summary>
        /// The solutions.
        /// </summary>
        public List<Solution> Solutions { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public override MessageClass MessageType => MessageClass.Solutions;

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
            builder.Append('}');

            return builder.ToString();
        }

        /// <summary>
        /// Object representation of a solution as in Solutions message.
        /// </summary>
        [Serializable]
        public class Solution
        {
            /// <summary>
            /// The id of subproblem given by TaskManager – no TaskId for final/merged solution.
            /// </summary>
            [JsonPropertyName("TaskId")]
            public ulong? PartialProblemId { get; set; }

            /// <summary>
            /// The indicator that the computations ended because of timeout.
            /// </summary>
            public bool TimeoutOccured { get; set; }

            /// <summary>
            /// The information about the status of result (Partial/Final) or status of computations (Ongoing).
            /// </summary>
            public SolutionType Type { get; set; }

            /// <summary>
            /// Total amount of time used by all threads in system for computing the solution / during the ongoing computations (in ms).
            /// </summary>
            public ulong ComputationsTime { get; set; }

            /// <summary>
            /// The solution data.
            /// </summary>
            public byte[] Data { get; set; } = null!; // if wrong message the xml parser will fail?

            /// <summary>
            /// Determines whether PartialProblemId property should be serialized.
            /// </summary>
            /// <returns>True if CommonDataPartialProblemId property should be serialized; false otherwise.</returns>
            public bool ShouldSerializePartialProblemId()
            {
                return PartialProblemId.HasValue;
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