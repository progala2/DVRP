using System;
using System.Collections.Generic;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement
{
    internal class MergeWork : Work
    {
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

        public List<PartialSolution> PartialSolutions { get; private set; }
        public override ulong AssigneeId { get; protected set; }

        public override WorkType Type
        {
            get { return WorkType.Merge; }
        }

        public override Message CreateMessage()
        {
            var msgPartialSolutions = new List<SolutionsMessage.Solution>(PartialSolutions.Count);

            foreach (var ps in PartialSolutions)
            {
                var msgPS = new SolutionsMessage.Solution
                {
                    ComputationsTime = ps.ComputationsTime,
                    Data = ps.Data,
                    PartialProblemId = ps.PartialProblem.Id,
                    TimeoutOccured = ps.TimeoutOccured,
                    Type = SolutionsMessage.SolutionType.Partial
                };
                msgPartialSolutions.Add(msgPS);
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