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
    }
}
