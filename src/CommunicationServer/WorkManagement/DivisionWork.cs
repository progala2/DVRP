using System;
using Dvrp.Ucc.Commons.Messaging.Models;
using Dvrp.Ucc.Commons.Messaging.Models.Base;
using Dvrp.Ucc.CommunicationServer.WorkManagement.Base;
using Dvrp.Ucc.CommunicationServer.WorkManagement.Models;

namespace Dvrp.Ucc.CommunicationServer.WorkManagement
{
    /// <summary>
    /// Class representing division work assignable to a task manager.
    /// </summary>
    internal class DivisionWork : Work
    {
        /// <summary>
        /// Creates DivisionWork instance.
        /// </summary>
        /// <param name="assigneeId">ID of the assignee task manager.</param>
        /// <param name="problem">Problem instance to divide.</param>
        /// <param name="availableThreads">Number of threads within the system that can solve this type of problem. De facto number of parts to divide into.</param>
        public DivisionWork(ulong assigneeId, Problem problem, ulong availableThreads): base(assigneeId)
        {
            if (problem == null)
                throw new ArgumentNullException();

            AssignedId = assigneeId;
            Problem = problem;

            Problem.NumberOfParts = availableThreads;
        }
        /// <summary>
        /// Problem instance to divide.
        /// </summary>
        public Problem Problem { get; }
        /// <summary>
        /// Type of work to be done (division).
        /// </summary>
        public override WorkType Type => WorkType.Division;

        /// <summary>
        /// Create divide problem message that can be send to a task manager.
        /// </summary>
        /// <returns>Divide problem message.</returns>
        public override Message CreateMessage()
        {
            var message = new DivideProblemMessage
            {
                ComputationalNodes = Problem.NumberOfParts,
                ProblemData = Problem.Data,
                ProblemInstanceId = Problem.Id,
                ProblemType = Problem.Type,
                TaskManagerId = AssignedId
            };

            return message;
        }
    }
}