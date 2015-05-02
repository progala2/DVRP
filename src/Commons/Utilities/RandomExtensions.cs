using System;

namespace _15pl04.Ucc.Commons.Utilities
{
    public static class RandomExtensions
    {
        public static ulong NextUInt64(this Random random)
        {
            var buffer = new byte[sizeof (ulong)];
            random.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}