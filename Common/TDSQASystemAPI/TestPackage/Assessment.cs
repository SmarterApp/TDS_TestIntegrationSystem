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

        [XmlIgnore]
        public string Key
        {
            get
            {
                return string.Format("({0}){1}-{2}", TestPackage.publisher, id, TestPackage.academicYear);
            }
        }
    }
}
