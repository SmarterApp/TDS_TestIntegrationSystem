using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDSQASystemAPI.Extensions;

namespace TISUnitTests.extensions
{
    /// <summary>
    /// Unit tests for the extension methods.
    /// </summary>
    [TestClass]
    public class ExtensionMethodUnitTests
    {
        [TestMethod]
        public void ShouldConvertAListOfIntsToADataTableWithADefaultName()
        {
            var intList = new List<int> { 1, 2, 3, 4, 5 };
            var result = intList.ToDataTable();

            Assert.AreEqual(5, result.Rows.Count);
            Assert.AreEqual("dataTable", result.TableName);
        }

        [TestMethod]
        public void ShouldConvertAListOfIntsToADataTableWithACustomName()
        {
            var intList = new List<int> { 1, 2, 3, 4, 5 };
            var result = intList.ToDataTable("intTable");

            Assert.AreEqual(5, result.Rows.Count);
            Assert.AreEqual("intTable", result.TableName);
        }

        [TestMethod]
        public void ShouldConvertAListOfObjectsToADataTable()
        {
            var objectList = new List<DataTableObjectTest>
            {
                new DataTableObjectTest { Id = 1, FirstName = "Han", LastName = "Solo" },
                new DataTableObjectTest { Id = 2, FirstName = "Luke", LastName = "Skywalker" },
                new DataTableObjectTest { Id = 3, FirstName = "Chewbacca" },
                new DataTableObjectTest { Id = 4, FirstName = "Leia", LastName = "Organa" }
            };

            var result = objectList.ToDataTable();

            Assert.AreEqual(4, result.Rows.Count);
        }

        [TestMethod]
        public void ShouldGetValueFromDictionaryForKey()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "firstKey", "foo" },
                { "secondKey", "bar" }
            };

            var result = dictionary.GetOrDefault("firstKey", "default");

            Assert.AreEqual("foo", result);
        }

        [TestMethod]
        public void ShouldGetDefaultValueWhenKeyDoesNotExist()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "firstKey", "foo" },
                { "secondKey", "bar" }
            };

            var firstValue = dictionary.GetOrDefault("i-do-not-exist", "default");

            Assert.AreEqual("default", firstValue);
        }
    }

    /// <summary>
    /// A class for unit testing the <code>ToDataTable</code> method.
    /// </summary>
    class DataTableObjectTest
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
