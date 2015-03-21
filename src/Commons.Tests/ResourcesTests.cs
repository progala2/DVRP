using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class ResourcesTests
    {
        [TestMethod]
        public void GetManifestResourceNameReturnesExpectedString()
        {
            var resourcesFolder = "Resources";
            var drf = "." + resourcesFolder + ".";
            var assembly = Assembly.GetAssembly(typeof(Resources));
            var expectedManifestResourceName = assembly.GetManifestResourceNames().First(s => s.Contains(drf));
            var position = expectedManifestResourceName.LastIndexOf(drf) + drf.Length;
            var resourceName = expectedManifestResourceName.Substring(position);

            var resultManifestResourceName = Resources.GetManifestResourceName(resourceName);

            Assert.AreEqual(expectedManifestResourceName, resultManifestResourceName);
        }
    }
}
