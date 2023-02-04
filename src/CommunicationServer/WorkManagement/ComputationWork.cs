using System;
using System.Collections.Generic;
using Dvrp.Ucc.Commons.Messaging.Models;
using Dvrp.Ucc.Commons.Messaging.Models.Base;
using Dvrp.Ucc.CommunicationServer.WorkManagement.Base;
using Dvrp.Ucc.CommunicationServer.WorkManagement.Models;

namespace Dvrp.Ucc.CommunicationServer.WorkManagement
{
    /// <summary>
    /// Class representing computation work assignable to a computational node.
    /// </summary>
    internal class ComputationWork : Work
    {
        /// <summary>
        /// Creates ComputationWork instance.
        /// </summary>
        /// <param name="assigneeId">ID of the assignee computational node.</param>
        /// <param name="partialProblems">Assigned partial problems to compute.</param>
        public ComputationWork(ulong assigneeId, IList<PartialProblem> partialProblems): base(assigneeId)
        {
            if (partialProblems == null)
                throw new ArgumentNullException();

            if (partialProblems.Count == 0)
                throw new ArgumentException();

            var problemId = partialProblems[0].Problem.Id;
            foreach (var pp in partialProblems)
            {
                if (problemId != pp.Problem.Id)
                    throw new ArgumentException("All partial problems must belong to the same problem instance.");
            }

            PartialProblems = new List<PartialProblem>(partialProblems);
        }
        /// <summary>
        /// Assigned partial problems to compute. All must belong to the same problem instance.
        /// </summary>
        public List<PartialProblem> PartialProblems { get; }
        /// <summary>
        /// Type of work to be done (computation).
        /// </summary>
        public override WorkType Type => WorkType.Computation;

        /// <summary>
        /// Create partial problems message that can be send to a computational node.
        /// </summary>
        /// <returns>Partial problems message.</returns>
        public override Message CreateMessage()
        {
            var msgPartialProblems = new List<PartialProblemsMessage.PartialProblem>(PartialProblems.Count);

            foreach (var pp in PartialProblems)
            {
                var msgPp = new PartialProblemsMessage.PartialProblem
                {
                    Data = pp.PrivateData,
                    PartialProblemId = pp.Id,
                    TaskManagerId = pp.Problem.DividingNodeId
                };
                msgPartialProblems.Add(msgPp);
            }

            var problem = PartialProblems[0].Problem;

            var message = new PartialProblemsMessage
            {
                CommonData = problem.CommonData,
                PartialProblems = msgPartialProblems,
                ProblemInstanceId = problem.Id,
                ProblemType = problem.Type,
                SolvingTimeout = problem.SolvingTimeout
            };

            return message;
        }
    }
}