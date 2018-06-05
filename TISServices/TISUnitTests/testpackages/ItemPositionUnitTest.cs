using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TDSQASystemAPI.TestPackage;

namespace TISUnitTests.testpackages
{
    [TestClass]
    public class ItemPositionUnitTest
    {
        [TestMethod]
        public void ShouldHaveFirstPositionWhenThereIsASingleItem()
        {
            var itemGroup = BuildItemGroup(1);

            Assert.AreEqual(1, itemGroup.Item[0].Position);
        }

        [TestMethod]
        public void ShouldHaveSecondPositionWhenThereAreTwoItems()
        {
            var itemGroup = BuildItemGroup(2);

            Assert.AreEqual(2, itemGroup.Item[1].Position);
        }

        [TestMethod]
        public void ShouldHavePositionTwoForSecondItemInSecondItemGroupWhenItemGroupsAreInAdaptivePool()
        {
            var firstItemGroup = BuildItemGroup(5);
            var secondItemGroup = BuildItemGroup(2);

            var segment = new TestSegment
            {
                id = "SBAC-IRP-Perf-MATH-11",
                position = 1,
                algorithmType = "adaptive2",
                algorithmImplementation = "FAIRWAY ROUNDROBIN",
                Item = new TestSegmentPool
                {
                    ItemGroup = new ItemGroup[] { firstItemGroup, secondItemGroup }
                }
            };

            Assert.AreEqual(2, secondItemGroup.Item[1].Position);
        }

        [TestMethod]
        public void ShouldHaveFourthPositionInASegmentFormWithTwoGroups()
        {
            var firstItemGroup = BuildItemGroup(2);
            var secondItemGroup = BuildItemGroup(2);

            var allItemGroups = new ItemGroup[] { firstItemGroup, secondItemGroup };

            var form = new TestSegmentSegmentFormsSegmentForm
            {
                cohort = "Cohort",
                id = "form_id",
                ItemGroup = allItemGroups
            };

            var allItems = from ig in form.ItemGroup
                           from item in ig.Item
                           select item;

            foreach (var item in allItems)
            {
                item.SegmentForm = form;
            }

            Assert.AreEqual(4, secondItemGroup.Item[1].Position);
        }

        /// <summary>
        /// Build an array of <code>ItemGroupItem</code>s for testing.
        /// </summary>
        /// <param name="quantity">The number of <code>ItemGroupItem</code>s to build.</param>
        /// <returns>An array of <code>ItemGroupItem</code>s</returns>
        private ItemGroupItem[] BuildItemGroupItems(int quantity)
        {
            var itemGroupItems = new ItemGroupItem[quantity];
            for (var i = 0; i < quantity; i++)
            {
                var item = new ItemGroupItem
                {
                    Presentations = new PresentationsPresentation[0],
                    BlueprintReferences = new ItemGroupItemBlueprintReference[0],
                    id = string.Format("itemId_{0}", i),
                    type = "type",
                    ItemScoreDimensions = new ItemGroupItemItemScoreDimension[] {
                        new ItemGroupItemItemScoreDimension
                    {
                        scorePoints = 1,
                        weight = 1,
                        measurementModel = "IRT3PLn",
                        ItemScoreParameter = new ItemGroupItemItemScoreDimensionItemScoreParameter[0]
                    }
                    }
                };

                itemGroupItems[i] = item;
            }

            return itemGroupItems;
        }

        /// <summary>
        /// Build an <code>ItemGroup</code> for testing.
        /// </summary>
        /// <param name="numberOfItems">The number of <code>ItemGroupItem</code>s</param> to include.
        /// <returns>An <code>ItemGroup</code> containing the specified number of <code>ItemGroupItem</code>s.</returns>
        private ItemGroup BuildItemGroup(int numberOfItems)
        {
            var allItems = BuildItemGroupItems(numberOfItems);
            var itemGroup = new ItemGroup
            {
                id = string.Format("itemGroupWith_{0}_Items", numberOfItems)
            };

            foreach (var item in allItems)
            {
                item.ItemGroup = itemGroup;
            }

            itemGroup.Item = allItems;

            return itemGroup;
        }
    }
}
