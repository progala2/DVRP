using System;
using System.Collections.Generic;
using System.Net;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons
{
    public class MessageSender
    {
        private readonly TcpClient _tcpClient;
        private List<ServerInfo> _servers;
        private readonly Marshaller _marshaller;

        public MessageSender(IPEndPoint serverAddress)
        {
            _servers = new List<ServerInfo>();
            _tcpClient = new TcpClient(serverAddress);

            var validator = new MessageValidator();
            var serializer = new MessageSerializer();
            _marshaller = new Marshaller(serializer, validator);
        }

        /// <summary>
        /// Sends messages and returns messages received. Retruns null if neither primary nor backup servers answered or due to other exception
        /// </summary>
        /// <param name="message">message to send</param>
        /// <returns>messages returned or null</returns>
        public List<Message> Send(Message message)
        {
            return Send(new List<Message>{ message });
        }

        /// <summary>
        /// Sends messages and returns messages received. Retruns null if neither primary nor backup servers answered or due to other exception
        /// </summary>
        /// <param name="messages">messages to send</param>
        /// <returns>messages returned or null</returns>
        public List<Message> Send(IList<Message> messages)
        {
            var data = _marshaller.Marshall(messages);
            byte[] retBytes = null;
            bool again;
            do
            {
                try
                {
                    again = false;
                    retBytes = _tcpClient.SendData(data);
                }
                catch (Commons.Exceptions.TimeoutException)
                {
                    again = true;
                    if (_servers.Count > 0)
                    {
                        try
                        {
                            _tcpClient.ServerAddress = IPEndPointParser.Parse(_servers[0].IpAddress, _servers[0].Port);
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
            return retBytes != null ? messagesReceived : null;
        }

        private void UpdateServerList(IList<Message> messages)
        {
            if (messages == null)
                return;

            for (int i = messages.Count - 1; i >= 0; --i)
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
