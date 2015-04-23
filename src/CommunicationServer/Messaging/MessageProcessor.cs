using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Logging;
using _15pl04.Ucc.Commons.Messaging.Base;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Components;
using _15pl04.Ucc.CommunicationServer.Components.Base;
using _15pl04.Ucc.CommunicationServer.Messaging.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    internal class MessageProcessor : IDataProcessor
    {
        public bool IsProcessing
        {
            get { return _isProcessing; }
        }

        private static ILogger _logger = new ConsoleLogger();
        private RawDataQueue _inputDataQueue;

        private IMarshaller<Message> _marshaller;
        private IComponentOverseer _componentOverseer;
        private IWorkManager _workManager;

        private CancellationTokenSource _cancellationTokenSource;
        private AutoResetEvent _processingLock;

        private volatile bool _isProcessing;

        public MessageProcessor(IMarshaller<Message> marshaller, IComponentOverseer overseer, IWorkManager workManager)
        {
            _inputDataQueue = new RawDataQueue();

            _marshaller = marshaller;
            _componentOverseer = overseer;
            _workManager = workManager;

            _processingLock = new AutoResetEvent(false);
        }

        public void EnqueueDataToProcess(byte[] data, Metadata metadata, ProcessedDataCallback callback)
        {
            _inputDataQueue.Enqueue(data, metadata, callback);

            _processingLock.Set();
        }

        public void StartProcessing()
        {
            if (_isProcessing)
                return;

            _cancellationTokenSource = new CancellationTokenSource();

            var token = _cancellationTokenSource.Token;
            token.Register(() =>
            {
                _isProcessing = false;
            });

            new Task(() =>
            {
                while (true)
                {
                    if (_inputDataQueue.Count == 0)
                        _processingLock.WaitOne(); // No data available, wait for some.

                    RawDataQueueItem dataToProcess;
                    _inputDataQueue.TryDequeue(out dataToProcess);
                    if (dataToProcess != null)
                        ProcessData(dataToProcess); // Actual processing.

                    if (token.IsCancellationRequested)
                        return;
                }
            }, token).Start();

            _processingLock.Set();
            _isProcessing = true;
        }

        public void StopProcessing()
        {
            if (!_isProcessing)
                return;

            _cancellationTokenSource.Cancel();
            _processingLock.Set();
        }

        private void ProcessData(RawDataQueueItem data)
        {
            List<Message> messages = _marshaller.Unmarshall(data.Data);
            List<Message> responseMessages = new List<Message>();

            foreach(Message msg in messages)
            {
                _logger.Trace("Processing " + msg.MessageType + " message.");

                switch (msg.MessageType)
                {
                    case MessageClass.Register:
                        responseMessages.Add(HandleRegisterMessage((RegisterMessage)msg));
                        break;




                }
            }

            byte[] marshalledResponse = _marshaller.Marshall(responseMessages);
            data.Callback(marshalledResponse);
        }

        public Message HandleRegisterMessage(RegisterMessage msg)
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
                throw new InvalidOperationException("Invalid component type wants to register.");

            _componentOverseer.TryRegister(componentInfo);

            var responseMsg = new RegisterResponseMessage()
            {
                AssignedId = componentInfo.ComponentId.Value,
                BackupServers = new List<ServerInfo>(), // TODO acquire backup server list.
                CommunicationTimeout = 5000, // TODO change
            };

            return responseMsg;
        }
    }
}
