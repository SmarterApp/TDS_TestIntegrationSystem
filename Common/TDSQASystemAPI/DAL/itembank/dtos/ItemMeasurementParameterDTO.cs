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

        public override bool Equals(object obj)
        {
            var itemMeasurementParameterDTO = obj as ItemMeasurementParameterDTO;

            if (itemMeasurementParameterDTO == null)
            {
                return false;
            }

            return this.ItemScoreDimensionKey.Equals(itemMeasurementParameterDTO.ItemScoreDimensionKey) &&
                this.MeasurementParameterKey.Equals(itemMeasurementParameterDTO.MeasurementParameterKey);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                if (ItemScoreDimensionKey != null)
                {
                    hash = hash * 23 + ItemScoreDimensionKey.GetHashCode();
                }
                hash = hash * 23 + MeasurementParameterKey.GetHashCode();
                return hash;
            }
        }

    }
}
