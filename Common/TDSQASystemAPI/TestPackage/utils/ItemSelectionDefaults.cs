namespace TDSQASystemAPI.TestPackage.utils
{
    /// <summary>
    /// Default values for the item selection fields in the <code>SetOfAdminSubjectDTO</code>.
    /// </summary>
    public sealed class ItemSelectionDefaults
    {
        public const double START_ABILITY = 0D;
        public const double START_INFO = 1D;
        public const double SLOPE = 1D;
        public const double INTERCEPT = 1D;
        public const double BLUEPRINT_WEIGHT = 5D;
        public const double ABILITY_WEIGHT = 1;
        public const int CSET1_SIZE = 20;
        public const int CSET2_RANDOM = 1;
        public const int CSET2_INITIAL_RANDOM = 5;
        public const bool COMPUTE_ABILITY_ESTIMATES = true;
        public const double ITEM_WEIGHT = 5D;
        public const double ABILITY_OFFSET = 0D;
        public const string CSET1_ORDER = "ABILITY";
        public const double RC_ABILITY_WEIGHT = 1;
        public const double PRECISION_TARGET_MET_WEIGHT = 1D;
        public const double PRECISION_TARGET_NOT_MET_WEIGHT = 1D;
        public const bool TERMINATION_FLAGS = false;
        public const string BLUEPRINT_METRIC_FUNCTION = "bp1";
        public static readonly string[] ALGORITHM_PROPERITES = {
            "ftstartpos",
            "ftendpos",
            "bpweight",
            "startability",
            "startinfo",
            "cset1size",
            "cset1order",
            "cset2random",
            "cset2initialrandom",
            "abilityoffset",
            "itemweight",
            "precisiontarget",
            "adaptivecut",
            "toocloseses",
            "slope",
            "intercept",
            "abilityweight",
            "computeabilityestimates",
            "rcabilityweight",
            "precisiontargetmetweight",
            "precisiontargetnotmetweight",
            "terminationoverallinfo",
            "terminationrcinfo",
            "terminationmincount",
            "terminationtooclose",
            "terminationflagsand" };
    }
}
