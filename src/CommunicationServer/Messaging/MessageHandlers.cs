﻿using System;
using System.Collections.Generic;
using Dvrp.Ucc.Commons.Components;
using Dvrp.Ucc.Commons.Messaging.Models;
using Dvrp.Ucc.Commons.Messaging.Models.Base;
using Dvrp.Ucc.CommunicationServer.Components;
using Dvrp.Ucc.CommunicationServer.Components.Base;
using Dvrp.Ucc.CommunicationServer.WorkManagement.Models;

namespace Dvrp.Ucc.CommunicationServer.Messaging
{
    internal partial class MessageProcessor
    {
        /// <summary>
        /// Handle message received from the cluster component.
        /// </summary>
        /// <typeparam name="T">Class-type of the received message.</typeparam>
        /// <param name="msg">Received message.</param>
        /// <param name="metadata">Information about data and the TCP connection it came from.</param>
        /// <returns>List of response messages.</returns>
        private List<Message> HandleMessageGeneric<T>(T msg, TcpDataProviderMetadata metadata) where T : Message
        {
            return HandleMessage((dynamic) msg, metadata);
        }

        /// <summary>
        /// Handle registration message.
        /// </summary>
        /// <param name="msg">Register message.</param>
        /// <param name="metadata">Information about data and the TCP connection it came from.</param>
        /// <returns>List of response messages.</returns>
        private List<Message> HandleMessage(RegisterMessage msg, TcpDataProviderMetadata metadata)
        {
            ComponentInfo componentInfo;
            switch (msg.ComponentType)
            {
                case ComponentType.CommunicationServer:
                    var serverInfo = new ServerInfo
                    {
                        IpAddress = metadata.SenderAddress.Address.ToString(),
                        Port = (ushort) metadata.SenderAddress.Port
                    };
                    componentInfo = new BackupServerInfo((ulong)IdGenerator.CreateId(), serverInfo, msg.ParallelThreads);
                    break;

                case ComponentType.ComputationalNode:
                case ComponentType.TaskManager:
                    componentInfo = new SolverNodeInfo((ulong)IdGenerator.CreateId(), msg.ComponentType, msg.SolvableProblems, msg.ParallelThreads);
                    break;

                default:
                    throw new InvalidOperationException("Invalid component type registration (" + msg.ComponentType + ").");
            }

            _componentOverseer.TryRegister(componentInfo);

            var responseMsg = new RegisterResponseMessage
            {
                AssignedId = componentInfo.ComponentId,
                BackupServers = CreateBackupList(),
                CommunicationTimeout = _componentOverseer.CommunicationTimeout
            };

            return new List<Message> {responseMsg};
        }

        /// <summary>
        /// Handle status message.
        /// </summary>
        /// <param name="msg">Status message.</param>
        /// <param name="metadata">Information about data and the TCP connection it came from.</param>
        /// <returns>List of response messages.</returns>
        private List<Message> HandleMessage(StatusMessage msg, TcpDataProviderMetadata metadata)
        {
            var responses = new List<Message>();

            if (!_componentOverseer.IsRegistered(msg.ComponentId))
            {
                Logger.Warn("The component is not registered (id: " + msg.ComponentId + ").");
                var errorMsg = new ErrorMessage
                {
                    ErrorType = ErrorType.UnknownSender,
                    ErrorText = "Component unregistered."
                };

                responses.Add(errorMsg);
            }
            else
            {
                _componentOverseer.UpdateTimestamp(msg.ComponentId);

                var component = _componentOverseer.GetComponent(msg.ComponentId);
                component.ThreadInfo = msg.Threads;

                switch (component.ComponentType)
                {
                    case ComponentType.ComputationalNode:
                    case ComponentType.TaskManager:
                    {
	                    _workManager.TryAssignWork((SolverNodeInfo) component, out var work);

                        if (work != null)
                            responses.Add(work.CreateMessage());
                        break;
                    }

                    case ComponentType.CommunicationServer:
                    {
                        //TODO get stuff from the sync queue
                        break;
                    }
                }
            }

            if (responses.Count == 0)
                responses.Add(CreateNoOperationMessage());

            return responses;
        }

        /// <summary>
        /// Handle solve request message.
        /// </summary>
        /// <param name="msg">Solve request message.</param>
        /// <param name="metadata">Information about data and the TCP connection it came from.</param>
        /// <returns>List of response messages.</returns>
        private List<Message> HandleMessage(SolveRequestMessage msg, TcpDataProviderMetadata metadata)
        {
            var solvingTimeout = msg.SolvingTimeout ?? ulong.MaxValue;
            var id = _workManager.AddProblem(msg.ProblemType, msg.ProblemData, solvingTimeout);

            var response = new SolveRequestResponseMessage
            {
                AssignedId = id
            };

            return new List<Message> {response};
        }

        /// <summary>
        /// Handle solution request message.
        /// </summary>
        /// <param name="msg">Solution request message.</param>
        /// <param name="metadata">Information about data and the TCP connection it came from.</param>
        /// <returns>List of response messages.</returns>
        private List<Message> HandleMessage(SolutionRequestMessage msg, TcpDataProviderMetadata metadata)
        {
            var solution = _workManager.GetSolution(msg.ProblemInstanceId);
            var problem = _workManager.GetProblem(msg.ProblemInstanceId);

            Message response;

            if (solution != null)
            {
                problem = solution.Problem;

                var msgSolution = new SolutionsMessage.Solution
                {
                    ComputationsTime = solution.ComputationsTime,
                    Data = solution.Data,
                    TimeoutOccured = solution.TimeoutOccured,
                    Type = SolutionsMessage.SolutionType.Final
                };

                response = new SolutionsMessage
                {
                    CommonData = problem.CommonData,
                    ProblemInstanceId = problem.Id,
                    ProblemType = problem.Type,
                    Solutions = new List<SolutionsMessage.Solution> {msgSolution}
                };
            }
            else if (problem != null)
            {
                var msgSolution = new SolutionsMessage.Solution
                {
                    ComputationsTime = _workManager.GetComputationsTime(problem.Id),
                    TimeoutOccured = false,
                    Type = SolutionsMessage.SolutionType.Ongoing
                };

                response = new SolutionsMessage
                {
                    CommonData = problem.CommonData,
                    ProblemInstanceId = problem.Id,
                    ProblemType = problem.Type,
                    Solutions = new List<SolutionsMessage.Solution> {msgSolution}
                };
            }
            else
            {
                response = new ErrorMessage
                {
                    ErrorType = ErrorType.ExceptionOccured,
                    ErrorText = "The specified problem id is unknown to the system."
                };
            }

            return new List<Message> {response};
        }

        /// <summary>
        /// Handle partial problems message.
        /// </summary>
        /// <param name="msg">Partial problems message.</param>
        /// <param name="metadata">Information about data and the TCP connection it came from.</param>
        /// <returns>List of response messages.</returns>
        private List<Message> HandleMessage(PartialProblemsMessage msg, TcpDataProviderMetadata metadata)
        {
            var responses = new List<Message>();
            Problem? problem;
            var senderId = _workManager.GetProcessingNodeId(msg.ProblemInstanceId);

            if (!senderId.HasValue)
            {
                Logger.Error("Couldn't identify the sender.");
                var errorMsg = new ErrorMessage
                {
                    ErrorType = ErrorType.UnknownSender,
                    ErrorText = "The server couldn't identify the sender component."
                };

                responses.Add(errorMsg);
            }
            else if (!_componentOverseer.IsRegistered(senderId.Value) || _componentOverseer.GetComponent(senderId.Value) is not SolverNodeInfo component)
            {
                Logger.Warn("The component is not registered (id: " + senderId + ").");
                var errorMsg = new ErrorMessage
                {
                    ErrorType = ErrorType.UnknownSender,
                    ErrorText = "Component unregistered."
                };

                responses.Add(errorMsg);
            }
            else if ((problem = _workManager.GetProblem(msg.ProblemInstanceId)) == null)
            {
	            Logger.Warn("The problem is not registered (id: " + msg.ProblemInstanceId + ").");
	            var errorMsg = new ErrorMessage
	            {
		            ErrorType = ErrorType.UnknownSender,
		            ErrorText = "Problem unregistered."
	            };

	            responses.Add(errorMsg);
            }
            else
            {
	            _componentOverseer.UpdateTimestamp(senderId.Value);

                if (problem.CommonData != null)
                    Logger.Warn("Common data shouldn't be set.");

                problem.CommonData = msg.CommonData;
                foreach (var pp in msg.PartialProblems)
                    _workManager.AddPartialProblem(msg.ProblemInstanceId, pp.PartialProblemId, pp.Data);

                _workManager.TryAssignWork(component, out var work);

                if (work != null)
                    responses.Add(work.CreateMessage());
            }

            if (responses.Count == 0)
                responses.Add(CreateNoOperationMessage());

            return responses;
        }

        /// <summary>
        /// Handle solutions message.
        /// </summary>
        /// <param name="msg">Solutions message.</param>
        /// <param name="metadata">Information about data and the TCP connection it came from.</param>
        /// <returns>List of response messages.</returns>
        private List<Message> HandleMessage(SolutionsMessage msg, TcpDataProviderMetadata metadata)
        {
            var responses = new List<Message>();

            var senderId = _workManager.GetProcessingNodeId(msg.ProblemInstanceId, msg.Solutions[0].PartialProblemId);

            if (!senderId.HasValue)
            {
                Logger.Error("Couldn't identify the sender.");
                var errorMsg = new ErrorMessage
                {
                    ErrorType = ErrorType.UnknownSender,
                    ErrorText = "The server couldn't identify the sender component."
                };

                responses.Add(errorMsg);
            }
            else if (!_componentOverseer.IsRegistered(senderId.Value) || _componentOverseer.GetComponent(senderId.Value) is not SolverNodeInfo component)
            {
                Logger.Warn("The component is not registered (id: " + senderId.Value + ").");
                var errorMsg = new ErrorMessage
                {
                    ErrorType = ErrorType.UnknownSender,
                    ErrorText = "Component unregistered."
                };

                responses.Add(errorMsg);
            }
            else
            {
                _componentOverseer.UpdateTimestamp(senderId.Value);

                foreach (var solution in msg.Solutions)
                {
                    if (solution.Type == SolutionsMessage.SolutionType.Final)
                    {
                        _workManager.AddSolution(
                            msg.ProblemInstanceId,
                            solution.Data,
                            solution.ComputationsTime,
                            solution.TimeoutOccured);
                    }
                    else if (solution.Type == SolutionsMessage.SolutionType.Partial)
                    {
                        if (!solution.PartialProblemId.HasValue)
                        {
                            Logger.Error("Received partial solution doesn't have partial problem id set.");
                            continue;
                        }

                        _workManager.AddPartialSolution(
                            msg.ProblemInstanceId,
                            solution.PartialProblemId.Value,
                            solution.Data,
                            solution.ComputationsTime,
                            solution.TimeoutOccured);
                    }
                }

                _workManager.TryAssignWork(component, out var work);

                if (work != null)
                    responses.Add(work.CreateMessage());
            }

            if (responses.Count == 0)
                responses.Add(CreateNoOperationMessage());

            return responses;
        }

        /// <summary>
        /// Handle error message.
        /// </summary>
        /// <param name="msg">Error message.</param>
        /// <param name="metadata">Information about data and the TCP connection it came from.</param>
        /// <returns>List of response messages.</returns>
        private List<Message> HandleMessage(ErrorMessage msg, TcpDataProviderMetadata metadata)
        {
            Logger.Error(msg.ErrorType + " error message received: \n" + msg.ErrorText);

            return new List<Message> {CreateNoOperationMessage()};
        }

        /// <summary>
        /// Generate no-operation message.
        /// </summary>
        /// <returns>No operation message.</returns>
        private NoOperationMessage CreateNoOperationMessage()
        {
            var noOperationMsg = new NoOperationMessage
            {
                BackupServers = CreateBackupList()
            };
            return noOperationMsg;
        }

        /// <summary>
        /// Generate current backup server list.
        /// </summary>
        /// <returns>List of information about currently registered backup servers.</returns>
        private List<ServerInfo> CreateBackupList()
        {
            var backupInfoToSend = new List<ServerInfo>();

            var backupServers =
                new List<ComponentInfo>(_componentOverseer.GetComponents(ComponentType.CommunicationServer));
            backupServers.Sort(ComponentInfo.RegistrationTimeComparer);

            foreach (var componentInfo in backupServers)
            {
                var bs = (BackupServerInfo) componentInfo;
                var backupInfo = new ServerInfo
                {
                    IpAddress = bs.Address.Address.ToString(),
                    Port = (ushort) bs.Address.Port
                };

                backupInfoToSend.Add(backupInfo);
            }

            return backupInfoToSend;
        }
    }
}