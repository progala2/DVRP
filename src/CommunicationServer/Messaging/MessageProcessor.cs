using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Logging;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Base;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Components;
using _15pl04.Ucc.CommunicationServer.Components.Base;
using _15pl04.Ucc.CommunicationServer.Messaging.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    internal partial class MessageProcessor : IDataProcessor
    {
        #region Public fields


        public bool IsProcessing
        {
            get { return _isProcessing; }
        }


        #endregion

        #region Private fields


        private static ILogger _logger = new TraceSourceLogger(typeof(MessageProcessor).Name);

        private RawDataQueue _inputDataQueue;

        private IMarshaller<Message> _marshaller;
        private IComponentOverseer _componentOverseer;
        private IWorkManager _workManager;

        private CancellationTokenSource _cancellationTokenSource;
        private AutoResetEvent _processingLock;

        private volatile bool _isProcessing;


        #endregion

        #region Constructors & control


        public MessageProcessor(IComponentOverseer overseer, IWorkManager workManager)
        {
            _inputDataQueue = new RawDataQueue();

            MessageSerializer serializer = new MessageSerializer();
            MessageValidator validator = new MessageValidator();
            _marshaller = new Marshaller(serializer, validator);

            _componentOverseer = overseer;
            _workManager = workManager;

            _processingLock = new AutoResetEvent(false);
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


        #endregion

        public void EnqueueDataToProcess(byte[] data, Metadata metadata, ProcessedDataCallback callback)
        {
            _inputDataQueue.Enqueue(data, metadata, callback);

            _processingLock.Set();
        }

        private void ProcessData(RawDataQueueItem data)
        {
            List<Message> messages = _marshaller.Unmarshall(data.Data);
            List<Message> responseMessages = new List<Message>();

            foreach (Message msg in messages)
            {
                _logger.Trace("Processing " + msg.MessageType + " message.");

                try
                {
                    HandleMessage(msg);
                }
                catch (SystemException e)
                {
                    _logger.Warn("Unsupported message type received (" + msg.MessageType + ").");
                    var errorMsg = new ErrorMessage()
                    {
                        ErrorType = ErrorType.InvalidOperation,
                        ErrorText = "Computational Server doesn't handle " + msg.MessageType + " message.",
                    };
                    responseMessages = new List<Message> { errorMsg };
                    break;
                }
            }

            byte[] marshalledResponse = _marshaller.Marshall(responseMessages);
            data.Callback(marshalledResponse);
        }
    }
}
