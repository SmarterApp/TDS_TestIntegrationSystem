using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.TestPackage;

namespace TISUnitTests.testpackages
{
    [TestClass]
    public class ItemGroupItemBlueprintReferenceUnitTest
    {
        [TestMethod]
        public void ShouldBuildACollectionOfContentLevelsFromIdRefString()
        {
            var bpRef = new ItemGroupItemBlueprintReference
            {
                idRef = "1|N-RN|A|a/s|N-RN.2"
            };

            var result = bpRef.GetContentLevels();

            Assert.AreEqual(5, result.Length);
            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("1|N-RN", result[1]);
            Assert.AreEqual("1|N-RN|A", result[2]);
            Assert.AreEqual("1|N-RN|A|a/s", result[3]);
            Assert.AreEqual("1|N-RN|A|a/s|N-RN.2", result[4]);
        }
    }
}
