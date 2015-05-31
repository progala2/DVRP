using System;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement
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
        public DivisionWork(ulong assigneeId, Problem problem, ulong availableThreads)
        {
            if (problem == null)
                throw new ArgumentNullException();

            AssigneeId = assigneeId;
            Problem = problem;

            Problem.NumberOfParts = availableThreads;
        }
        /// <summary>
        /// Problem instance to divide.
        /// </summary>
        public Problem Problem { get; private set; }
        /// <summary>
        /// ID of the task manager this division work has been assigned to.
        /// </summary>
        public override ulong AssigneeId { get; protected set; }
        /// <summary>
        /// Type of work to be done (division).
        /// </summary>
        public override WorkType Type
        {
            get { return WorkType.Division; }
        }
        /// <summary>
        /// Create divide problem message that can be send to a task manager.
        /// </summary>
        /// <returns>Divide problem message.</returns>
        public override Message CreateMessage()
        {
            var message = new DivideProblemMessage
            {
                ComputationalNodes = Problem.NumberOfParts.Value,
                ProblemData = Problem.Data,
                ProblemInstanceId = Problem.Id,
                ProblemType = Problem.Type,
                TaskManagerId = AssigneeId
            };

            return message;
        }
    }
}