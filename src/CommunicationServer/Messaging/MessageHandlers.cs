﻿using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.Components;
using _15pl04.Ucc.CommunicationServer.Components.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;
using System;
using System.Collections.Generic;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    internal partial class MessageProcessor
    {
        private List<Message> HandleMessage<T>(T msg, TcpDataProviderMetadata metadata) where T : Message
        {
            return HandleMessage((dynamic)msg, metadata);
        }

        private List<Message> HandleMessage(RegisterMessage msg, TcpDataProviderMetadata metadata)
        {
            ComponentInfo componentInfo;

            switch (msg.ComponentType)
            {
                case ComponentType.CommunicationServer:
                    var serverInfo = new ServerInfo()
                    {
                        IpAddress = metadata.SenderAddress.Address.ToString(),
                        Port = (ushort)metadata.SenderAddress.Port,
                    };
                    componentInfo = new BackupServerInfo(serverInfo, msg.ParallelThreads);
                    break;

                case ComponentType.ComputationalNode:
                case ComponentType.TaskManager:
                    componentInfo = new SolverNodeInfo(msg.ComponentType, msg.SolvableProblems, msg.ParallelThreads);
                    break;

                default:
                    throw new InvalidOperationException("Invalid component type registration (" + msg.ComponentType + ").");
            }

            _componentOverseer.TryRegister(componentInfo);

            var responseMsg = new RegisterResponseMessage()
            {
                AssignedId = componentInfo.ComponentId.Value,
                BackupServers = CreateBackupList(),
                CommunicationTimeout = _componentOverseer.CommunicationTimeout,
            };

            return new List<Message> { responseMsg };
        }

        private List<Message> HandleMessage(StatusMessage msg, TcpDataProviderMetadata metadata)
        {
            List<Message> responses = new List<Message>();

            if (!_componentOverseer.IsRegistered(msg.ComponentId))
            {
                _logger.Warn("The component is not registered (id: " + msg.ComponentId + ").");
                var errorMsg = new ErrorMessage()
                {
                    ErrorType = ErrorType.UnknownSender,
                    ErrorText = "Component unregistered.",
                };

                responses.Add(errorMsg);
            }
            else
            {
                ComponentInfo component = _componentOverseer.GetComponent(msg.ComponentId);
                component.ThreadInfo = msg.Threads;

                switch (component.ComponentType)
                {
                    case ComponentType.ComputationalNode:
                    case ComponentType.TaskManager:
                        {
                            Work work;
                            _workManager.TryAssignWork((SolverNodeInfo)component, out work);

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

        private List<Message> HandleMessage(SolveRequestMessage msg, TcpDataProviderMetadata metadata)
        {
            ulong solvingTimeout = msg.SolvingTimeout.HasValue ? msg.SolvingTimeout.Value : 0;
            ulong id = _workManager.AddProblem(msg.ProblemType, msg.ProblemData, solvingTimeout);

            var response = new SolveRequestResponseMessage()
            {
                AssignedId = id,
            };

            return new List<Message> { response };
        }

        private List<Message> HandleMessage(SolutionRequestMessage msg, TcpDataProviderMetadata metadata)
        {
            Solution solution = _workManager.GetSolution(msg.ProblemInstanceId);
            Problem problem = _workManager.GetProblem(msg.ProblemInstanceId);

            Message response;

            if (solution != null)
            {
                var msgSolution = new SolutionsMessage.Solution()
                {
                    ComputationsTime = solution.ComputationsTime,
                    Data = solution.Data,
                    TimeoutOccured = solution.TimeoutOccured,
                    Type = SolutionsMessage.SolutionType.Final,
                };

                response = new SolutionsMessage()
                {
                    CommonData = problem.CommonData,
                    ProblemInstanceId = problem.Id,
                    ProblemType = problem.Type,
                    Solutions = new List<SolutionsMessage.Solution> { msgSolution },
                };
            }
            else if (problem != null)
            {
                var msgSolution = new SolutionsMessage.Solution()
                {
                    ComputationsTime = _workManager.GetComputationsTime(problem.Id),
                    TimeoutOccured = false,
                    Type = SolutionsMessage.SolutionType.Ongoing,
                };

                response = new SolutionsMessage()
                {
                    CommonData = problem.CommonData,
                    ProblemInstanceId = problem.Id,
                    ProblemType = problem.Type,
                    Solutions = new List<SolutionsMessage.Solution> { msgSolution },
                };
            }
            else
            {
                response = new ErrorMessage()
                {
                    ErrorType = ErrorType.ExceptionOccured,
                    ErrorText = "The specified problem id is unknown to the system.",
                };
            }

            return new List<Message> { response };
        }

        private List<Message> HandleMessage(PartialProblemsMessage msg, TcpDataProviderMetadata metadata)
        {
            List<Message> responses = new List<Message>();

            ulong? senderId = _workManager.GetProcessingNodeId(msg.ProblemInstanceId, msg.PartialProblems[0].PartialProblemId);

            if (!_componentOverseer.IsRegistered(senderId.Value))
            {
                _logger.Warn("The component is not registered (id: " + senderId + ").");
                var errorMsg = new ErrorMessage()
                {
                    ErrorType = ErrorType.UnknownSender,
                    ErrorText = "Component unregistered.",
                };

                responses.Add(errorMsg);
            }
            else
            {
                Problem problem = _workManager.GetProblem(msg.ProblemInstanceId);

                if (problem.CommonData != null)
                    _logger.Warn("Common data shouldn't be set.");

                problem.CommonData = msg.CommonData;
                foreach (var pp in msg.PartialProblems)
                    _workManager.AddPartialProblem(msg.ProblemInstanceId, pp.PartialProblemId, pp.Data);


                var component = _componentOverseer.GetComponent(senderId.Value) as SolverNodeInfo;
                Work work;
                _workManager.TryAssignWork((SolverNodeInfo)component, out work);

                if (work != null)
                    responses.Add(work.CreateMessage());
            }

            if (responses.Count == 0)
                responses.Add(CreateNoOperationMessage());

            return responses;
        }

        private List<Message> HandleMessage(SolutionsMessage msg, TcpDataProviderMetadata metadata)
        {
            List<Message> responses = new List<Message>();

            ulong? senderId = _workManager.GetProcessingNodeId(msg.ProblemInstanceId, msg.Solutions[0].PartialProblemId);

            if (!_componentOverseer.IsRegistered(senderId.Value))
            {
                _logger.Warn("The component is not registered (id: " + senderId.Value + ").");
                var errorMsg = new ErrorMessage()
                {
                    ErrorType = ErrorType.UnknownSender,
                    ErrorText = "Component unregistered.",
                };

                responses.Add(errorMsg);
            }
            else
            {
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
                        _workManager.AddPartialSolution(
                            msg.ProblemInstanceId,
                            solution.PartialProblemId.Value,
                            solution.Data,
                            solution.ComputationsTime,
                            solution.TimeoutOccured);
                    }
                }

                var component = _componentOverseer.GetComponent(senderId.Value) as SolverNodeInfo;
                Work work;
                _workManager.TryAssignWork((SolverNodeInfo)component, out work);

                if (work != null)
                    responses.Add(work.CreateMessage());
            }

            if (responses.Count == 0)
                responses.Add(CreateNoOperationMessage());

            return responses;
        }

        private List<Message> HandleMessage(ErrorMessage msg, TcpDataProviderMetadata metadata)
        {
            _logger.Error(msg.ErrorType + " error message received: \n" + msg.ErrorText);

            return new List<Message> { CreateNoOperationMessage() };
        }

        private NoOperationMessage CreateNoOperationMessage()
        {
            var noOperationMsg = new NoOperationMessage()
            {
                BackupServers = CreateBackupList(),
            };
            return noOperationMsg;
        }

        private List<ServerInfo> CreateBackupList()
        {
            List<ServerInfo> backupInfoToSend = new List<ServerInfo>();

            var backupServers = new List<ComponentInfo>(_componentOverseer.GetComponents(ComponentType.CommunicationServer));
            backupServers.Sort(ComponentInfo.RegistrationTimeComparer);

            foreach (BackupServerInfo bs in backupServers)
            {
                ServerInfo backupInfo = new ServerInfo()
                {
                    IpAddress = bs.Address.Address.ToString(),
                    Port = (ushort)bs.Address.Port,
                };

                backupInfoToSend.Add(backupInfo);
            }

            return backupInfoToSend;
        }
    }
}
