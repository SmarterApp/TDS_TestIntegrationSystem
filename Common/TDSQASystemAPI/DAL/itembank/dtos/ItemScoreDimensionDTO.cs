using System;

namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..ItemScoreDimension</code> table.
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Itembank..ItemScoreDimension</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class ItemScoreDimensionDTO
    {
        public Guid ItemScoreDimensionKey { get; set; } // maps to _ke in ItemScoreDimension
        public string ItemKey { get; set; } // maps to _fk_item in ItemScoreDimension
        public string SegmentKey { get; set; } // maps to _fk_adminsubject in ItemScoreDimension
        public string Dimension { get; set; }
        public int ScorePoints { get;  set; }
        public double Weight { get; set; }
        public int MeasurementModel { get; set; } // maps to _fk_measurementmodel in ItemScoreDimension
        public string RecodeRule { get; set; }
    }
}