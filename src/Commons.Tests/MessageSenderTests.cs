using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Dvrp.Ucc.Commons.Components;
using Dvrp.Ucc.Commons.Messaging;
using Dvrp.Ucc.Commons.Messaging.Models;
using Dvrp.Ucc.Commons.Messaging.Models.Base;
using Xunit;

namespace Dvrp.Ucc.Commons.Tests
{
    [Collection("localhost")]
    public class MessageSenderTests
    {
        private const int Port = 9123;
        private const int BufferSize = 2048;
        private static readonly IPAddress TestIp = new (new byte[] {127, 0, 0, 1});
        private Socket _socket = null!;

        [Fact]
        public void MessageSenderSendingMessage()
        {
            var sender = new MessageSender(new IPEndPoint(TestIp, Port));
            Message message = new ErrorMessage
            {
                ErrorText = "TestErrorMessage",
                ErrorType = ErrorType.UnknownSender
            };

            var t = new Task(ListenAndResend);
            t.Start();

            var receivedMessage = sender.Send(new [] {message});

            Assert.NotNull(receivedMessage);
            Assert.Single(receivedMessage);
            Assert.Equal(message.MessageType, receivedMessage[0].MessageType);
            Assert.Equal(message.MessageType, receivedMessage[0].MessageType);
            Assert.Equal(((ErrorMessage) message).ErrorText, ((ErrorMessage) receivedMessage[0]).ErrorText);
            Assert.Equal(((ErrorMessage) message).ErrorType, ((ErrorMessage) receivedMessage[0]).ErrorType);

            EndConnection();
            t.Wait();
        }

        [Fact]
        public void MessageSenderUpdatingBackupServerList()
        {
            var sender = new MessageSender(new IPEndPoint(TestIp, Port));
            var backupServers = new List<ServerInfo>
            {
	            new ("123.123.123.123", 9876)
            };
            Message message = new NoOperationMessage
            {
                BackupServers = backupServers
            };

            var t = new Task(ListenAndResend);
            t.Start();

            var receivedMessage = sender.Send(new [] {message});

            Assert.NotNull(receivedMessage);
            Assert.Single( receivedMessage);
            Assert.Equal(message.MessageType, receivedMessage[0].MessageType);
            Assert.Equal(message.MessageType, receivedMessage[0].MessageType);
            Assert.Equal(((NoOperationMessage) message).BackupServers.Count,
                ((NoOperationMessage) receivedMessage[0]).BackupServers.Count);
            Assert.Equal(((NoOperationMessage) message).BackupServers[0].IpAddress,
                ((NoOperationMessage) receivedMessage[0]).BackupServers[0].IpAddress);
            Assert.Equal(((NoOperationMessage) message).BackupServers[0].Port,
                ((NoOperationMessage) receivedMessage[0]).BackupServers[0].Port);


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
            var handlerSocket = _socket.Accept();
            var bytes = new byte[BufferSize];
            var bytesReceived = handlerSocket.Receive(bytes);

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