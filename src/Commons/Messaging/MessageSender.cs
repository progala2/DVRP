using System;
using System.Collections.Generic;
using System.Net;
using Dvrp.Ucc.Commons.Components;
using Dvrp.Ucc.Commons.Messaging.Marshalling;
using Dvrp.Ucc.Commons.Messaging.Models;
using Dvrp.Ucc.Commons.Messaging.Models.Base;
using Dvrp.Ucc.Commons.Utilities;
using TimeoutException = Dvrp.Ucc.Commons.Exceptions.TimeoutException;

namespace Dvrp.Ucc.Commons.Messaging
{
    /// <summary>
    /// The class providing sending messages to server and getting response messages from that server.
    /// </summary>
    public class MessageSender
    {
        private readonly Marshaller _marshaller;
        private readonly TcpClient _tcpClient;
        private List<ServerInfo> _servers;

        /// <summary>
        /// Creates a message sender.
        /// </summary>
        /// <param name="serverAddress">The address of server that messages will be send to.</param>
        public MessageSender(IPEndPoint serverAddress)
        {
            _servers = new List<ServerInfo>();
            _tcpClient = new TcpClient(serverAddress);

            var validator = new MessageValidator();
            var serializer = new MessageSerializer();
            _marshaller = new Marshaller(serializer, validator);
        }

        /// <summary>
        ///     Sends messages and returns messages received. Retruns null if neither primary nor backup servers answered or due to
        ///     other exception.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <returns>Messages returned or null.</returns>
        public List<Message>? Send(Message message)
        {
            return Send(new List<Message> { message });
        }

        /// <summary>
        ///     Sends messages and returns messages received. Retruns null if neither primary nor backup servers answered or due to
        ///     other exception.
        /// </summary>
        /// <param name="messages">Messages to send.</param>
        /// <returns>Messages returned or null.</returns>
        public List<Message>? Send(IList<Message> messages)
        {
            var data = _marshaller.Marshall(messages);
            byte[] retBytes = Array.Empty<byte>();
            bool again;
            do
            {
                try
                {
                    again = false;
                    retBytes = _tcpClient.SendData(data);
                }
                catch (TimeoutException)
                {
                    again = true;
                    if (_servers.Count > 0)
                    {
                        try
                        {
                            _tcpClient.ServerAddress = IpEndPointParser.Parse(_servers[0].IpAddress, _servers[0].Port);
                        }
                        catch (Exception)
                        {
                            // just skip incorrect address
                        }

                        _servers.RemoveAt(0);
                    }
                    else
                    {
                        //No servers available
                        return null;
                    }
                }
                catch (Exception)
                {
                    //can not send to server
                    return null;
                }
            } while (again);


            var messagesReceived = _marshaller.Unmarshall(retBytes);
            UpdateServerList(messagesReceived);
            return messagesReceived;
        }

        /// <summary>
        /// Updates server list.
        /// </summary>
        /// <param name="messages">Messages to update server list based on.</param>
        private void UpdateServerList(IList<Message> messages)
        {
	        for (var i = messages.Count - 1; i >= 0; --i)
            {
                var message = messages[i];
                switch (message.MessageType)
                {
                    case MessageClass.NoOperation:
                        _servers = ((NoOperationMessage)message).BackupServers;
                        return;
                    case MessageClass.RegisterResponse:
                        _servers = ((RegisterResponseMessage)message).BackupServers;
                        return;
                }
            }
        }
    }
}