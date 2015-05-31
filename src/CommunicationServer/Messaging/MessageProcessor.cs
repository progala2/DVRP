using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _15pl04.Ucc.Commons.Logging;
using _15pl04.Ucc.Commons.Messaging.Marshalling;
using _15pl04.Ucc.Commons.Messaging.Marshalling.Base;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Components.Base;
using _15pl04.Ucc.CommunicationServer.Messaging.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using Microsoft.CSharp.RuntimeBinder;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    /// <summary>
    /// Module responsible for processing and generating output for incoming messages.
    /// </summary>
    internal partial class MessageProcessor : IDataProcessor
    {
        private static readonly ILogger Logger = new ConsoleLogger();
        private readonly IComponentOverseer _componentOverseer;
        private readonly RawDataQueue _inputDataQueue;
        private readonly IMarshaller<Message> _marshaller;
        private readonly AutoResetEvent _processingLock;
        private readonly IWorkManager _workManager;
        private CancellationTokenSource _cancellationTokenSource;
        private volatile bool _isProcessing;

        /// <summary>
        /// Creates MessageProcessor instance.
        /// </summary>
        /// <param name="componentOverseer">Component overseer module.</param>
        /// <param name="workManager">Work manager to get work from.</param>
        public MessageProcessor(IComponentOverseer componentOverseer, IWorkManager workManager)
        {
            if (componentOverseer == null)
                throw new ArgumentNullException("IComponentOverseer dependancy is null.");
            if (workManager == null)
                throw new ArgumentNullException("IWorkManager dependancy is null.");

            _inputDataQueue = new RawDataQueue();

            var serializer = new MessageSerializer();
            var validator = new MessageValidator();
            _marshaller = new Marshaller(serializer, validator);

            _componentOverseer = componentOverseer;
            _workManager = workManager;

            _processingLock = new AutoResetEvent(false);
        }

        /// <summary>
        /// True if the message processor is processing messages. False otherwise.
        /// </summary>
        public bool IsProcessing
        {
            get { return _isProcessing; }
        }

        /// <summary>
        /// Enqueues new raw message to process.
        /// </summary>
        /// <param name="data">Raw message data.</param>
        /// <param name="metadata">Information about received data.</param>
        /// <param name="callback">Callback invoked after processing containing marshalled response messages.</param>
        public void EnqueueDataToProcess(byte[] data, Metadata metadata, ProcessedDataCallback callback)
        {
            _inputDataQueue.Enqueue(data, metadata, callback);

            _processingLock.Set();
        }

        /// <summary>
        /// Starts processing messages.
        /// </summary>
        public void StartProcessing()
        {
            if (_isProcessing)
                return;

            _cancellationTokenSource = new CancellationTokenSource();

            var token = _cancellationTokenSource.Token;
            token.Register(() => { _isProcessing = false; });

            var task = new Task(() =>
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
            }, token);

            task.ContinueWith(t =>
            {
                Logger.Error(t.Exception.ToString());
                Logger.Error(t.Exception.StackTrace);
            }, TaskContinuationOptions.OnlyOnFaulted);

            task.Start();

            _processingLock.Set();
            _isProcessing = true;
        }

        /// <summary>
        /// Stop processing messages.
        /// </summary>
        public void StopProcessing()
        {
            if (!_isProcessing)
                return;

            _cancellationTokenSource.Cancel();
            _processingLock.Set();
        }

        /// <summary>
        /// Process item from raw data queue.
        /// </summary>
        /// <param name="data">Dequeued data.</param>
        private void ProcessData(RawDataQueueItem data)
        {
            var messages = _marshaller.Unmarshall(data.Data);
            var responseMessages = new List<Message>();

            foreach (var msg in messages)
            {
                Logger.Trace("Processing " + msg.MessageType + " message.");

                try
                {
                    var metadata = data.Metadata as TcpDataProviderMetadata;
                    var response = HandleMessageGeneric(msg, metadata);
                    responseMessages.AddRange(response);
                }
                catch (RuntimeBinderException e)
                {
                    Logger.Debug(e.Message);
                    Logger.Warn("Unsupported message type received (" + msg.MessageType + ").");
                    var errorMsg = new ErrorMessage
                    {
                        ErrorType = ErrorType.InvalidOperation,
                        ErrorText = "Computational Server doesn't handle " + msg.MessageType + " message."
                    };
                    responseMessages = new List<Message> { errorMsg };
                    break;
                }
            }

            foreach (var msgToSend in responseMessages)
                Logger.Trace("Sending " + msgToSend.MessageType + " message.");

            var marshalledResponse = _marshaller.Marshall(responseMessages);
            data.Callback(marshalledResponse);
        }
    }
}