using System;
using System.Collections.Generic;
using System.Net;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Computations.Base;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using _15pl04.Ucc.Commons.Utilities;
using UCCTaskSolver;

namespace _15pl04.Ucc.TaskManager
{
    public sealed class TaskManager : ComputationalComponent
    {
        /// <summary>
        ///     Creates TaskManager which looks for task solvers in current directory.
        /// </summary>
        /// <param name="threadManager">The thread manager. Cannot be null.</param>
        /// <param name="serverAddress">The primary server address. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public TaskManager(ThreadManager threadManager, IPEndPoint serverAddress)
            : base(threadManager, serverAddress)
        {
        }

        /// <summary>
        ///     Creates TaskManager.
        /// </summary>
        /// <param name="threadManager">The thread manager. Cannot be null.</param>
        /// <param name="serverAddress">The primary server address. Cannot be null.</param>
        /// <param name="taskSolversDirectoryRelativePath">The relative path to directory with task solvers.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        public TaskManager(ThreadManager threadManager, IPEndPoint serverAddress,
            string taskSolversDirectoryRelativePath)
            : base(threadManager, serverAddress, taskSolversDirectoryRelativePath)
        {
        }

        public override ComponentType ComponentType
        {
            get { return ComponentType.TaskManager; }
        }

        /// <summary>
        ///     Handles any message received from server after registration process completes successfully.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        /// <remarks>
        ///     RegisterResponse is handled in base class.
        /// </remarks>
        protected override void HandleReceivedMessage(Message message)
        {
            switch (message.MessageType)
            {
                case MessageClass.NoOperation:
                    NoOperationMessageHandler((NoOperationMessage)message);
                    break;
                case MessageClass.DivideProblem:
                    DivideProblemMessageHandler((DivideProblemMessage)message);
                    break;
                case MessageClass.Solutions:
                    SolutionsMessageHandler((SolutionsMessage)message);
                    break;
                case MessageClass.Error:
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

        /// <exception cref="System.InvalidOperationException">
        ///     Thrown when:
        ///     - message is designated for TaskManger with different ID,
        ///     - problem type can't be solved with this TaskManger,
        ///     - dividing problem cannot be started bacause no tasks are available in task pool.
        /// </exception>
        private void DivideProblemMessageHandler(DivideProblemMessage message)
        {
            if (Id != message.TaskManagerId)
            {
                // shouldn't ever get here - received message for other TaskManager
                throw new InvalidOperationException(
                    string.Format("TaskManager manager with ID={0} received message for TaskManager with ID={1}.", Id,
                        message.TaskManagerId));
            }
            if (!TaskSolvers.ContainsKey(message.ProblemType))
            {
                // shouldn't ever get here - received unsolvable problem
                throw new InvalidOperationException(
                    string.Format("\"{0}\" problem type can't be divided with this TaskManager.", message.ProblemType));
            }
            var taskSolverType = TaskSolvers[message.ProblemType];

            var actionDescription = string.Format("Dividing problem \"{0}\"(problem instance id={1})",
                message.ProblemType, message.ProblemInstanceId);
            // should be started properly cause server sends at most as many tasks to do as count of component's tasks in idle state
            var started = StartActionInNewThread(() =>
            {
                var taskSolver = (TaskSolver)Activator.CreateInstance(taskSolverType, message.ProblemData);
                taskSolver.ThrowIfError();

                var partialProblemsData = taskSolver.DivideProblem((int)message.ComputationalNodes);
                taskSolver.ThrowIfError();

                var partialProblems = new List<PartialProblemsMessage.PartialProblem>(partialProblemsData.GetLength(0));
                for (var i = 0; i < partialProblemsData.GetLength(0); i++)
                {
                    partialProblems.Add(new PartialProblemsMessage.PartialProblem
                    {
                        PartialProblemId = (ulong)i,
                        Data = partialProblemsData[i],
                        TaskManagerId = Id
                    });
                }

                var partialProblemsMessage = new PartialProblemsMessage
                {
                    ProblemType = message.ProblemType,
                    ProblemInstanceId = message.ProblemInstanceId,
                    CommonData = message.ProblemData,
                    PartialProblems = partialProblems
                };

                EnqueueMessageToSend(partialProblemsMessage);
            }, actionDescription, message.ProblemType, message.ProblemInstanceId, null);
            if (!started)
            {
                // tragedy, CommunicationServer surprised us like the Spanish Inquisition
                throw new InvalidOperationException(
                    "Couldn't divide problem because couldn't start new thread.");
            }
        }

        private void SolutionsMessageHandler(SolutionsMessage message)
        {
            if (!TaskSolvers.ContainsKey(message.ProblemType))
            {
                // shouldn't ever get here - received unsolvable problem
                throw new InvalidOperationException(
                    string.Format("\"{0}\" problem type can't be merged with this TaskManager.", message.ProblemType));
            }
            var taskSolverType = TaskSolvers[message.ProblemType];

            var actionDescription = string.Format("Merging partial problems \"{0}\"(problem instance id={1})",
                message.ProblemType, message.ProblemInstanceId);
            // should be started properly cause server sends at most as many tasks to do as count of component's tasks in idle state
            var started = StartActionInNewThread(() =>
            {
                ulong totalComputationsTime = 0;
                var timeoutOccured = false;
                var solutionsData = new byte[message.Solutions.Count][];
                for (int i = 0; i < message.Solutions.Count; i++)
                {
                    var solution = message.Solutions[i];

                    if (solution.Type != SolutionsMessage.SolutionType.Partial)
                        throw new InvalidOperationException(string.Format("Received non-partial solution({0})(partial problem id={1}).",
                            solution.Type, solution.PartialProblemId));
                    totalComputationsTime += solution.ComputationsTime;
                    timeoutOccured |= solution.TimeoutOccured;
                    solutionsData[i] = solution.Data;
                }

                var taskSolver = (TaskSolver)Activator.CreateInstance(taskSolverType, message.CommonData);
                taskSolver.ThrowIfError();

                // measure time using DateTime cause StopWatch is not guaranteed to be thread safe
                var start = DateTime.UtcNow;
                var finalSolutionData = taskSolver.MergeSolution(solutionsData);
                var stop = DateTime.UtcNow;

                taskSolver.ThrowIfError();
                totalComputationsTime += (ulong)((stop - start).TotalMilliseconds);

                var finalSolution = new SolutionsMessage.Solution()
                {
                    Type = SolutionsMessage.SolutionType.Final,
                    ComputationsTime = totalComputationsTime,
                    TimeoutOccured = timeoutOccured,
                    Data = finalSolutionData
                };
                var finalSolutionMessage = new SolutionsMessage
                {
                    ProblemType = message.ProblemType,
                    ProblemInstanceId = message.ProblemInstanceId,
                    CommonData = message.CommonData,
                    Solutions = new List<SolutionsMessage.Solution>() { finalSolution }
                };

                EnqueueMessageToSend(finalSolutionMessage);
            }, actionDescription, message.ProblemType, message.ProblemInstanceId, null);
            if (!started)
            {
                // tragedy, CommunicationServer surprised us like the Spanish Inquisition
                throw new InvalidOperationException(
                    "Couldn't merge partial solutions because couldn't start new thread.");
            }
        }

        private void ErrorMessageHandler(ErrorMessage message)
        {
            switch (message.ErrorType)
            {
                case ErrorType.UnknownSender:
                    Register();
                    break;
                case ErrorType.InvalidOperation:
                    // nothing to do
                    break;
                case ErrorType.ExceptionOccured:
                    throw new InvalidOperationException("Information about exception on server shouldn't be send to component.");
            }
        }
    }
}