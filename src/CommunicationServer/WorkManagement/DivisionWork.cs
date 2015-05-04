using System;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement
{
    internal class DivisionWork : Work
    {
        public DivisionWork(ulong assigneeId, Problem problem, ulong availableThreads)
        {
            if (problem == null)
                throw new ArgumentNullException();

            AssigneeId = assigneeId;
            Problem = problem;

            Problem.NumberOfParts = availableThreads;
        }

        public Problem Problem { get; private set; }
        public override ulong AssigneeId { get; protected set; }

        public override WorkType Type
        {
            get { return WorkType.Division; }
        }

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