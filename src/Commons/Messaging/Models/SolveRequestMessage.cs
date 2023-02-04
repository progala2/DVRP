using System;
using System.Text;
using System.Text.Json.Serialization;
using Dvrp.Ucc.Commons.Messaging.Models.Base;

namespace Dvrp.Ucc.Commons.Messaging.Models
{
    /// <summary>
    /// Object representation of the Solve Request message.
    /// </summary>
    [Serializable]
    public class SolveRequestMessage : Message
    {
        ///
	    public SolveRequestMessage(string problemType, byte[] problemData, ulong? solvingTimeout = null,
	        ulong? problemInstanceId = null)
	    {
		    ProblemType = problemType;
		    SolvingTimeout = solvingTimeout;
		    ProblemData = problemData;
		    ProblemInstanceId = problemInstanceId;
	    }

	    /// <summary>
        /// The name of the type as given by TaskSolver.
        /// </summary>
        public string ProblemType { get; set; }

        /// <summary>
        /// The optional time restriction for solving the problem (in ms).
        /// </summary>
        public ulong? SolvingTimeout { get; set; }

        /// <summary>
        /// The serialized problem data.
        /// </summary>
        [JsonPropertyName("Data")]
        public byte[] ProblemData { get; set; }

        /// <summary>
        /// The ID of the problem instance assigned by the server.
        /// </summary>
        [JsonPropertyName("Id")]
        public ulong? ProblemInstanceId { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [JsonIgnore]
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