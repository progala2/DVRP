using System.IO;
using System.Reflection;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    public abstract class Message
    {
        /// <summary>
        /// Gets content of corresponding .xsd file.
        /// </summary>
        /// <returns>Content of corresponding .xsd file.</returns>
        /// <remarks>
        /// This method assumes that each non-abstract derived type:
        ///  - has name like "NameMessage",
        ///  - has corresponding "Name.xsd" file in Resources folder.
        ///  Because of this restriction it is an internal method.
        ///  </remarks>
        internal string GetXsdFileContent()
        {
            var className = this.GetType().Name;
            var resourceFileName = className.Remove(className.Length - "Message".Length) + ".xsd";
            var resourceContent = Resources.GetResourceContent(resourceFileName);
            return resourceContent;
        }
    }
}
