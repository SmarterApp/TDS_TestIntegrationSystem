using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>ItemGroup</code> with additional 
    /// data.
    /// </summary>
    public partial class ItemGroup
    {
        /// <summary>
        /// The <code>AssessmentSegment</code> that owns this <code>ItemGroup</code>.
        /// </summary>
        [XmlIgnore]
        public TestSegment AssessmentSegment { get; set; }

        /// <summary>
        /// Get the "key"/unique identifier of this <code>ItemGroup</code>.
        /// </summary>
        /// <remarks>
        /// The convention is:
        /// * Item groups that have more than one item (e.g. several questions related to a passage) start with "G"
        /// * Item groups taht have only one item start with "I"
        /// </remarks>
        [XmlIgnore]
        public string Key
        {
            get
            {
                return Item.Length > 1
                    ? string.Format("G-{0}-{1}-{2}", AssessmentSegment.Assessment.TestPackage.bankKey, id, GetGroupSuffix())
                    : string.Format("I-{0}-{1}", AssessmentSegment.Assessment.TestPackage.bankKey, id);
            }
        }

        /// <summary>
        /// Get the correct item group key suffix, based on whether the assessment has multiple segments.
        /// </summary>
        /// <returns>if the Assessment is segmented, the position of the segment this <code>ItemGroup</code> belongs to; 
        /// otherwise "0" (indicating the Assessment is not segmented).</returns>
        private string GetGroupSuffix()
        {
            return AssessmentSegment.Assessment.IsSegmented()
                ? AssessmentSegment.position.ToString()
                : "0";
        }
    }
}
