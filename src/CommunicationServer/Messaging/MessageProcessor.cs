using _15pl04.Ucc.Commons.Messaging.Base;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Messaging.Base;
using System.Threading;
using System.Threading.Tasks;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    internal class MessageProcessor : IDataProcessor
    {
        public bool IsProcessing
        {
            get { return _isProcessing; }
        }

        private RawDataQueue _inputDataQueue;
        private IMarshaller<Message> _marshaller;

        private CancellationTokenSource _cancellationTokenSource;
        private AutoResetEvent _processingLock;

        private volatile bool _isProcessing;

        public MessageProcessor(IMarshaller<Message> marshaller)
        {
            _inputDataQueue = new RawDataQueue();
            _marshaller = marshaller;

            _processingLock = new AutoResetEvent(false);
        }

        public void EnqueueDataToProcess(byte[] data, Metadata metadata, ProcessedDataCallback callback)
        {
            _inputDataQueue.Enqueue(data, metadata, callback);

            _processingLock.Set();
        }

        public void StartProcessing()
        {
            if (_isProcessing)
                return;

            _cancellationTokenSource = new CancellationTokenSource();

            var token = _cancellationTokenSource.Token;
            token.Register(() =>
            {
                _isProcessing = false;
            });

            new Task(() =>
            {
                while (true)
                {
                    if (_inputDataQueue.Count == 0)
                        _processingLock.WaitOne(); // No data available, wait for some.

                    RawDataQueueItem dataToProcess;
                    _inputDataQueue.TryDequeue(out dataToProcess);
                    if (dataToProcess != null)
                        ProcessData(dataToProcess); // Actual processing.

                    if (token.IsCancellationRequested)
                        return;
                }
            }, token).Start();

            _processingLock.Set();
            _isProcessing = true;
        }

        public void StopProcessing()
        {
            if (!_isProcessing)
                return;

            _cancellationTokenSource.Cancel();
            _processingLock.Set();
        }

        private void ProcessData(RawDataQueueItem data)
        {
            Message[] messages = _marshaller.Unmarshall(data.Data);

            foreach(Message msg in messages)
            {
                //TODO
            }
        }
    }
}
