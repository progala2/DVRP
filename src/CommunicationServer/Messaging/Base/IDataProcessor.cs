using System;

namespace _15pl04.Ucc.CommunicationServer.Messaging.Base
{
    /// <summary>
    /// Invoked after processing client's data and generating a response.
    /// </summary>
    /// <param name="response">Response data returned to the client.</param>
    public delegate void ProcessedDataCallback(byte[] response);

    /// <summary>
    /// Module responsible for processing and generating output for incoming byte data.
    /// </summary>
    public interface IDataProcessor
    {
        /// <summary>
        /// True if the data processor is processing data. False otherwise.
        /// </summary>
        bool IsProcessing { get; }
        /// <summary>
        /// Enqueues new data to process.
        /// </summary>
        /// <param name="data">Byte data.</param>
        /// <param name="metadata">Information about received data.</param>
        /// <param name="callback">Callback invoked after processing containing the output data.</param>
        void EnqueueDataToProcess(byte[] data, Metadata metadata, ProcessedDataCallback callback);
        /// <summary>
        /// Starts processing data.
        /// </summary>
        void StartProcessing();
        /// <summary>
        /// Stops processing data.
        /// </summary>
        void StopProcessing();
    }

    /// <summary>
    /// Information about received data and/or its source.
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Data reception time.
        /// </summary>
        public DateTime ReceptionTime { get; set; }
    }
}