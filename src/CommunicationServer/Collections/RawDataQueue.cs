using System.Collections.Concurrent;
using Dvrp.Ucc.CommunicationServer.Messaging.Base;

namespace Dvrp.Ucc.CommunicationServer.Collections
{
    /// <summary>
    /// Data structure representing data received from a cluster client.
    /// </summary>
    public class RawDataQueueItem
    {
        /// <summary>
        /// Byte data received from the client.
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// Information about the data and connection.
        /// </summary>
        public Metadata Metadata { get; set; }
        /// <summary>
        /// Callback that finalizes the connection.
        /// </summary>
        public ProcessedDataCallback Callback { get; set; }
    }

    /// <summary>
    /// Concurrent queue that stores (meta)data from cluster clients.
    /// </summary>
    public class RawDataQueue : ConcurrentQueue<RawDataQueueItem>
    {
        /// <summary>
        /// Dequeue item from the queue if possible.
        /// </summary>
        /// <param name="data">Byte data sent by the client.</param>
        /// <param name="metadata">Information about the data and connection.</param>
        /// <param name="callback">Callback that finalizes the connection.</param>
        /// <returns>True if managed to dequeue.</returns>
        public bool TryDequeue(out byte[]? data, out Metadata? metadata, out ProcessedDataCallback? callback)
        {
	        if (TryDequeue(out var item))
            {
                data = item.Data;
                metadata = item.Metadata;
                callback = item.Callback;

                return true;
            }
            data = null;
            metadata = null;
            callback = null;

            return false;
        }
        /// <summary>
        /// Enqueue data into the queue.
        /// </summary>
        /// <param name="data">Byte data sent by the client.</param>
        /// <param name="metadata">Information about the data and connection.</param>
        /// <param name="callback">Callback that finalizes the connection.</param>
        public void Enqueue(byte[] data, Metadata metadata, ProcessedDataCallback callback)
        {
            var item = new RawDataQueueItem
            {
                Data = data,
                Metadata = metadata,
                Callback = callback
            };

            Enqueue(item);
        }
    }
}