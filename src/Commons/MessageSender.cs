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
            _servers = new List<BackupCommunicationServer>
            {
                new BackupCommunicationServer()
                {
                    Address = endPoint.Address.ToString(),
                    Port = (ushort) endPoint.Port
                }
            };
            _tcpClient = new TcpClient(endPoint);
            _marshaller = new Marshaller();
        }

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
                    _servers.RemoveAt(0);
                    if (_servers.Count > 0)
                    {
                        _tcpClient.ServerAddress = new IPEndPoint(
                            new IPAddress(long.Parse(_servers[0].Address)), _servers[0].Port );
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

        private void UpdateServerList(Message[] messages)
        {
            if (messages == null)
                return;

            bool isRemovingDuplicatesNecessary = false;
            foreach (var message in messages)
            {
                switch (message.MessageType)
                {
                    case Message.MessageClassType.NoOperation:
                        _servers.AddRange(((NoOperationMessage)message).BackupCommunicationServers);
                        isRemovingDuplicatesNecessary = true;
                        break;
                    case Message.MessageClassType.RegisterResponse:
                        _servers.AddRange(((RegisterResponseMessage)message).BackupCommunicationServers);
                        isRemovingDuplicatesNecessary = true;
                        break;
                    default:
                        break;
                }
            }
            if (isRemovingDuplicatesNecessary)
                _servers = _servers.Distinct().ToList();
        }
    }
}
