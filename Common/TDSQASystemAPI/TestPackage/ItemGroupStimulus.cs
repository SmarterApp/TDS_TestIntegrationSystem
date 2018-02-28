using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>ItemGroupStimulus</code> with additional 
    /// data.
    /// </summary>
    public partial class ItemGroupStimulus
    {
        [XmlIgnore]
        public TestPackage TestPackage { get; set; }
    }
}
