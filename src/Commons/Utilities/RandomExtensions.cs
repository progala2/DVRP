using System;

namespace _15pl04.Ucc.Commons.Utilities
{
    /// <summary>
    /// Extension methods for the Random class.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Generate pseudo-random ulong number.
        /// </summary>
        /// <param name="random">Extended random instance.</param>
        /// <returns>Generated ulong value.</returns>
        public static ulong NextUInt64(this Random random)
        {
            var buffer = new byte[sizeof (ulong)];
            random.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}