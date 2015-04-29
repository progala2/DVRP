using _15pl04.Ucc.Commons.Messaging.Models;
using System;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Models
{
    internal class PartialProblem
    {
        public enum PartialProblemState
        {
            AwaitingComputation = 0,
            BeingComputed,
        }

        public PartialProblemState State
        {
            get;
            set;
        }
        public ulong? ComputingNodeId
        {
            get;
            set;
        }
        public Problem Problem
        {
            get;
            private set;
        }
        public ulong Id
        {
            get;
            private set;
        }
        public byte[] PrivateData
        {
            get;
            private set;
        }
        public byte[] CommonData
        {
            get
            {
                return Problem.CommonData;
            }
        }

        public PartialProblem(ulong id, Problem problem, byte[] privateData)
        {
            Id = id;
            Problem = problem;
            PrivateData = privateData;

            if (problem.CommonData == null)
                throw new Exception("Common data in the corresponding problem instance must be set.");
        }
    }
}
