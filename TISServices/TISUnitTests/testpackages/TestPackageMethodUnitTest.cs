using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDSQASystemAPI.TestPackage;

namespace TISUnitTests.testpackages
{
    [TestClass]
    public class TestPackageMethodUnitTest
    {
        [TestMethod]
        public void ShouldGetAFlattenedListOfBlueprintElements()
        {
            var testPackage = new TestPackage
            {
                Blueprint = new BlueprintElement[]
                {
                    new BlueprintElement
                    {
                        id = "level-0",
                        type = "strand",
                        BlueprintElement1 = new BlueprintElement[]
                        {
                            new BlueprintElement
                            {
                                id = "level-1",
                                type = "contentlevel",
                                BlueprintElement1 = new BlueprintElement[]
                                {
                                    new BlueprintElement
                                    {
                                        id = "level-2",
                                        type = "contentlevel"
                                    },
                                    new BlueprintElement
                                    {
                                        id ="level-2.5",
                                        type = "contentlevel"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var flattenedBlueprintDictionary = testPackage.GetAllTestPackageBlueprintElements();

            Assert.AreEqual(4, flattenedBlueprintDictionary.Count);
            Assert.IsTrue(flattenedBlueprintDictionary.ContainsKey("level-0"));
            Assert.IsTrue(flattenedBlueprintDictionary.ContainsKey("level-2.5"));
        }
    }
}
