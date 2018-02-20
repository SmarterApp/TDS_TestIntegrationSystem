using System;

namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..ItemMeasurementParameter</code> table.
    /// </summary>
    public class ItemMeasurementParameterDTO
    {
        public Guid ItemScoreDimensionKey { get; set; } // maps to _fk_itemscoredimension in ItemMeasurementParameter
        public int MeasurementParameterKey { get; set; } // maps to _fk_measurementparameter in ItemMeasurementParameter
        public float ParmValue { get; set; }
    }
}
