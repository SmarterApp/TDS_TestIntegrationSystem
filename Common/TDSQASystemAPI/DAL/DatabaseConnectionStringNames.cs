namespace TDSQASystemAPI.DAL
{
    /// <summary>
    /// The names of the various connection strings stored in the app.config file.
    /// </summary>
    public static class DatabaseConnectionStringNames
    {
        /// <summary>
        /// Name of the connection string in app.config that points to the OSS_Configs database.
        /// </summary>
        public const string CONFIGS = "configs";

        /// <summary>
        /// Name of the connection string in app.config that points to the OSS_Itembank database.
        /// </summary>
        public const string ITEMBANK = "itembank";

        /// <summary>
        /// Name of the connection string in app.config that points to the OSS_TestScoringConfigs database.
        /// </summary>
        public const string SCORING = "scoring";

        /// <summary>
        /// Name of the connection string in app.config that points to the OSS_TIS database.
        /// </summary>
        public const string TDSQC = "TDSQC";
    }
}
