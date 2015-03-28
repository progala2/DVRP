using System;
using System.Collections.Generic;
using System.Net;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;
using UCCTaskSolver;

namespace _15pl04.Ucc.ComputationalNode
{
    public sealed class ComputationalNode : ComputationalComponent
    {
        public ComputationalNode(IPEndPoint serverAddress)
            : base(serverAddress)
        {
        }

        /// <summary>
        /// Gets proper register message for this ComputationalComponent.
        /// </summary>
        /// <returns>A proper RegisterMessage.</returns>
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

        /// <summary>
        /// Handles any message received from server after registration process completes successfully.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        /// <remarks>
        /// RegisterResponse is handled in base class.
        /// </remarks>
        protected override void HandleResponseMessage(Message message)
        {
            switch (message.MessageType)
            {
                case Message.MessageClassType.NoOperation:
                    NoOperationMessageHandler((NoOperationMessage)message);
                    break;
                case Message.MessageClassType.PartialProblems:
                    PartialProblemsMessageHandler((PartialProblemsMessage)message);
                    break;
                case Message.MessageClassType.Error:
                    ErrorMessageHandler((ErrorMessage)message);
                    break;
                default:
                    throw new InvalidOperationException("Received not supported message type.");
            }
        }

        private void NoOperationMessageHandler(NoOperationMessage message)
        {
            throw new NotImplementedException();
        }

        /// <exception cref="System.InvalidOperationException">Thrown when:
        /// - problem type can't be solved with this ComputationalNode,
        /// - received more partial problems than can be currently started.</exception>
        private void PartialProblemsMessageHandler(PartialProblemsMessage message)
        {
            if (!TaskSolvers.ContainsKey(message.ProblemType))
            {
                // shouldn't ever get here - received unsolvable problem
                throw new InvalidOperationException(string.Format("\"{0}\" problem type can't be solved with this ComputationalNode.", message.ProblemType));
            }
            var taskSolverType = TaskSolvers[message.ProblemType];
            var timeout = message.SolvingTimeout.HasValue ? TimeSpan.FromMilliseconds((double)message.SolvingTimeout.Value) : TimeSpan.MaxValue;
            foreach (var partialProblem in message.PartialProblems)
            {
                /* each partial problem should be started properly cause server sends at most 
                 * as many partial problems as count of component's tasks in idle state */
                bool started = ComputationalTaskPool.StartComputationalTask(() =>
                {
                    // not sure if TaskSolver can change CommonData during computations so recreate it for each partial problem
                    var taskSolver = (TaskSolver)Activator.CreateInstance(taskSolverType, message.CommonData);

                    // measure time using DateTime cause StopWatch is not guaranteed to be thread safe
                    var start = DateTime.UtcNow;
                    var partialProblemSolutionData = taskSolver.Solve(partialProblem.Data, timeout);
                    var stop = DateTime.UtcNow;

                    var solutions = new List<SolutionsSolution>();
                    solutions.Add(new SolutionsSolution()
                    {
                        TaskId = partialProblem.TaskId,
                        TimeoutOccured = taskSolver.State == TaskSolver.TaskSolverState.Timeout,
                        Type = SolutionType.Partial,
                        ComputationsTime = (ulong)(stop - start).TotalMilliseconds,
                        Data = partialProblemSolutionData,
                    });
                    var solutionsMessage = new SolutionsMessage()
                    {
                        ProblemType = message.ProblemType,
                        Id = message.Id,
                        CommonData = message.CommonData,
                        Solutions = solutions,
                    };

                    EnqueueMessageToSend(solutionsMessage);

                }, message.ProblemType, message.Id, partialProblem.TaskId);
                if (!started)
                {
                    // tragedy, CommunicationServer surprised us like the Spanish Inquisition
                    throw new InvalidOperationException("Received more partial problems than can be currently started.");
                }
            }
        }

        private void ErrorMessageHandler(ErrorMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
