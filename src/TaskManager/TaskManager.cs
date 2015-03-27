using System;
using System.Collections.Generic;
using System.Net;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.TaskManager
{
    public sealed class TaskManager : ComputationalComponent
    {
        public TaskManager(IPEndPoint serverAddress)
            : base(serverAddress)
        { }

        protected override RegisterMessage GetRegisterMessage()
        {
            var registerMessage = new RegisterMessage()
            {
                Type = RegisterType.TaskManager,
                ParallelThreads = _parallelThreads,
                SolvableProblems = new List<string>(TaskSolvers.Keys)
            };
            return registerMessage;
        }

        protected override void HandleResponseMessage(Message message)
        {
            switch (message.MessageType)
            {
                case Message.MessageClassType.DivideProblem:
                    DivideProblemMessageHandler((DivideProblemMessage)message);
                    break;
                case Message.MessageClassType.Solutions:
                    SolutionsMessageHandler((SolutionsMessage)message);
                    break;
                default:
                    // maybe other messages to add
                    throw new NotImplementedException();
            }
        }
        private void DivideProblemMessageHandler(DivideProblemMessage divideProblemMessage)
        {
            throw new NotImplementedException();
        }

        private void SolutionsMessageHandler(SolutionsMessage solutionsMessage)
        {
            throw new NotImplementedException();
        }


    }
}
