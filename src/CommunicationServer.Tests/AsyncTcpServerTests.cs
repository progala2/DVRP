using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.Messaging;

namespace _15pl04.Ucc.CommunicationServer.Tests
{
    [TestClass]
    public class AsyncTcpServerTests
    {
        private static readonly IPAddress TestIp = new IPAddress(new byte[] { 127, 0, 0, 1 });
        private const int Port = 9123;
        private const int BufferSize = 204800;
        private AsyncTcpServer _tcpServer;
        private readonly Marshaller _marshaller = new Marshaller();
        private Socket _socket;
        private ServerConfig _config;

        [TestMethod]
        public void AsyncTcpServerForwardsStatusMessageAndReceivesErrorMessage()
        {
            //Creating TcpServer
            Init();
            
            InitSocket();

            Message message = new StatusMessage()
            {
                Id = 1,
                noNamespaceSchemaLocation = "?",
                Threads = new List<StatusThread>()
                    {
                        new StatusThread()
                        {
                            ProblemType = "ss",
                            State = Commons.ThreadState.Busy
                        }
                    }
            };
            Message expectedMessage = new ErrorMessage()
            {
                ErrorMessageText = "Unregistered component error.",
                ErrorMessageType = ErrorMessageErrorType.UnknownSender,
            };

            var messagesReturned = Send(new Message[] {message});

            Assert.AreEqual(1, messagesReturned.Length);
            Assert.AreEqual(expectedMessage.MessageType, messagesReturned[0].MessageType);
            Assert.AreEqual((expectedMessage as ErrorMessage).ErrorMessageText,
                (messagesReturned[0] as ErrorMessage).ErrorMessageText);
            Assert.AreEqual((expectedMessage as ErrorMessage).ErrorMessageType,
                (messagesReturned[0] as ErrorMessage).ErrorMessageType);
            
            _tcpServer.StopListening();
        }

        [TestMethod]
        public void AsyncTcpServerForwardsRegistersMessageAndStatusMessageAndResponds()
        {
            //Preparing TcpServer
            Init();

            InitSocket();

            Message messageRegister = new RegisterMessage()
            {
                Deregister = null,
                Id = null,
                noNamespaceSchemaLocation = "?",
                ParallelThreads = 1,
                SolvableProblems = new List<string>()
                {
                    "s"
                },
                Type = ComponentType.ComputationalNode
            };
            Message expectedMessage1 = new RegisterResponseMessage()
            {
                Id = 1,
                BackupCommunicationServers = new List<BackupCommunicationServer>(),
                Timeout = _config.CommunicationTimeout
            };
            var messagesReturned = Send(new Message[] { messageRegister });

            Assert.AreEqual(1, messagesReturned.Length);
            Assert.AreEqual(expectedMessage1.MessageType, messagesReturned[0].MessageType);
            Assert.AreEqual((expectedMessage1 as RegisterResponseMessage).Timeout,
                (messagesReturned[0] as RegisterResponseMessage).Timeout);


            Message messageExpected2 = new NoOperationMessage();

            Message messageStatus = new StatusMessage()
            {
                Id = (messagesReturned[0] as RegisterResponseMessage).Id,
                noNamespaceSchemaLocation = "?",
                Threads = new List<StatusThread>()
                    {
                        new StatusThread()
                        {
                            ProblemType = "s",
                            State = Commons.ThreadState.Idle
                        }
                    }
            };

            InitSocket();
            messagesReturned = Send(new Message[] { messageStatus });

            Assert.AreEqual(1, messagesReturned.Length);
            Assert.AreEqual(messageExpected2.MessageType, messagesReturned[0].MessageType);


            _tcpServer.StopListening();
        }
        

        private void Init()
        {
            _config = new ServerConfig(null)
            {
                Address = new IPEndPoint(TestIp, Port),
                Mode = ServerConfig.ServerMode.Primary,
                CommunicationTimeout = 10
            } ;

            _tcpServer = new AsyncTcpServer(_config, new MessageProcessor(new Marshaller(), _config.CommunicationTimeout));
            
            new Thread(_tcpServer.StartListening).Start();
        }

        private void InitSocket()
        {
            IPEndPoint remoteEp = new IPEndPoint(TestIp, Port);
            _socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _socket.Connect(remoteEp);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void InitSocket(out Socket _socket)
        {
            IPEndPoint remoteEp = new IPEndPoint(TestIp, Port);
            _socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _socket.Connect(remoteEp);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private Message[] Send(Message[] messages)
        {
            byte[] bytes = new byte[BufferSize];

            byte[] bytesToSend= _marshaller.Marshall(messages);

            _socket.Send(bytesToSend);

            _socket.Shutdown(SocketShutdown.Send);

            int bytesRec = _socket.Receive(bytes);
            _socket.Close();
            return _marshaller.Unmarshall(bytes.Take(bytesRec).ToArray());
        }

        private Message[] Send(Message[] messages, Socket _socket)
        {
            byte[] bytes = new byte[BufferSize];

            byte[] bytesToSend = _marshaller.Marshall(messages);

            _socket.Send(bytesToSend);

            _socket.Shutdown(SocketShutdown.Send);

            int bytesRec = _socket.Receive(bytes);
            _socket.Close();
            return _marshaller.Unmarshall(bytes.Take(bytesRec).ToArray());
        }
    }
}

