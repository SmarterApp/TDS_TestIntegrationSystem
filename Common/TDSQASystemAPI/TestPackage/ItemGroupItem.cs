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
        /// <summary>
        /// The <code>TestPackage</code> that owns this <code>ItemGroupItem</code>.
        /// </summary>
        [XmlIgnore]
        public TestPackage TestPackage { get; set; }

        /// <summary>
        /// The <code>TestSegment</code> that owns this <code>ItemGroupItem</code>.
        /// </summary>
        [XmlIgnore]
        public TestSegment TestSegment { get; set; }

        /// <summary>
        /// The <code>TestSegmentSegmentFormsSegmentForm</code> that owns this <code>ItemGroupItem</code>.
        /// 
        /// CAN BE NULL.
        /// <remarks>
        /// This property is only populated for fixed-form Tests.  Adaptive (aka CAT) tests do not have forms.
        /// Instead, they draw their items from a pool of available items.
        /// </remarks>
        /// </summary>
        [XmlIgnore]
        public TestSegmentSegmentFormsSegmentForm SegmentForm { get; set; }

        /// <summary>
        /// The <code>ItemGroup</code> that owns this <code>ItemGroupItem</code>.
        /// </summary>
        [XmlIgnore]
        public ItemGroup ItemGroup { get; set; }

        /// <summary>
        /// Return the identifier/"key" of this <code>ItemGroupItem</code>
        /// </summary>
        [XmlIgnore]
        public string Key { get { return string.Format("{0}-{1}", TestPackage.bankKey, id); } }

        /// <summary>
        /// Get the ordinal position of this <code>ItemGroupItem</code>.
        /// <remarks>
        /// If this item belongs to a form (i.e. <code>TestSegmentSegmentFormsSegmentForm</code> is not null),
        /// get all the items from every <code>ItemGroup</code> (a form can have many item groups) and find this item
        /// in the resulting collection.  Otherwise, look for this item the item pool's item group.
        /// </remarks>
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
