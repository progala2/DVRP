using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Tasks.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    internal class MessageProcessor
    {
        public delegate void MessageReceptionEventHandler(object sender, MessageReceptionEventArgs e);
        public event MessageReceptionEventHandler MessageReception;

        private InputMessageQueue _inputQueue;
        private Marshaller _marshaller;
        private Task _processingThread;
        private uint _communicationTimeout;

        public MessageProcessor(Marshaller marshaller, uint communicationTimeout)
        {
            _inputQueue = new InputMessageQueue();
            _marshaller = marshaller;
            _communicationTimeout = communicationTimeout;
        }

        public void EnqeueInputMessage(byte[] rawMsg, AsyncTcpServer.ResponseCallback callback)
        {
            _inputQueue.Enqueue(rawMsg, callback);

            if (_processingThread == null)
            {
                _processingThread = new Task(ProcessQueuedMessages);
                _processingThread.Start();
            }
        }

        public void EnqueueOutputMessage(ComponentType addresseeType, Message msg)
        {
            // legacy (nie zmieniać dopóki Rad nie skończy pracy nad synchronizacją)
        }

        private void ProcessQueuedMessages()
        {
            while (true)
            {
                byte[] rawMsg;
                AsyncTcpServer.ResponseCallback callback;

                if (_inputQueue.TryDequeue(out rawMsg, out callback))
                {
                    Message[] input = _marshaller.Unmarshall(rawMsg);

                    foreach (var message in input)
                    {
                        ColorfulConsole.WriteMessageInfo("Received", message);

                        var responseMsgs = GetResponseMessages(message);

                        ColorfulConsole.WriteMessageInfo("Sending", responseMsgs);
                        var rawResponse = _marshaller.Marshall(new Message[] { responseMsgs });
                        new Task(() => { callback(rawResponse); }).Start();
                    }
                }
                else
                {
                    _processingThread = null;
                    return;
                }
            }
        }

        private Message GetResponseMessages(Message msg)
        {
            switch (msg.MessageType)
            {
                //for AsyncTcp tests to work, from pie/architecture, can (even should) be replaced later
                case MessageClass.Register:
                    {
                        var registerMsg = msg as RegisterMessage;

                        ulong id;

                        switch (registerMsg.ComponentType)
                        {
                            case ComponentType.ComputationalNode:
                            case ComponentType.TaskManager:
                                id = ComponentMonitor.Instance.RegisterNode(registerMsg.ComponentType, registerMsg.ParallelThreads, registerMsg.SolvableProblems);
                                break;

                            //TODO - case ComponentType.CommunicationServer:

                            default:
                                throw new Exception("Wrong type of component is trying to register.");
                        }

                        var registerResponseMsg = new RegisterResponseMessage()
                        {
                            AssignedId = id,
                            BackupServers = new List<ServerInfo>(),
                            CommunicationTimeout = _communicationTimeout,
                        };
                        return registerResponseMsg;
                    }

                //for AsyncTcp tests to work, from pie/architecture, can (even should) be replaced later
                case MessageClass.Status:
                    {
                        var statusMsg = msg as StatusMessage;
                        // TODO - implement
                        if (ComponentMonitor.Instance.IsRegistered(statusMsg.Id))
                        {
                            ComponentMonitor.Instance.UpdateTimestamp(statusMsg.Id);

                            var noOperationMsg = new NoOperationMessage()
                            {
                                BackupServers = new List<ServerInfo>(),
                            };
                            return noOperationMsg;
                        }
                        else
                        {
                            var errorMsg = new ErrorMessage()
                            {
                                ErrorText = "Unregistered component error.",
                                ErrorType = ErrorType.UnknownSender,
                            };
                            return errorMsg;
                        }
                    }

                case MessageClass.SolveRequest:
                    {
                        var solveRequestMsg = msg as SolveRequestMessage;

                        ulong id = Ucc.CommunicationServer.Tasks.TaskScheduler.Instance.GenerateProblemInstanceId();
                        ulong solvingTimeout = solveRequestMsg.SolvingTimeout.GetValueOrDefault(0);

                        var problemInstance = new ProblemInstance(id, solveRequestMsg.ProblemType, solveRequestMsg.Data, solvingTimeout);
                        Ucc.CommunicationServer.Tasks.TaskScheduler.Instance.AddNewProblemInstance(problemInstance);

                        var solveRequestResponseMsg = new SolveRequestResponseMessage()
                        {
                            Id = problemInstance.Id
                        };
                        return solveRequestResponseMsg;
                    }

                case MessageClass.SolutionRequest:
                    {
                        var solutionRequestMsg = msg as SolutionRequestMessage;

                        ulong id = solutionRequestMsg.ProblemInstanceId;
                        FinalSolution fs;
                        SolutionsMessage solutionMsg;

                        if (Ucc.CommunicationServer.Tasks.TaskScheduler.Instance.TryGetFinalSolution(id, out fs))
                        {
                            var ss = new Solution()
                            {
                                ComputationsTime = fs.ComputationsTime,
                                Data = fs.SolutionData,
                                PartialProblemId = fs.ProblemInstanceId,
                                TimeoutOccured = fs.TimeoutOccured,
                                Type = Solution.SolutionType.Final
                            };

                            solutionMsg = new SolutionsMessage()
                            {
                                ProblemInstanceId = id,
                                CommonData = null,
                                ProblemType = fs.ProblemType,
                                Solutions = new List<Solution>() { ss },
                            };
                        }
                        else
                        {
                            // TODO - stuff below is merely a temporary solution
                            var ss = new Solution()
                            {
                                ComputationsTime = 0,
                                Data = new byte[0],
                                PartialProblemId = id,
                                TimeoutOccured = true,
                                Type = Solution.SolutionType.Ongoing
                            };
                            solutionMsg = new SolutionsMessage()
                            {
                                ProblemInstanceId = id,
                                CommonData = null,
                                ProblemType = "dummy",
                                Solutions = new List<Solution>() { ss },
                            };
                        }
                        return solutionMsg;
                    }

                case MessageClass.SolvePartialProblems:
                    {
                        var partialProblemsMsg = msg as PartialProblemsMessage;

                        // TODO

                        return new NoOperationMessage();
                    }

                case MessageClass.Solutions:
                    {
                        var solutionsMessage = msg as SolutionsMessage;

                        // TODO

                        return new NoOperationMessage();
                    }
                default:
                    throw new Exception("Unsupported type received: " + msg.MessageType.ToString());
            }
        }

        // TODO additional MessageProcessor for backup CS
        // TODO check if senders are registered

    }
}
