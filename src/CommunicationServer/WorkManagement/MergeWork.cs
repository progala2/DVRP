using System;
using System.Collections.Generic;
using Dvrp.Ucc.Commons.Messaging.Models;
using Dvrp.Ucc.Commons.Messaging.Models.Base;
using Dvrp.Ucc.CommunicationServer.WorkManagement.Base;
using Dvrp.Ucc.CommunicationServer.WorkManagement.Models;

namespace Dvrp.Ucc.CommunicationServer.WorkManagement
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
        public MergeWork(ulong assigneeId, IList<PartialSolution> partialSolutions): base(assigneeId)
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

            AssignedId = assigneeId;
            PartialSolutions = new List<PartialSolution>(partialSolutions);
        }

        /// <summary>
        /// List of partial solutions to merge.
        /// </summary>
        public List<PartialSolution> PartialSolutions { get; }

        /// <summary>
        /// Type of work to be done (merge).
        /// </summary>
        public override WorkType Type => WorkType.Merge;

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