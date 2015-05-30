using System;

namespace _15pl04.Ucc.CommunicationServer.Messaging.Base
{
    /// <summary>
    /// Invoked after processing client's data and generating a response.
    /// </summary>
    /// <param name="response">Response data returned to the client.</param>
    public delegate void ProcessedDataCallback(byte[] response);

    public interface IDataProcessor
    {

        bool IsProcessing { get; }
        void EnqueueDataToProcess(byte[] data, Metadata metadata, ProcessedDataCallback callback);
        void StartProcessing();
        void StopProcessing();
    }

    public class Metadata
    {
        /// <summary>
        ///     Reception time in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime ReceptionTime { get; set; }
    }
}