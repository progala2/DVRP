using System;

namespace Dvrp.Ucc.CommunicationServer.WorkManagement.Models
{
    /// <summary>
    /// Contains data and metadata concerning a partial problem.
    /// </summary>
    internal class PartialProblem
    {
        /// <summary>
        /// Possible states of a partial problem during its lifetime.
        /// </summary>
        public enum PartialProblemState
        {
            AwaitingComputation = 0,
            BeingComputed
        }

        /// <summary>
        /// Creates PartialProblem instance.
        /// </summary>
        /// <param name="id">ID of the partial problem within a problem instance.</param>
        /// <param name="problem">Corresponding problem instance.</param>
        /// <param name="privateData">Partial problem's private data.</param>
        public PartialProblem(ulong id, Problem problem, byte[] privateData)
        {
            Id = id;
            Problem = problem;
            PrivateData = privateData;

            if (problem.CommonData == null)
                throw new Exception("Common data in the corresponding problem instance must be set.");
        }

        /// <summary>
        /// Current state of the partial problem.
        /// </summary>
        public PartialProblemState State { get; set; }

        /// <summary>
        /// ID of the computational node that is currently processing this partial problem. Null if isn't processed.
        /// </summary>
        public ulong? ComputingNodeId { get; set; }

        /// <summary>
        /// Corresponding problem instance.
        /// </summary>
        public Problem Problem { get; }

        /// <summary>
        /// ID of the partial problem within the corresponding problem instance.
        /// </summary>
        public ulong Id { get; }

        /// <summary>
        /// Partial problem's private data.
        /// </summary>
        public byte[] PrivateData { get; }

        /// <summary>
        /// Data shared among all partial problems of the same problem instance.
        /// </summary>
        public byte[]? CommonData => Problem.CommonData;
    }
}