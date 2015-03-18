
namespace _15pl04.Ucc.Commons.Messaging
{
    public interface IMarshaller<T>
        where T : class
    {
        byte[] Marshall(T[] data);
    }
}
