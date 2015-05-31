using System;
using System.Collections.Generic;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement
{
    /// <summary>
    /// Class representing merge work assignable to a task manager.
    /// </summary>
    internal class MergeWork : Work
    {
        /// <summary>
        /// Creates MergeWork instance.
        /// </summary>
        /// <param name="assigneeId">ID of the assignee task manager.</param>
        /// <param name="partialSolutions">List of partial solutions to merge into the final one.</param>
        public MergeWork(ulong assigneeId, IList<PartialSolution> partialSolutions)
        {
            if (partialSolutions == null || partialSolutions.Count == 0)
                throw new ArgumentException();

            var expectedProblemId = partialSolutions[0].PartialProblem.Problem.Id;
            foreach (var ps in partialSolutions)
            {
                var problemId = partialSolutions[0].PartialProblem.Problem.Id;

                if (expectedProblemId != problemId)
                    throw new ArgumentException("All partial solutions must belong to the same problem instance.");
            }

            AssigneeId = assigneeId;
            PartialSolutions = new List<PartialSolution>(partialSolutions);
        }

        /// <summary>
        /// List of partial solutions to merge.
        /// </summary>
        public List<PartialSolution> PartialSolutions { get; private set; }

        /// <summary>
        /// ID of the task manager this merge work has been assigned to.
        /// </summary>
        public override ulong AssigneeId { get; protected set; }

        /// <summary>
        /// Type of work to be done (merge).
        /// </summary>
        public override WorkType Type
        {
            get { return WorkType.Merge; }
        }

        /// <summary>
        /// Create solutions message that can be send to a task manager in order to request the merge.
        /// </summary>
        /// <returns>Solutions message.</returns>
        public override Message CreateMessage()
        {
            var msgPartialSolutions = new List<SolutionsMessage.Solution>(PartialSolutions.Count);

            foreach (var ps in PartialSolutions)
            {
                var msgPs = new SolutionsMessage.Solution
                {
                    ComputationsTime = ps.ComputationsTime,
                    Data = ps.Data,
                    PartialProblemId = ps.PartialProblem.Id,
                    TimeoutOccured = ps.TimeoutOccured,
                    Type = SolutionsMessage.SolutionType.Partial
                };
                msgPartialSolutions.Add(msgPs);
            }

            var problem = PartialSolutions[0].PartialProblem.Problem;

            var message = new SolutionsMessage
            {
                CommonData = problem.CommonData,
                ProblemInstanceId = problem.Id,
                ProblemType = problem.Type,
                Solutions = msgPartialSolutions
            };

            return message;
        }
    }
}