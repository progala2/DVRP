namespace Dvrp.Ucc.Commons.Messaging.Marshalling.Base
{
    /// <summary>
    /// Interface for (de)serialiazing.
    /// </summary>
    /// <typeparam name="T">Type to be (de)serailized.</typeparam>
    public interface ISerializer<T>
    {
        /// <summary>
        /// Convert bytes to type T.
        /// </summary>
        /// <param name="buffer">Bytes to be converted.</param>
        /// <returns>Object T after conversion.</returns>
        T Deserialize(byte[] buffer);
        /// <summary>
        /// Convert count bytes to type T.
        /// </summary>
        /// <param name="buffer">Bytes to be converted.</param>
        /// <param name="index">Where to begin reading bytes for deserializing</param>
        /// <param name="count">Number of bytes to deserialize.</param>
        /// <returns>Object of type T after conversion.</returns>
        T Deserialize(byte[] buffer, int index, int count);
        /// <summary>
        /// Convert object T to bytes.
        /// </summary>
        /// <param name="obj">Object to be serialized.</param>
        /// <returns>Bytes after serialization.</returns>
        byte[] Serialize(T obj);
    }
}