using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using UCCTaskSolver;

namespace _15pl04.Ucc.ComputationalNode
{
    public class ComputationalNode
    {
        private readonly byte _parallelThreads;

        private Task _messagingTask;
        private ComputationalTask[] _computationalTasks;
        private ConcurrentQueue<Message> _messagesToSend;
        private CancellationTokenSource _cancellationTokenSource;

        // dictionary of TaskSolvers; the keys are names of problems
        private Dictionary<string, TaskSolver> _taskSolvers;

        private IPEndPoint _serverAddress;
        private TcpClient _tcpClient;
        private Marshaller _marshaller;
        private ulong _id;
        private uint _timeout;


        /* it could be a List<IPEndPoint> but messages from server give information about
         * backup servers with List<BackupCommunicationServer> so keeping it this way allows to
         * parse to IPEndPoint only after primary server crash
         */
        private List<BackupCommunicationServer> _backupCommunicationServers;

        public ComputationalNode(IPEndPoint serverAddress)
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
            // create RegisterMessage
            // TRY to send it and get response with _tcpClient.SendData(marshalledMessage);
            // save information from unmarshalled response


            _computationalTasks = new ComputationalTask[_parallelThreads];
            for (int i = 0; i < _computationalTasks.Length; i++)
            {
                _computationalTasks[i] = new ComputationalTask();
            }

            // start informing about status(es) of threads
            _messagingTask = new Task(() => SendMessage(), _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
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

        private void SendMessage()
        {
            try
            {
                while (true)
                {
                    Message msg;
                    while (_messagesToSend.TryDequeue(out msg))
                    {
                        // send msg (and handle response)
                    }

                    // send status message (use GetStatus()) and handle response

                    // _timeout / 2 is just a proposition
                    Thread.Sleep((int)(_timeout / 2));
                }
            }
            catch (TaskCanceledException)
            {
                // component stops...
            }
        }

        private void HandleResponseMessages(Message[] messages)
        {
            throw new NotImplementedException();
            /* if it is a PartialProblemsMessage
             * start new task running SolvePartialProblem method with available taskIndex
             * (threadIndex is available if _computationalTasks[taskIndex].State == Idle)
             */
            // update information in _computationalTasks[availableTaskIndex] needed for getting Status message (ProblemInstanceId,PartialProblemId,ProblemType)
            //_computationalTasks[availableTaskIndex].Task = new Task(()=>SolvePartialProblem(availableTaskIndex,ppmsg),_cancellationTokenSource.Token);
        }

        private StatusMessage GetStatus()
        {
            throw new NotImplementedException();
            // if _computationalTasks[taskIndex]==null then it is in Idle state
        }

        private void SolvePartialProblem(int taskIndex, PartialProblemsMessage msg)
        {
            //throw new NotImplementedException();
            // get proper TaskSolver by
            // _taskSolvers["nameOfProblem"] = null;


            // solve problem


            // add proper message to send
            // _messagesToSend.Enqueue(...);

            // release computation task (create new in idle state)
            _computationalTasks[taskIndex] = new ComputationalTask();
        }
    }
}
