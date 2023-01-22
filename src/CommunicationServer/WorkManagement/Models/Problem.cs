namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Models
{
    /// <summary>
    /// Contains data and metadata concerning a problem instance.
    /// </summary>
    internal class Problem
    {
        /// <summary>
        /// Possible states of a problem instance during its lifetime.
        /// </summary>
        public enum ProblemState
        {
            AwaitingDivision = 0,
            BeingDivided,
            AwaitingSolution
        }

        /// <summary>
        /// Creates Problem instance.
        /// </summary>
        /// <param name="id">ID of the problem instance.</param>
        /// <param name="type">Problem class type name.</param>
        /// <param name="data">Problem data.</param>
        /// <param name="solvingTimeout">Timeout for solving this problem instance.</param>
        public Problem(ulong id, string type, byte[] data, ulong solvingTimeout)
        {
            Id = id;
            Type = type;
            Data = data;
            SolvingTimeout = solvingTimeout;
        }

        /// <summary>
        /// Current state of the problem instance.
        /// </summary>
        public ProblemState State { get; set; }

        /// <summary>
        /// ID of the task manager that is currently dividing this problem instance. Null if hasn't been divided.
        /// </summary>
        public ulong DividingNodeId { get; set; }

        /// <summary>
        /// Problem class type name.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Timeout for solving this problem instance.
        /// </summary>
        public ulong SolvingTimeout { get; }

        /// <summary>
        /// Problem data as sent by the client.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Problem data shared among partial problems (null before problem division).
        /// </summary>
        public byte[]? CommonData { get; set; }

        /// <summary>
        /// ID of the problem instance.
        /// </summary>
        public ulong Id { get; }

        /// <summary>
        /// Number of partial problems this problem instance has been divided into.
        /// </summary>
        public ulong NumberOfParts { get; set; }

        public void AwaitDivision()
        {
	        State = ProblemState.AwaitingDivision;
	        DividingNodeId = 0;
        }
    }
}