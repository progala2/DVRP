using System;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Dvrp.Ucc.Commons.Messaging.Models.Base;

namespace Dvrp.Ucc.Commons.Messaging.Models
{
    /// <summary>
    /// Object representation of the Divide Problem message.
    /// </summary>
    [Serializable]
    public class DivideProblemMessage : Message
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="problemType"></param>
        /// <param name="problemInstanceId"></param>
        /// <param name="problemData"></param>
        /// <param name="computationalNodes"></param>
        /// <param name="taskManagerId"></param>
	    public DivideProblemMessage(string problemType, ulong problemInstanceId, byte[]? problemData, ulong computationalNodes, ulong taskManagerId)
        {
	        ProblemType = problemType;
	        ProblemInstanceId = problemInstanceId;
	        ProblemData = problemData;
	        ComputationalNodes = computationalNodes;
	        TaskManagerId = taskManagerId;
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
        /// The problem data.
        /// </summary>
        [JsonPropertyName("Data")]
        public byte[]? ProblemData { get; set; }

        /// <summary>
        /// The total number of currently available threads.
        /// </summary>
        [JsonPropertyName("ComputationalNodes")]
        public ulong ComputationalNodes { get; set; }

        /// <summary>
        /// The ID of the TM that is dividing the problem.
        /// </summary>
        [JsonPropertyName("NodeID")]
        public ulong TaskManagerId { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public override MessageClass MessageType => MessageClass.DivideProblem;

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>String value that represents this object.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" ProblemId(" + ProblemInstanceId + ")");
            builder.Append(" ProblemType(" + ProblemType + ")");
            builder.Append(" NodeId(" + TaskManagerId + ")");
            builder.Append(" CompNodes(" + ComputationalNodes + ")");

            return builder.ToString();
        }
    }
}