using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class MessageSenderTests
    {
        private static readonly IPAddress TestIp = new IPAddress(new byte[] { 127, 0, 0, 1 });
        private const int Port = 9123;
        private Socket _socket;
        private const int BufferSize = 2048;
        

        [TestMethod]
        public void MessageSenderSendingMessage()
        {
            MessageSender sender = new MessageSender(new IPEndPoint(TestIp, Port));
            Message message = new ErrorMessage()
            {
                ErrorText = "TestErrorMessage",
                ErrorType = ErrorMessageErrorType.UnknownSender
            };

            Task t = new Task(new Action(ListenAndResend));
            t.Start();

            Message[] receivedMessage = sender.Send(new Message[]{message});

            Assert.AreEqual(1, receivedMessage.Length);
            Assert.AreEqual(message.MessageType, receivedMessage[0].MessageType);
            Assert.AreEqual(message.MessageType, receivedMessage[0].MessageType);
            Assert.AreEqual(((ErrorMessage)message).ErrorText, ((ErrorMessage)receivedMessage[0]).ErrorText);
            Assert.AreEqual(((ErrorMessage)message).ErrorType, ((ErrorMessage)receivedMessage[0]).ErrorType);

            EndConnection();
            t.Wait();
        }

        [TestMethod]
        public void MessageSenderUpdatingBackupServerList()
        {
            MessageSender sender = new MessageSender(new IPEndPoint(TestIp, Port));
            var backupServers = new List<BackupCommunicationServer>
            {
                new BackupCommunicationServer()
                {
                    IpAddress = "123.123.123.123",
                    Port = 9876
                }
            };
            Message message = new NoOperationMessage()
            {
                BackupCommunicationServers = backupServers
            };

            Task t = new Task(new Action(ListenAndResend));
            t.Start();

            Message[] receivedMessage = sender.Send(new Message[] { message });

            Assert.AreEqual(1, receivedMessage.Length);
            Assert.AreEqual(message.MessageType, receivedMessage[0].MessageType);
            Assert.AreEqual(message.MessageType, receivedMessage[0].MessageType);
            Assert.AreEqual(((NoOperationMessage)message).BackupCommunicationServers.Count, 
                ((NoOperationMessage)receivedMessage[0]).BackupCommunicationServers.Count);
            Assert.AreEqual(((NoOperationMessage)message).BackupCommunicationServers[0].IpAddress,
                ((NoOperationMessage)receivedMessage[0]).BackupCommunicationServers[0].IpAddress);
            Assert.AreEqual(((NoOperationMessage)message).BackupCommunicationServers[0].Port,
                ((NoOperationMessage)receivedMessage[0]).BackupCommunicationServers[0].Port);
            
            
            EndConnection();
            t.Wait();
        }
        

        private void ListenAndResend()
        {
            StartListening();
            AcceptConnection();
            //EndConnection();
        }

        private void AcceptConnection()
        {
            Socket handlerSocket = _socket.Accept();
            byte[] bytes = new byte[BufferSize];
            int bytesReceived = handlerSocket.Receive(bytes);

            bytes = bytes.Take(bytesReceived).ToArray();

            handlerSocket.Send(bytes);
            handlerSocket.Shutdown(SocketShutdown.Send);
            handlerSocket.Close();
        }

        private void EndConnection()
        {
            //Thread.Sleep(1500);
            _socket.Close();
        }

        private void StartListening()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _socket.Bind(new IPEndPoint(TestIp, Port));
            _socket.Listen(10);
        }
    }
}
