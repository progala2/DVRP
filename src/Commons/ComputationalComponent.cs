using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using UCCTaskSolver;

namespace _15pl04.Ucc.Commons
{
    /// <summary>
    /// Base class for ComputationalNode and TaskManager.
    /// </summary>
    public abstract class ComputationalComponent
    {
        protected readonly byte _parallelThreads;

        protected Task _messagingTask;
        protected ComputationalTask[] _computationalTasks;
        protected ConcurrentQueue<Message> _messagesToSend;
        protected CancellationTokenSource _cancellationTokenSource;

        // dictionary of TaskSolvers; the keys are names of problems
        protected Dictionary<string, TaskSolver> _taskSolvers;

        protected IPEndPoint _serverAddress;
        protected TcpClient _tcpClient;
        protected Marshaller _marshaller;

        protected ulong _id;
        protected uint _timeout;

        /* it could be a List<IPEndPoint> but messages from server give information about
         * backup servers with List<BackupCommunicationServer> so keeping it this way allows to
         * parse to IPEndPoint only after primary server crash
         */
        protected List<BackupCommunicationServer> _backupCommunicationServers;

        public ComputationalComponent(IPEndPoint serverAddress)
        {
            _serverAddress = serverAddress;
            _tcpClient = new TcpClient(_serverAddress);

            _taskSolvers = GetTaskSolvers();
            _marshaller = new Marshaller();
            _messagesToSend = new ConcurrentQueue<Message>();

            // information for registration message; probably it is a temporary solution
            _parallelThreads = (byte)Environment.ProcessorCount;

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.Token.Register(() =>
            {
                _messagingTask = null;
                if (_computationalTasks != null)
                {
                    for (int i = 0; i < _computationalTasks.Length; i++)
                    {
                        _computationalTasks[i] = null;
                    }
                }
            });
        }

        public void Start()
        {
            // get RegisterMessage
            var registerMessage = GetRegisterMessage();
            // TRY to send it and get response with _tcpClient.SendData(marshalledMessage);
            throw new NotImplementedException();
            // save information from response
            throw new NotImplementedException();

            _computationalTasks = new ComputationalTask[_parallelThreads];
            for (int i = 0; i < _computationalTasks.Length; i++)
            {
                _computationalTasks[i] = new ComputationalTask();
            }

            // start informing about status(es) of threads
            _messagingTask = new Task(() => MessagesProcessing(), _cancellationTokenSource.Token);
            _messagingTask.Start();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Gets RegisterMessage specified for this component.
        /// </summary>
        /// <returns>A proper RegisterMessage.</returns>
        protected abstract RegisterMessage GetRegisterMessage();

        /// <summary>
        /// Handles a message received from server.
        /// </summary>
        /// <param name="message">A message to handle. It is received from server.</param>
        /// <remarks>
        /// Here should be started computationl tasks.
        /// </remarks>
        protected abstract void HandleResponseMessage(Message message);


        private void HandleResponseMessages(Message[] messages)
        {
            foreach (var message in messages)
            {
                HandleResponseMessage(message);
            }
        }

        /// <summary>
        /// Gets dictionary with names of solvable problems as keys and proper TaskSolvers as values.
        /// </summary>
        /// <returns>A dictionary with names of solvable problems as keys and proper TaskSolvers as values.</returns>
        private Dictionary<string, TaskSolver> GetTaskSolvers()
        {
            var result = new Dictionary<string, TaskSolver>();

            // add <key,value> pairs based on types derived from TaskSolver in *.dll files
            throw new NotImplementedException();

            return result;
        }

        private void MessagesProcessing()
        {
            try
            {
                Message message;
                Message[] responseMessages;
                while (true)
                {
                    while (_messagesToSend.TryDequeue(out message))
                    {
                        // send message
                        responseMessages = SendMessage(message);
                        // and handle response
                        HandleResponseMessages(responseMessages);
                    }

                    // send status message
                    var statusMessage = GetStatusMessage();
                    responseMessages = SendMessage(statusMessage);
                    // and handle response
                    HandleResponseMessages(responseMessages);

                    // _timeout / 2 is just a proposition
                    Thread.Sleep((int)(_timeout / 2));
                }
            }
            catch (TaskCanceledException)
            {
                // component stops... nothing to do
            }
        }

        private StatusMessage GetStatusMessage()
        {
            // create StatusMessage based on _id and _computationalTasks fields
            throw new NotImplementedException();
        }

        private Message[] SendMessage(Message message)
        {
            // just because it should be check for possible exceptions
            throw new NotImplementedException();


            var messagesToSend = new Message[] { message };
            var dataToSend = _marshaller.Marshall(messagesToSend);

            var dataReceived = _tcpClient.SendData(dataToSend);

            var messagesReceived = _marshaller.Unmarshall(dataReceived);
            return messagesReceived;
        }
    }
}
