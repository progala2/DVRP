using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;
using System;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement
{
    public class DivisionWork : Work
    {
        public Problem Problem 
        { 
            get; 
            private set; 
        }
        public override ulong AssigneeId
        {
            get;
            protected set;
        }
        public override WorkType Type
        {
            get { return WorkType.Division; }
        }


        public DivisionWork(ulong assigneeId, Problem problem, ulong availableThreads)
        {
            if (problem == null)
                throw new ArgumentException();

            AssigneeId = assigneeId;
            Problem = problem;
        }

        public override DivideProblemMessage CreateMessage()
        {
            if (!Problem.NumberOfParts.HasValue)
                throw new InvalidOperationException("Target number of parts to divide into must be set.");


            var message = new DivideProblemMessage()
            {
                ComputationalNodes = Problem.NumberOfParts.Value,
                ProblemData = Problem.Data,
                ProblemInstanceId = Problem.Id.Value,
                ProblemType = Problem.ProblemType,
                TaskManagerId = AssigneeId,
            };

            return message;
        }


    }
}
