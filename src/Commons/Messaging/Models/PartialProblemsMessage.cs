using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Dvrp.Ucc.Commons.Messaging.Models.Base;

namespace Dvrp.Ucc.Commons.Messaging.Models
{
    /// <summary>
    /// Object representation of the Partial Problems message.
    /// </summary>
    [Serializable]
    public class PartialProblemsMessage : Message
    {
	    /// <summary>
        /// Creates PartialProblems instance.
        /// </summary>
        public PartialProblemsMessage(string problemType)
	    {
		    ProblemType = problemType;
		    PartialProblems = new List<PartialProblem>();
	    }

        /// <summary>
        /// The problem type name as given by TaskSolver and Client.
        /// </summary>
        public string ProblemType { get; }

        /// <summary>
        /// The ID of the problem instance assigned by the server.
        /// </summary>
        [JsonPropertyName("Id")]
        public ulong ProblemInstanceId { get; set; }

        /// <summary>
        /// The data to be sent to all Computational Nodes.
        /// </summary>
        public byte[]? CommonData { get; set; }

        /// <summary>
        /// Optional time limit – set by Client (in ms).
        /// </summary>
        public ulong? SolvingTimeout { get; set; }

        /// <summary>
        /// The partial problems.
        /// </summary>
        [JsonPropertyName("PartialProblem")]
        public List<PartialProblem> PartialProblems { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [JsonIgnore]
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
            builder.Append('}');

            return builder.ToString();
        }

        /// <summary>
        /// Object representation of a partial problem in Partial Problems message.
        /// </summary>
        [Serializable]
        public class PartialProblem
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="partialProblemId"></param>
            /// <param name="data"></param>
            /// <param name="taskManagerId"></param>
	        public PartialProblem(ulong partialProblemId, byte[] data, ulong taskManagerId)
	        {
		        PartialProblemId = partialProblemId;
		        Data = data;
		        TaskManagerId = taskManagerId;
	        }

	        /// <summary>
            /// The Id of subproblem given by TaskManager.
            /// </summary>
            [JsonPropertyName("TaskId")]
            public ulong PartialProblemId { get; set; }

            /// <summary>
            /// The Data specific for the given subproblem.
            /// </summary>
            public byte[] Data { get; set; }

            /// <summary>
            /// The ID of the TM that is dividing the problem.
            /// </summary>
            [JsonPropertyName("NodeID")]
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