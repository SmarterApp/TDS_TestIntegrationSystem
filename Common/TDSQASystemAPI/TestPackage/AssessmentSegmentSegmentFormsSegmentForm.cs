using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>AssessmentSegmentSegmentFormsSegmentForm</code> with additional 
    /// data.
    /// </summary>
    public partial class AssessmentSegmentSegmentFormsSegmentForm
    {
        [XmlIgnore]
        public AssessmentSegment AssessmentSegment { get; set; }
    }
}
