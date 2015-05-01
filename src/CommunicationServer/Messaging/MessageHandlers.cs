using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.Components;
using _15pl04.Ucc.CommunicationServer.Components.Base;
using System;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    internal partial class MessageProcessor
    {
        private List<Message> HandleMessage<T>(T msg) where T : Message
        {
            return HandleMessage((dynamic)msg);
        }

        private List<Message> HandleMessage(RegisterMessage msg)
        {
            ComponentInfo componentInfo;
            if (msg.ComponentType == ComponentType.CommunicationServer)
            {
                throw new NotImplementedException(); // TODO registering backups
            }
            else if (msg.ComponentType == ComponentType.ComputationalNode ||
                msg.ComponentType == ComponentType.TaskManager)
            {
                componentInfo = new SolverNodeInfo(msg.ComponentType,
                    msg.SolvableProblems,
                    msg.ParallelThreads);
            }
            else
                throw new InvalidOperationException("Invalid component type registration (" + msg.ComponentType + ").");

            _componentOverseer.TryRegister(componentInfo);

            var responseMsg = new RegisterResponseMessage()
            {
                AssignedId = componentInfo.ComponentId.Value,
                BackupServers = new List<ServerInfo>(), // TODO acquire backup server list.
                CommunicationTimeout = _componentOverseer.CommunicationTimeout,
            };

            return new List<Message> { responseMsg };
        }
        private List<Message> HandleMessage(StatusMessage msg)
        {
            return null;
        }
        private List<Message> HandleMessage(SolveRequestMessage msg)
        {
            return null;
        }
        private List<Message> HandleMessage(SolutionRequestMessage msg)
        {
            return null;
        }
        private List<Message> HandleMessage(PartialProblemsMessage msg)
        {
            var partialProblemsMsg = msg as PartialProblemsMessage;

            ulong senderId = partialProblemsMsg.PartialProblems[0].TaskManagerId;
            if (!_componentOverseer.IsRegistered(senderId))
            {
                _logger.Warn("Received SolvePartialProblems message from an unregistered component (id: " + senderId + ").");
                var errorMsg = new ErrorMessage()
                {
                    ErrorType = ErrorType.UnknownSender,
                    ErrorText = "Component unregistered.",
                };

                return new List<Message> { errorMsg };
            }

            // TODO
            return null;
        }
        private List<Message> HandleMessage(SolutionsMessage msg)
        {
            return null;
        }
        private List<Message> HandleMessage(ErrorMessage msg)
        {
            var errorMsg = msg as ErrorMessage;

            _logger.Error(errorMsg.ErrorType + " error message received: \n" + errorMsg.ErrorText);

            return new List<Message>();
        }
    }
}
