namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..MeasurementParameter</code> table.
    /// </summary>
    /// <remarks>
    /// The <code>OSS_Itembank..MeasurementParameter</code> table is populated via a seed data script that is
    /// run when TIS is first deployed.
    /// </remarks>
    public class MeasurementParameter
    {
        public int MeasurementModelKey { get; set; } // maps to _fk_measurementmodel
        public int Number { get; set; } // maps to parmnum
        public string Name { get; set; } // maps to parmname
        public string Description { get; set; } // maps to parmdescription

        public MeasurementParameter(int measurementModelKey, int number, string name, string description)
        {
            MeasurementModelKey = measurementModelKey;
            Number = number;
            Name = name;
            Description = description;
        }
    }
}
