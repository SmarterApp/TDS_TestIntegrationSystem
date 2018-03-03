namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// Represents the default values for a <code>ToolsTool</code>.
    /// <remarks>
    /// This class exists because there is no easy/obvious way to generate nullable primitive types for classes created by
    /// xsd.exe.  This class helps with the tool configuration hierarchy of Test Package XML -> Default Tool Setting ->
    /// Default from Test Package XSD by supplying nullable properties.
    /// </remarks>
    /// </summary>
    public class DefaultToolConfiguration
    {
        /// <summary>
        /// The name of the tool.
        /// <remarks>
        /// Cannot be null.
        /// </remarks>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The student package field name, which comes from ART.
        /// <remarks>
        /// Cannot be null.
        /// </remarks>
        /// </summary>
        public string StudentPackageFieldName { get; set; }

        /// <summary>
        /// The tool this tool dependes on, if any.
        /// </summary>
        public string DependsOnToolType { get; set; }

        /// <summary>
        /// The type of tool.
        /// <remarks>
        /// Can be null.
        /// </remarks>
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The position in which this tool should be sorted.
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// Dictate whether the tool can be changed.
        /// </summary>
        public bool? AllowChange { get; set; }

        /// <summary>
        /// Dictate whether the tool is required.
        /// </summary>
        public bool? Required { get; set; }

        /// <summary>
        /// Dictate whether the tool is selectable.
        /// </summary>
        public bool? Selectable { get; set; }

        /// <summary>
        /// Dictate whether the tool is visible.
        /// </summary>
        public bool? Visible { get; set; }

        /// <summary>
        /// Dictate if the student has control over this tool.
        /// </summary>
        public bool? StudentControl { get; set; }

        /// <summary>
        /// Dictate whether the tool should be disabled when a student/user is logged in as a GUEST.
        /// </summary>
        public bool? DisableOnGuest { get; set; }

        /// <summary>
        /// Dictate whether the "allow multpiple" flag should be enabled.
        /// </summary>
        public bool? AllowMultiple { get; set; }

        /// <summary>
        /// Dictate whether the "functional" flag should be enabled.
        /// </summary>
        public bool? Functional { get; set; }
    }
}
