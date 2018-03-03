using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>ToolsTool</code> with additional 
    /// data.
    /// </summary>
    public partial class ToolsTool
    {
        /// <summary>
        /// The name of the <code>ToolsTool</code> that this tool depends on.
        /// </summary>
        [XmlIgnore]
        public string DependsOnToolType { get; set; }

        /// <summary>
        /// Dictate if this <code>ToolsTool</code> should be visible.
        /// </summary>
        [XmlIgnore]
        public bool Visible { get; set; }

        /// <summary>
        /// Dictate if this <code>ToolsTool</code> is functional.
        /// </summary>
        [XmlIgnore]
        public bool Functional { get; set; }

        /// <summary>
        /// Dictate if the student can control this <code>ToolsTool</code>.
        /// </summary>
        [XmlIgnore]
        public bool StudentControl { get; set; }

        /// <summary>
        /// Dictate if the tool is selectable.
        /// </summary>
        [XmlIgnore]
        public bool Selectable { get; set; }
    }
}
