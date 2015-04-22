using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Models
{
    public class PartialSolution
    {
        public PartialProblem PartialProblem
        {
            get;
            private set;
        }
        public byte[] Data
        {
            get;
            private set;
        }
        public ulong ComputationsTime
        {
            get;
            private set;
        }
        public bool TimeoutOccured
        {
            get;
            private set;
        }
        public ulong SolvingComputationalNodeId
        {
            get;
            private set;
        }

        public PartialSolution(PartialProblem problem, byte[] data,
            ulong computationsTime, bool timeoutOccured, ulong solverNodeId)
        {
            PartialProblem = problem;
            Data = data;
            ComputationsTime = computationsTime;
            TimeoutOccured = timeoutOccured;
            SolvingComputationalNodeId = solverNodeId;
        }

        public static explicit operator SolutionsMessage.Solution(PartialSolution ps)
        {
            var output = new SolutionsMessage.Solution()
            {
                ComputationsTime = ps.ComputationsTime,
                Data = ps.Data,
                PartialProblemId = ps.PartialProblem.Id,
                TimeoutOccured = ps.TimeoutOccured,
                Type = SolutionsMessage.SolutionType.Partial,
            };

            return output;
        }
    }
}
