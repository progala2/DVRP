using System.Collections.Concurrent;
using _15pl04.Ucc.CommunicationServer.Messaging.Base;

namespace _15pl04.Ucc.CommunicationServer.Collections
{
    public class RawDataQueueItem
    {
        public byte[] Data { get; set; }
        public Metadata Metadata { get; set; }
        public ProcessedDataCallback Callback { get; set; }
    }

    public class RawDataQueue : ConcurrentQueue<RawDataQueueItem>
    {
        public bool TryDequeue(out byte[] data, out Metadata metadata, out ProcessedDataCallback callback)
        {
            RawDataQueueItem item;

            if (TryDequeue(out item))
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