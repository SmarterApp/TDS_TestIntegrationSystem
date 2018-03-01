using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>Assessment</code> with additional 
    /// data.
    /// </summary>
    public partial class Assessment
    {
        [XmlIgnore]
        public TestPackage TestPackage { get; set; }

        /// <summary>
        /// Get the full unique identifier of this <code>Assessment</code>
        /// </summary>
        [XmlIgnore]
        public string Key
        {
            get
            {
                return string.Format("({0}){1}-{2}", TestPackage.publisher, id, TestPackage.academicYear);
            }
        }

        /// <summary>
        /// A convenience method for determining if this <code>Assessment</code> has more than one 
        /// <code>AssessmentSegment</code>.
        /// </summary>
        /// <returns>True if this <code>Assessement</code> has more than one segment; otherwise false.</returns>
        public bool IsSegmented()
        {
            return Segments.Length > 1;
        }
    }
}
