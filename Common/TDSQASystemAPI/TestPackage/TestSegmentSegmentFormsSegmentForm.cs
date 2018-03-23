using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>TestSegmentSegmentFormsSegmentForm</code> with additional 
    /// data.
    /// </summary>
    public partial class TestSegmentSegmentFormsSegmentForm
    {
        /// <summary>
        /// The <code>TestSegment</code> that owns this <code>TestSegmentSegmentFormsSegmentForm</code>.
        /// </summary>
        [XmlIgnore]
        public TestSegment TestSegment { get; set; }
    }
}
