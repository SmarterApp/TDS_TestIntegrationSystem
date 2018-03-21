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

        [TestMethod]
        public void ShouldIdentifyTestPackageAsCombined()
        {
            var testPackage = new TestPackage
            {
                publisher = "SBAC_PT",
                academicYear = "2017-2018",
                Blueprint = new BlueprintElement[]
                {
                    new BlueprintElement
                    {
                        id = "ICA-UNIT-TEST-GRADE-11-COMBINED",
                        type = "combined"
                    }
                }
            };

            Assert.IsTrue(testPackage.IsCombined());
        }

        [TestMethod]
        public void shouldNotIdentifyTestPackageAsCombined()
        {
            var testPackage = new TestPackage
            {
                publisher = "SBAC_PT",
                academicYear = "2017-2018",
                Blueprint = new BlueprintElement[]
                {
                    new BlueprintElement
                    {
                        id = "ICA-UNIT-TEST-GRADE-11-FIXED",
                        type = "foo"
                    }
                }
            };

            Assert.IsFalse(testPackage.IsCombined());
        }

        [TestMethod]
        public void ShouldGetCombinationTestPackageKeyForCombinedTestPackage()
        {
            var testPackage = new TestPackage
            {
                publisher = "SBAC_PT",
                academicYear = "2017-2018",
                Blueprint = new BlueprintElement[]
                {
                    new BlueprintElement
                    {
                        id = "ICA-UNIT-TEST-GRADE-11-COMBINED",
                        type = "combined"
                    }
                }
            };

            Assert.AreEqual("(SBAC_PT)ICA-UNIT-TEST-GRADE-11-COMBINED-2017-2018", testPackage.GetCombinationTestPackageKey());
        }
    }
}
