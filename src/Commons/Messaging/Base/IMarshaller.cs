
namespace _15pl04.Ucc.Commons.Messaging.Base
{
    public interface IMarshaller<T>
    {
        T[] Unmarshall(byte[] rawData);
        byte[] Marshall(T[] data);
    }
}
