using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>ItemGroup</code> with additional 
    /// data.
    /// </summary>
    public partial class ItemGroup
    {
        [XmlIgnore]
        public AssessmentSegment AssessmentSegment { get; set; }
    }
}
