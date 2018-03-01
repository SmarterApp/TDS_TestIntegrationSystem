using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>AssessmentSegment</code> with additional 
    /// data.
    /// </summary>
    public partial class AssessmentSegment
    {
        /// <summary>
        /// The <code>Assessment</code> that owns this <code>AssessmentSegment</code>.
        /// </summary>
        [XmlIgnore]
        public Assessment Assessment { get; set; }

        /// <summary>
        /// The unique identifier for this <code>AssessmentSegment</code>.
        /// </summary>
        public string Key
        {
            get
            {
                return string.Format("({0}){1}-{2}", Assessment.TestPackage.publisher, id, Assessment.TestPackage.academicYear);
            }
        }
    }
}
