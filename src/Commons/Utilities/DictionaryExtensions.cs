using System;
using System.Collections.Generic;
using System.Linq;

namespace Dvrp.Ucc.Commons.Utilities
{
    /// <summary>
    /// Extension methods for a generic dictionary.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Removes all elements that match specified condition.
        /// </summary>
        /// <typeparam name="TK">Key type.</typeparam>
        /// <typeparam name="TV">Value type.</typeparam>
        /// <param name="dict">Extended dictionary instance.</param>
        /// <param name="match">Check to perform againt all elements in order to remove them.</param>
        public static void RemoveAll<TK, TV>(this IDictionary<TK, TV> dict, Func<TK, TV, bool> match)
        {
            foreach (var key in dict.Keys.ToArray()
                .Where(key => match(key, dict[key])))
                dict.Remove(key);
        }
    }
}