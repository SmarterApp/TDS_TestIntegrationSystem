using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>TestSegment</code> with additional 
    /// data.
    /// </summary>
    public partial class TestSegment
    {
        /// <summary>
        /// The <code>Assessment</code> that owns this <code>TestSegment</code>.
        /// </summary>
        [XmlIgnore]
        public Test Test { get; set; }

        /// <summary>
        /// The unique identifier for this <code>TestSegment</code>.
        /// </summary>
        public string Key
        {
            get
            {
                return string.Format("({0}){1}-{2}", Test.TestPackage.publisher, id, Test.TestPackage.academicYear);
            }
        }
    }
}
