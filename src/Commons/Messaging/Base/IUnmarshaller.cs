
namespace _15pl04.Ucc.Commons.Messaging
{
    public interface IUnmarshaller<T>
        where T : class
    {
        T[] Unmarshall(byte[] data);
    }
}
