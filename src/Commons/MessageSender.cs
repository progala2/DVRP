using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons
{
    class MessageSender
    {
        private readonly TcpClient _tcpClient;
        private List<BackupCommunicationServer> _servers;
        private readonly Marshaller _marshaller;

        public MessageSender(IPEndPoint endPoint)
        {
            _servers = new List<BackupCommunicationServer>();
            _tcpClient = new TcpClient(endPoint);
            _marshaller = new Marshaller();
        }

        /// <summary>
        /// Functions sends messages and returns messages received. Retruns null if neither primary nor backup servers answered or due to other exception
        /// </summary>
        /// <param name="messages">messages to send</param>
        /// <returns>messages returned or null</returns>
        public Message[] SendMessages(Message[] messages)
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
                catch (Commons.TimeoutException)
                {
                    again = true;
                    if (_servers.Count > 0)
                    {
                        _tcpClient.ServerAddress = new IPEndPoint(
                            new IPAddress(long.Parse(_servers[0].Address)), _servers[0].Port);
                        _servers.RemoveAt(0);
                    }
                    else
                    {
                        //No servers available
                        return null;
                    }
                }
                catch (Exception e)
                {
                    //can not send to server
                    return null;
                }
            } while (again);


            var messagesReceived = _marshaller.Unmarshall(retBytes);
            UpdateServerList(messagesReceived);
            return retBytes != null ? messagesReceived : null;
        }

        private void UpdateServerList(Message[] messages)
        {
            if (messages == null)
                return;
            
            for (int i = messages.Length - 1; i >= 0; --i)
            {
                var message = messages[i];
                switch (message.MessageType)
                {
                    case Message.MessageClassType.NoOperation:
                        _servers = ((NoOperationMessage)message).BackupCommunicationServers;
                        return;
                    case Message.MessageClassType.RegisterResponse:
                        _servers = ((RegisterResponseMessage)message).BackupCommunicationServers;
                        return;
                }
            }
        }
    }
}
