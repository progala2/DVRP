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
    /// <summary>
    ///     The task manager class.
    /// </summary>
    public sealed class TaskManager : ComputationalComponent
    {
        /// <summary>
        ///     Creates a task manager which looks for task solvers in current directory.
        /// </summary>
        /// <param name="threadManager">The thread manager. Cannot be null.</param>
        /// <param name="serverAddress">The primary server address. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="threadManager"/> or <paramref name="serverAddress"/> is null.</exception>
        public TaskManager(ThreadManager threadManager, IPEndPoint serverAddress)
            : base(threadManager, serverAddress)
        {
        }

        /// <summary>
        ///     Creates a task manager.
        /// </summary>
        /// <param name="threadManager">The thread manager. Cannot be null.</param>
        /// <param name="serverAddress">The primary server address. Cannot be null.</param>
        /// <param name="taskSolversDirectoryRelativePath">The relative path to directory with task solvers. If null current directory will be searched for task solvers.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="threadManager"/> or <paramref name="serverAddress"/> is null.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">TODO</exception>
        public TaskManager(ThreadManager threadManager, IPEndPoint serverAddress,
            string taskSolversDirectoryRelativePath)
            : base(threadManager, serverAddress, taskSolversDirectoryRelativePath)
        {
        }

        /// <summary>
        ///     The type of the component.
        /// </summary>
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
        /// <exception cref="System.InvalidOperationException">Thrown when received not supported message type.</exception>
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

        /// <summary>
        /// Handler for NoOperationMessage.
        /// </summary>
        /// <param name="message">A NoOperationMessage.</param>
        private void NoOperationMessageHandler(NoOperationMessage message)
        {
            // nothing to do, backuping is handled by MessageSender
        }

        /// <summary>
        /// Handler for DivideProblemMessage.
        /// </summary>
        /// <param name="message">A DivideProblemMessage.</param>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown when:
        ///     - message is designated for TaskManger with different ID,
        ///     - problem type can't be divided with this TaskManager,
        ///     - dividing the problem cannot be started bacause no threads are available in thread pool.
        /// </exception>
        private void DivideProblemMessageHandler(DivideProblemMessage message)
        {
            if (Id != message.TaskManagerId)
            {
                // shouldn't ever get here - received message for other TaskManager
                throw new InvalidOperationException(
                    string.Format("TaskManager with ID={0} received message for TaskManager with ID={1}.", Id,
                        message.TaskManagerId));
            }
            Type taskSolverType;
            if (!TaskSolvers.TryGetValue(message.ProblemType, out taskSolverType))
            {
                // shouldn't ever get here - received unsolvable problem
                throw new InvalidOperationException(
                    string.Format("\"{0}\" problem type can't be divided with this TaskManager.", message.ProblemType));
            }

            var actionDescription = string.Format("Dividing problem \"{0}\"(problem instance id={1})",
                message.ProblemType, message.ProblemInstanceId);
            // should be started properly cause server sends at most as many tasks to do as count of component's threads in idle state
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
                throw new InvalidOperationException(
                    "Couldn't divide problem because couldn't start new thread.");
            }
        }

        /// <summary>
        /// Handler for SolutionsMessage.
        /// </summary>
        /// <param name="message">A SolutionsMessage.</param>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown when:
        ///     - problem type can't be merged with this TaskManager,
        ///     - merging the problem cannot be started bacause no threads are available in thread pool.
        /// </exception>
        private void SolutionsMessageHandler(SolutionsMessage message)
        {
            Type taskSolverType;
            if (!TaskSolvers.TryGetValue(message.ProblemType, out taskSolverType))
            {
                // shouldn't ever get here - received unsolvable problem
                throw new InvalidOperationException(
                    string.Format("\"{0}\" problem type can't be merged with this TaskManager.", message.ProblemType));
            }

            var actionDescription = string.Format("Merging partial problems \"{0}\"(problem instance id={1})",
                message.ProblemType, message.ProblemInstanceId);
            // should be started properly cause server sends at most as many tasks to do as count of component's threads in idle state
            var started = StartActionInNewThread(() =>
            {
                ulong totalComputationsTime = 0;
                var timeoutOccured = false;
                var solutionsData = new byte[message.Solutions.Count][];
                for (var i = 0; i < message.Solutions.Count; i++)
                {
                    var solution = message.Solutions[i];

                    if (solution.Type != SolutionsMessage.SolutionType.Partial)
                        throw new InvalidOperationException(
                            string.Format("Received non-partial solution({0})(partial problem id={1}).",
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

                var finalSolution = new SolutionsMessage.Solution
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
                    Solutions = new List<SolutionsMessage.Solution> { finalSolution }
                };

                EnqueueMessageToSend(finalSolutionMessage);
            }, actionDescription, message.ProblemType, message.ProblemInstanceId, null);
            if (!started)
            {
                throw new InvalidOperationException(
                    "Couldn't merge partial solutions because couldn't start new thread.");
            }
        }

        /// <summary>
        /// Handler for ErrorMessage.
        /// </summary>
        /// <param name="message">An ErrorMessage.</param>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown when <paramref name="message"/> received from server contains information about server exception.
        /// </exception>
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
                    throw new InvalidOperationException(
                        "Information about exception on server shouldn't be send to component.");
            }
        }
    }
}