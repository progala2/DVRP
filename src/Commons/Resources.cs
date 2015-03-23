using System.IO;
using System.Reflection;

namespace _15pl04.Ucc.Commons
{
    public static class Resources
    {
        private const string _resourcesDirectoryPath = "_15pl04.Ucc.Commons.Resources";

        /// <summary>
        /// Gets the name of resource in assembly from resource file name.
        /// </summary>
        /// <param name="resourceFileName">Resource file name.</param>
        /// <returns>The name of resource in assembly.</returns>
        public static string GetManifestResourceName(string resourceFileName)
        {
            return _resourcesDirectoryPath + "." + resourceFileName;
        }

        /// <summary>
        /// Gets content of specified resource.
        /// </summary>
        /// <param name="resourceFileName">Resource file name.</param>
        /// <returns>The content of specified resource file; or null if there is no such resource or resource cannot be read.</returns>
        public static string GetResourceContent(string resourceFileName)
        {
            string result = null;
            var manifestResourceName = Resources.GetManifestResourceName(resourceFileName);
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(manifestResourceName))
            {
                if (stream != null && stream.CanRead)
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        result = streamReader.ReadToEnd();
                    }
                }
            }

            return result;
        }
    }
}
