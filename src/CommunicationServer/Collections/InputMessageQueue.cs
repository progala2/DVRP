using System;
using System.Collections.Concurrent;

namespace _15pl04.Ucc.CommunicationServer.Collections
{
    internal class InputMessageQueue
    {
        private ConcurrentQueue<Tuple<byte[], AsyncTcpServer.ResponseCallback>> _queue;

        public InputMessageQueue()
        {
            _queue = new ConcurrentQueue<Tuple<byte[], AsyncTcpServer.ResponseCallback>>();
        }

        public void Enqueue(byte[] rawMsg, AsyncTcpServer.ResponseCallback callback)
        {
            _queue.Enqueue(new Tuple<byte[], AsyncTcpServer.ResponseCallback>(rawMsg, callback));

        }

        public bool TryDequeue(out byte[] rawMsg, out AsyncTcpServer.ResponseCallback callback)
        {
            Tuple<byte[], AsyncTcpServer.ResponseCallback> result;

            if (!_queue.TryDequeue(out result))
            {
                rawMsg = null;
                callback = null;
                return false;
            }
            else
            {
                rawMsg = result.Item1;
                callback = result.Item2;
                return true;
            }
        }
    }
}
