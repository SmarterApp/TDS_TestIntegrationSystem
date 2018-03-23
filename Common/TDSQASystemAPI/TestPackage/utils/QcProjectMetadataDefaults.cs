namespace TDSQASystemAPI.TestPackage.utils
{
    /// <summary>
    /// Represents the various statuses for the records in the <code>OSS_TIS..QC_ProjectMetaData</code>.
    /// </summary>
    /// <remarks>
    /// These status values are appeneded to the end of the <code>VarName</code> field.
    /// </remarks>
    public class QcProjectMetadataDefaults
    {
        /// <summary>
        /// The description of the project for <code>Assessment</code>s loaded into TIS.
        /// </summary>
        /// <remarks>
        /// This project description is defined in
        /// TDS_TestIntegrationSystem\TDSQAService\OSS.TIS\SQL\TISDB\7_oss_tis_add_new_project_and_metadata.sql
        /// </remarks>
        public const string TEST_PACKAGE_XSD_PROJECT_DESCRIPTION = "SBAC Test Package XSD Project";

        /// <summary>
        /// Represents the project id of the "root" QC_ProjectMetadata record for the Assessment/status 
        /// combination.
        /// </summary>
        public const int PROJECT_MAP_ID = -1;

        /// <summary>
        /// Represents the group name for the "root" QC_ProjectMetadata record for the Assessment/status 
        /// combination.
        /// </summary>
        public const string PROJECT_MAP_GROUP_NAME = "ProjectMap";

        /// <summary>
        /// Represents the comment for the "root" QC_ProjectMetadata record for the Assessment/status
        /// combination.
        /// </summary>
        public const string COMMENT = "mapping of 'HS project ID'-'test'-'[testFormat]'-'[qaLevel]'-'[discrep]'-'status' to QC Project";

        /// <summary>
        /// These statuses come from the "suffix" of the VarName field in the <code>OSS_TIS..QC_ProjectMetaData</code>
        /// table.
        /// </summary>
        public static string[] Statuses
        {
            get
            {
                return new string[]
                {
                    "completed",
                    "expired",
                    "invalidated",
                    "reported",
                    "reset",
                    "scored",
                    "submitted"
                };
            }
        }
    }
}
