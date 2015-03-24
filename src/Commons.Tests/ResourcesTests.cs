using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class ResourcesTests
    {
        private string _drf;
        private Assembly _assembly;

        [TestInitialize]
        public void TestInit()
        {
            var resourcesFolder = "Resources";
            _drf = "." + resourcesFolder + ".";
            _assembly = Assembly.GetAssembly(typeof(Resources));
        }

        [TestMethod]
        public void GetManifestResourceNameReturnesExpectedString()
        {
            var expectedManifestResourceName = _assembly.GetManifestResourceNames().First(s => s.Contains(_drf));
            var position = expectedManifestResourceName.LastIndexOf(_drf) + _drf.Length;
            var resourceName = expectedManifestResourceName.Substring(position);

            var resultManifestResourceName = Resources.GetManifestResourceName(resourceName);

            Assert.AreEqual(expectedManifestResourceName, resultManifestResourceName);
        }

        [TestMethod]
        public void GetResourceContentReturnsContentOfExistingResource()
        {
            var manifestResourceName = _assembly.GetManifestResourceNames().First(s => s.Contains(_drf));
            var position = manifestResourceName.LastIndexOf(_drf) + _drf.Length;
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
