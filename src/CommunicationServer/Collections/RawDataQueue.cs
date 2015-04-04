using _15pl04.Ucc.CommunicationServer.Messaging.Base;
using System.Collections.Concurrent;

namespace _15pl04.Ucc.CommunicationServer.Collections
{
    public class RawDataQueueItem<T>
        where T : class
    {
        public byte[] Data { get; set; }
        public T Metadata { get; set; }
        public ProcessedDataCallback Callback { get; set; }
    }

    public class RawDataQueue<T> : ConcurrentQueue<RawDataQueueItem<T>>
        where T : class
    {
        public bool TryDequeue(out byte[] data, out T metadata, out ProcessedDataCallback callback)
        {
            RawDataQueueItem<T> item;

            if (TryDequeue(out item))
            {
                data = item.Data;
                metadata = item.Metadata;
                callback = item.Callback;

                return true;
            }
            else
            {
                data = null;
                metadata = null;
                callback = null;

                return false;
            }
        }

        public void Enqueue(byte[] data, T metadata, ProcessedDataCallback callback)
        {
            var item = new RawDataQueueItem<T>
            {
                Data = data,
                Metadata = metadata,
                Callback = callback,
            };

            Enqueue(item);
        }
    }
}
