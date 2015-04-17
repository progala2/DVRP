using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;
using System;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement
{
    public class ComputationWork : Work
    {
        public List<PartialProblem> PartialProblems
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
            get { return WorkType.Computation; }
        }


        public ComputationWork(ulong assigneeId, IList<PartialProblem> partialProblems)
        {
            if (partialProblems == null || partialProblems.Count == 0)
                throw new ArgumentException();

            ulong problemId = partialProblems[0].Problem.Id.Value;
            foreach (PartialProblem pp in partialProblems)
            {
                if (problemId != pp.Problem.Id)
                    throw new ArgumentException("All partial problems must belong to the same problem instance.");
            }
            
            AssigneeId = assigneeId;
            PartialProblems = new List<PartialProblem>(partialProblems);
        }

        public override PartialProblemsMessage CreateMessage()
        {
            var msgPartialProblems = new List<PartialProblemsMessage.PartialProblem>(PartialProblems.Count);

            foreach (PartialProblem pp in PartialProblems)
                msgPartialProblems.Add((PartialProblemsMessage.PartialProblem)pp);


            var message = new PartialProblemsMessage()
            {
                CommonData = PartialProblems[0].CommonData,
                PartialProblems = msgPartialProblems,
                ProblemInstanceId = PartialProblems[0].Problem.Id.Value,
                ProblemType = PartialProblems[0].Problem.ProblemType,
                SolvingTimeout = PartialProblems[0].Problem.SolvingTimeout,                
            };

            return message;
        }


    }
}
