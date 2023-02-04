using System.Text.Json.Serialization;

namespace Dvrp.Ucc.Commons.Messaging.Models.Base
{
    /// <summary>
    /// Base abstract class for all messages used in the system.
    /// </summary>
    [JsonDerivedType(typeof(DivideProblemMessage), (int)MessageClass.DivideProblem)]
    [JsonDerivedType(typeof(ErrorMessage), (int)MessageClass.Error)]
    [JsonDerivedType(typeof(NoOperationMessage), (int)MessageClass.NoOperation)]
    [JsonDerivedType(typeof(RegisterMessage), (int)MessageClass.Register)]
    [JsonDerivedType(typeof(RegisterResponseMessage), (int)MessageClass.RegisterResponse)]
    [JsonDerivedType(typeof(SolutionRequestMessage), (int)MessageClass.SolutionRequest)]
    [JsonDerivedType(typeof(SolutionsMessage), (int)MessageClass.Solutions)]
    [JsonDerivedType(typeof(PartialProblemsMessage), (int)MessageClass.SolvePartialProblems)]
    [JsonDerivedType(typeof(SolveRequestMessage), (int)MessageClass.SolveRequest)]
    [JsonDerivedType(typeof(SolveRequestResponseMessage), (int)MessageClass.SolveRequestResponse)]
    [JsonDerivedType(typeof(StatusMessage), (int)MessageClass.Status)]
    public abstract class Message
    {
	    /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [JsonIgnore]
        public abstract MessageClass MessageType { get; }

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>String value that represents this object.</returns>
        public override string ToString()
        {
            return "[" + MessageType + "]";
        }
    }
}