using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>ItemGroupStimulus</code> with additional 
    /// data.
    /// </summary>
    public partial class ItemGroupStimulus
    {
        /// <summary>
        /// The <code>TestPackage</code> that owns this <code>ItemGroupStimulus</code>.
        /// </summary>
        [XmlIgnore]
        public TestPackage TestPackage { get; set; }
    }
}
