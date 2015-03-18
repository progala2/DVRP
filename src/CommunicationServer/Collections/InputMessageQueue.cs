using System;
using System.Collections.Concurrent;

namespace _15pl04.Ucc.CommunicationServer.Collections
{
    public class InputMessageQueue
    {
        private ConcurrentQueue<Tuple<byte[], TcpListener.ResponseCallback>> _queue;

        public void Enqueue(byte[] rawMsg, TcpListener.ResponseCallback callback)
        {
            _queue.Enqueue(new Tuple<byte[], TcpListener.ResponseCallback>(rawMsg, callback));
        }

        public bool Dequeue(out byte[] rawMsg, out TcpListener.ResponseCallback callback)
        {
            Tuple<byte[], TcpListener.ResponseCallback> result;

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
