using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.ComputationalNode
{
    public class ComputationalNode : ComputationalComponent
    {
        public ComputationalNode(IPEndPoint serverAddress)
            : base(serverAddress)
        { }

        protected override RegisterMessage GetRegisterMessage()
        {
            var registerMessage = new RegisterMessage()
            {
                Type = ComponentType.ComputationalNode,
                ParallelThreads = _parallelThreads,
                SolvableProblems = new List<string>(TaskSolvers.Keys)
            };
            return registerMessage;
        }

        protected override void HandleResponseMessage(Message message)
        {
            switch (message.MessageType)
            {
                case Message.MessageClassType.PartialProblems:
                    PartialProblemsMessageHandler((PartialProblemsMessage)message);
                    break;
                default:
                    // maybe other messages to add
                    throw new NotImplementedException();
            }
        }

        private void PartialProblemsMessageHandler(PartialProblemsMessage message)
        {
            foreach (var partialProblem in message.PartialProblems)
            {
                /* each partial problem should be started properly cause server sends at most 
                 * as many partial problems as count of component's tasks in idle state */
                bool started = ComputationalTaskPool.StartComputationalTask(() =>
                {
                    var taskSolver = TaskSolvers[message.ProblemType];
                    var stopwatch = new Stopwatch();

                    // dont know if stopwatch is thread safe...
                    stopwatch.Start();
                    var result = taskSolver.Solve(partialProblem.Data, TimeSpan.FromMilliseconds((double)message.SolvingTimeout.Value));
                    stopwatch.Stop();

                    var solutions = new List<SolutionsSolution>();
                    solutions.Add(new SolutionsSolution()
                    {
                        Data = result,
                        ComputationsTime = (ulong)stopwatch.ElapsedMilliseconds,
                        TaskId = partialProblem.TaskId,
                        TimeoutOccured = false, // don't know how to find it
                        Type = SolutionType.Partial
                    });
                    var solutionsMessage = new SolutionsMessage()
                    {
                        Solutions = solutions,
                        ProblemType = message.ProblemType,
                        Id = message.Id,
                        CommonData = null, // or what?                        
                    };

                    EnqueueMessageToSend(solutionsMessage);

                }, message.ProblemType, message.Id, partialProblem.TaskId);
                if (!started)
                {
                    // tragedy, have to handle it someway
                }
            }
        }
    }
}
