namespace Dvrp.Ucc.Commons.Messaging.Marshalling.Base
{
    /// <summary>
    /// Interface for converting data between type T and bytes.
    /// </summary>
    /// <typeparam name="T">Type of objects to be marshalled.</typeparam>
    public interface IMarshaller<T>
    {
        /// <summary>
        /// Convert bytes to List of T type.
        /// </summary>
        /// <param name="rawData">Raw bytes.</param>
        /// <returns>List of type T items after conversion.</returns>
        T[] Deserialize(byte[] rawData);
        /// <summary>
        /// Convert items T to bytes.
        /// </summary>
        /// <param name="data">Items for processing.</param>
        /// <returns>Bytes after conversion.</returns>
        byte[] Serialize(params T[] data);
    }
}