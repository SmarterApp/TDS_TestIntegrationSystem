namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>ItemGroupItem</code> with additional 
    /// data.
    /// </summary>
    public partial class ItemGroupItem
    {
        public TestPackage TestPackage { get; set; }
        public AssessmentSegmentSegmentFormsSegmentForm SegmentForm { get; set; }
        public ItemGroup ItemGroup { get; set; }
    }
}
