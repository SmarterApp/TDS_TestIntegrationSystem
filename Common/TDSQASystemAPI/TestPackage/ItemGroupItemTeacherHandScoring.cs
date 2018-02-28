using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>ItemGroupItemTeacherHandScoring</code> with additional 
    /// data.
    /// </summary>
    public partial class ItemGroupItemTeacherHandScoring
    {
        [XmlIgnore]
        public TestPackage TestPackage { get; set; }

        [XmlIgnore]
        public ItemGroupItem ItemGroupItem { get; set; }
    }
}
