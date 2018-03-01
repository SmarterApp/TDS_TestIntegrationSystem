using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>ItemGroupItemTeacherHandScoring</code> with additional 
    /// data.
    /// </summary>
    public partial class ItemGroupItemTeacherHandScoring
    {
        /// <summary>
        /// The <code>TestPackage</code> that owns this <code>ItemGroupItemTeacherHandScoring</code>
        /// </summary>
        [XmlIgnore]
        public TestPackage TestPackage { get; set; }

        /// <summary>
        /// The <code>ItemGroupItem</code> that owns this <code>ItemGroupItemTeacherHandScoring</code>.
        /// </summary>
        [XmlIgnore]
        public ItemGroupItem ItemGroupItem { get; set; }
    }
}
