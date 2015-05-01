
namespace _15pl04.Ucc.Commons.Messaging.Marshalling.Base
{
    public interface ISerializer<T>
    {
        T Deserialize(byte[] buffer);
        T Deserialize(byte[] buffer, int index, int count);
        byte[] Serialize(T obj);
    }
}
