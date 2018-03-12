using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>ItemGroupStimulus</code> with additional 
    /// data.
    /// </summary>
    public partial class ItemGroupStimulus
    {
        /// <summary>
        /// The <code>TestPackage</code> that owns this <code>ItemGroupStimulus</code>.
        /// </summary>
        [XmlIgnore]
        public TestPackage TestPackage { get; set; }

        [XmlIgnore]
        public AssessmentSegment AssessmentSegment { get; set; }

        [XmlIgnore]
        public ItemGroup ItemGroup { get; set; }

        /// <summary>
        /// Return the identifier/"key" of this <code>ItemGroupStimulus</code>
        /// </summary>
        [XmlIgnore]
        public string Key { get { return string.Format("{0}-{1}", TestPackage.bankKey, id); } }
    }
}
