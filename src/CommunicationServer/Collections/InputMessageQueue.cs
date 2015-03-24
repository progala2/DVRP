using System;
using System.Collections.Concurrent;

namespace _15pl04.Ucc.CommunicationServer.Collections
{
    /// <summary>
    /// Collection that stores all incoming, non-processed, raw messages from the system nodes.
    /// </summary>
    internal class InputMessageQueue
    {
        /// <summary>
        /// Get number of messages currently stored in the queue.
        /// </summary>
        public int Count
        {
            get { return _queue.Count; }
        }

        private ConcurrentQueue<Tuple<byte[], AsyncTcpServer.ResponseCallback>> _queue;

        public InputMessageQueue()
        {
            _queue = new ConcurrentQueue<Tuple<byte[], AsyncTcpServer.ResponseCallback>>();
        }

        /// <summary>
        /// Enqueues a raw message and associated callback into the queue.
        /// </summary>
        /// <param name="rawMsg">Raw message.</param>
        /// <param name="callback">Async callback invoked after processing the data.</param>
        public void Enqueue(byte[] rawMsg, AsyncTcpServer.ResponseCallback callback)
        {
            _queue.Enqueue(new Tuple<byte[], AsyncTcpServer.ResponseCallback>(rawMsg, callback));
        }

        /// <summary>
        /// Attempts to remove a raw message and associated callback from the front of the queue.
        /// </summary>
        /// <param name="rawMsg">Raw message.</param>
        /// <param name="callback">Associated response callback.</param>
        /// <returns>True if success; false if the queue is empty.</returns>
        public bool TryDequeue(out byte[] rawMsg, out AsyncTcpServer.ResponseCallback callback)
        {
            Tuple<byte[], AsyncTcpServer.ResponseCallback> result;

            if (_queue.TryDequeue(out result))
            {
                rawMsg = result.Item1;
                callback = result.Item2;
                return true;
            }
            else
            {
                rawMsg = null;
                callback = null;
                return false;
            }
        }
    }
}
