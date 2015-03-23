using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class ResourcesTests
    {
        private string drf;
        private Assembly assembly;

        [TestInitialize]
        public void TestInit()
        {
            var resourcesFolder = "Resources";
            drf = "." + resourcesFolder + ".";
            assembly = Assembly.GetAssembly(typeof(Resources));
        }

        [TestMethod]
        public void GetManifestResourceNameReturnesExpectedString()
        {
            var expectedManifestResourceName = assembly.GetManifestResourceNames().First(s => s.Contains(drf));
            var position = expectedManifestResourceName.LastIndexOf(drf) + drf.Length;
            var resourceName = expectedManifestResourceName.Substring(position);

            var resultManifestResourceName = Resources.GetManifestResourceName(resourceName);

            Assert.AreEqual(expectedManifestResourceName, resultManifestResourceName);
        }

        [TestMethod]
        public void GetResourceContentReturnsContentOfExistingResource()
        {
            var manifestResourceName = assembly.GetManifestResourceNames().First(s => s.Contains(drf));
            var position = manifestResourceName.LastIndexOf(drf) + drf.Length;
            var resourceName = manifestResourceName.Substring(position);

            var resourceContent = Resources.GetResourceContent(resourceName);

            Assert.IsNotNull(resourceContent);
        }

        [TestMethod]
        public void GetResourceContentReturnsNullForIncorrectResourceName()
        {
            var resourceName = "SomeIncorrectResourceName";

            var resourceContent = Resources.GetResourceContent(resourceName);

            Assert.IsNull(resourceContent);
        }
    }
}
