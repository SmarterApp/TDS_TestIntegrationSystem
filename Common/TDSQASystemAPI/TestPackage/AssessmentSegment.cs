using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>AssessmentSegment</code> with additional 
    /// data.
    /// </summary>
    public partial class AssessmentSegment
    {
        [XmlIgnore]
        public Assessment Assessment { get; set; }

        public string Key
        {
            get
            {
                return string.Format("({0}){1}-{2}", Assessment.TestPackage.publisher, id, Assessment.TestPackage.academicYear);
            }
        }
    }
}
