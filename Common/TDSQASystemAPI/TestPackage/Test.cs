using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>Test</code> with additional 
    /// data.
    /// </summary>
    public partial class Test
    {
        /// <summary>
        /// The <code>TestPackage</code> that owns this <code>Test</code>.
        /// </summary>
        [XmlIgnore]
        public TestPackage TestPackage { get; set; }

        /// <summary>
        /// Get the full unique identifier of this <code>Test</code>.
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
        /// A convenience method for determining if this <code>Test</code> has more than one 
        /// <code>TestSegment</code>.
        /// </summary>
        /// <returns>True if this <code>Test</code> has more than one segment; otherwise false.</returns>
        public bool IsSegmented()
        {
            return Segments.Length > 1;
        }
    }
}
