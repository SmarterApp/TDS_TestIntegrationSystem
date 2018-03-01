using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>ItemGroupItem</code> with additional 
    /// data.
    /// </summary>
    public partial class ItemGroupItem
    {
        [XmlIgnore]
        public TestPackage TestPackage { get; set; }

        [XmlIgnore]
        public AssessmentSegmentSegmentFormsSegmentForm SegmentForm { get; set; }

        [XmlIgnore]
        public ItemGroup ItemGroup { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Position
        {
            get
            {
                IEnumerable<ItemGroupItem> allItems;

                if (SegmentForm != null)
                {
                    allItems = from ig in SegmentForm.ItemGroup
                               from item in ig.Item
                               select item;
                }
                else
                {
                    allItems = from item in ItemGroup.Item
                               select item;
                }

                return Array.IndexOf(allItems.ToArray(), this) + 1;
            }
        }
    }
}
