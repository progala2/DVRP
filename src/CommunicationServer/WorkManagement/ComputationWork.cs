﻿using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;
using System;
using System.Collections.Generic;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement
{
    internal class ComputationWork : Work
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
            if (partialProblems == null)
                throw new ArgumentNullException();

            if (partialProblems.Count == 0)
                throw new ArgumentException();

            ulong problemId = partialProblems[0].Problem.Id;
            foreach (PartialProblem pp in partialProblems)
            {
                if (problemId != pp.Problem.Id)
                    throw new ArgumentException("All partial problems must belong to the same problem instance.");
            }
            
            PartialProblems = new List<PartialProblem>(partialProblems);
            AssigneeId = assigneeId;
        }

        public override Message CreateMessage()
        {
            var msgPartialProblems = new List<PartialProblemsMessage.PartialProblem>(PartialProblems.Count);

            foreach (PartialProblem pp in PartialProblems)
            {
                var msgPP = new PartialProblemsMessage.PartialProblem()
                {
                    Data = pp.PrivateData,
                    PartialProblemId = pp.Id,
                    TaskManagerId = pp.Problem.DividingNodeId.Value,
                };
                msgPartialProblems.Add(msgPP);
            }

            Problem problem = PartialProblems[0].Problem;

            var message = new PartialProblemsMessage()
            {
                CommonData = problem.CommonData,
                PartialProblems = msgPartialProblems,
                ProblemInstanceId = problem.Id,
                ProblemType = problem.Type,
                SolvingTimeout = problem.SolvingTimeout,
            };

            return message;
        }


    }
}
