using System.Linq;
using System.Reflection;

namespace _15pl04.Ucc.Commons
{
    public static class Resources
    {
        private static readonly string _resourcesFolderPath = "_15pl04.Ucc.Commons.Resources";

        /// <summary>
        /// Gets the name of resource in assembly from resource file name.
        /// </summary>
        /// <param name="resourceFileName">Resource file name.</param>
        /// <returns>The name of resource in assembly.</returns>
        public static string GetManifestResourceName(string resourceFileName)
        {
            return _resourcesFolderPath + "." + resourceFileName;
        }
    }
}
