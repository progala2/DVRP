
using _15pl04.Ucc.Commons.Messaging.Models;
namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Models
{
    public class Solution
    {
        public Problem Problem
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


        public Solution(Problem problem, byte[] data,
            ulong computationsTime, bool timeoutOccured = false)
        {
            Problem = problem;
            Data = data;
            ComputationsTime = computationsTime;
            TimeoutOccured = timeoutOccured;
        }

        public static explicit operator SolutionsMessage.Solution(Solution s)
        {
            var output = new SolutionsMessage.Solution()
            {
                ComputationsTime = s.ComputationsTime,
                Data = s.Data,
                PartialProblemId = null,
                TimeoutOccured = s.TimeoutOccured,
                Type = SolutionsMessage.SolutionType.Final,
            };

            return output;
        }
    }
}
