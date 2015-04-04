
namespace _15pl04.Ucc.CommunicationServer.Messaging.Base
{
    public delegate void ProcessedDataCallback(byte[] response);




    public interface IDataProcessor<T>
        where T : class
    {
        //event ProcessedDataCallback OnProcessing()

        void EnqueueDataToProcess(byte[] data, T metadata, ProcessedDataCallback callback);
    }
}
