using System;
using System.Collections.Generic;
using System.Net;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;
using UCCTaskSolver;

namespace _15pl04.Ucc.TaskManager
{
    public sealed class TaskManager : ComputationalComponent
    {
        public TaskManager(IPEndPoint serverAddress)
            : base(serverAddress)
        {
        }

        /// <summary>
        /// Gets proper register message for this TaskManager.
        /// </summary>
        /// <returns>A proper RegisterMessage.</returns>
        protected override RegisterMessage GetRegisterMessage()
        {
            var registerMessage = new RegisterMessage()
            {
                Type = ComponentType.TaskManager,
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
                case Message.MessageClassType.DivideProblem:
                    DivideProblemMessageHandler((DivideProblemMessage)message);
                    break;
                case Message.MessageClassType.Solutions:
                    SolutionsMessageHandler((SolutionsMessage)message);
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
            // nothing to do, backuping is handled by MessageSender
        }

        /// <exception cref="System.InvalidOperationException">Thrown when:
        /// - message is designated for TaskManger with different ID,
        /// - problem type can't be solved with this TaskManger,
        /// - dividing problem cannot be started bacause no tasks are available in task pool.</exception>
        private void DivideProblemMessageHandler(DivideProblemMessage message)
        {
            if (ID != message.NodeId)
            {
                // shouldn't ever get here - received message for other TaskManager
                throw new InvalidOperationException(string.Format("TaskManager manager with ID={0} received message for TaskManager with ID={1}.", ID, message.NodeId));
            }
            if (!TaskSolvers.ContainsKey(message.ProblemType))
            {
                // shouldn't ever get here - received unsolvable problem
                throw new InvalidOperationException(string.Format("\"{0}\" problem type can't be divided with this TaskManager.", message.ProblemType));
            }
            var taskSolverType = TaskSolvers[message.ProblemType];

            // should be started properly cause server sends at most as many tasks to do as count of component's tasks in idle state
            bool started = ComputationalTaskPool.StartComputationalTask(() =>
            {
                var taskSolver = (TaskSolver)Activator.CreateInstance(taskSolverType, message.Data);

                // measure time using DateTime cause StopWatch is not guaranteed to be thread safe
                var start = DateTime.UtcNow;
                var partialProblemsData = taskSolver.DivideProblem((int)message.ComputationalNodes);
                var stop = DateTime.UtcNow;

                var partialProblems = new List<RegisterResponsePartialProblem>();
                for (int i = 0; i < partialProblemsData.GetLength(0); i++)
                {
                    partialProblems.Add(new RegisterResponsePartialProblem()
                    {
                        TaskId = (ulong)i,
                        Data = partialProblemsData[i],
                        NodeId = ID,
                    });
                }

                var partialProblemsMessage = new PartialProblemsMessage()
                {
                    ProblemType = message.ProblemType,
                    Id = message.Id,
                    CommonData = message.Data,
                    PartialProblems = partialProblems,
                };

                EnqueueMessageToSend(partialProblemsMessage);
            }, message.ProblemType, message.Id, null);
            if (!started)
            {
                // tragedy, CommunicationServer surprised us like the Spanish Inquisition
                throw new InvalidOperationException("Couldn't divide problem bacause no tasks are available in task pool.");
            }
        }

        private void SolutionsMessageHandler(SolutionsMessage message)
        {
            throw new NotImplementedException();
        }

        private void ErrorMessageHandler(ErrorMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
