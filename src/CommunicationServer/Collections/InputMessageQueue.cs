using System;
using System.Collections.Concurrent;

namespace _15pl04.Ucc.CommunicationServer.Collections
{
    public class InputMessageQueue
    {
        private ConcurrentQueue<Tuple<byte[], TcpClientManager.ResponseCallback>> _queue;

        public void Enqueue(byte[] rawMsg, TcpClientManager.ResponseCallback callback)
        {
            _queue.Enqueue(new Tuple<byte[], TcpClientManager.ResponseCallback>(rawMsg, callback));
        }

        public bool Dequeue(out byte[] rawMsg, out TcpClientManager.ResponseCallback callback)
        {
            Tuple<byte[], TcpClientManager.ResponseCallback> result;

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
