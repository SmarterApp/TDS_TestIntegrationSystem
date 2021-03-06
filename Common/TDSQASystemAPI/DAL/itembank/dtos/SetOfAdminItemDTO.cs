﻿namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..tblSetOfAdminItems</code> table.
    /// </summary>
    public class SetOfAdminItemDTO
    {
        public string ItemKey { get; set; } // maps to _fk_item in tblSetOfAdminItems
        public string SegmentKey { get; set; } // maps to _fk_adminsubject in tblSetOfAdminItems
        public long TestVersion { get; set; } // maps to loadconfig in tblSetOfAdminItems
        public string StrandKey { get; set; } // maps to _fk_strand in tblSetOfAdminItems
        public string TestAdminKey { get; set; } // maps to _fk_testadmin in tblSetOfAdminItems
        public string GroupId { get; set; }
        public int ItemPosition { get; set; }
        public bool IsFieldTest { get; set; }
        public bool IsActive { get; set; }
        public string BlockId { get; set; }
        public bool IsRequired { get; set; }
        public string GroupKey { get; set; }
        public string StrandName { get; set; }
        public float IrtA { get; set; } // maps to irt_a in tblSetOfAdminItems
        public string IrtB { get; set; } // maps to irt_b in tblSetOfAdminItems
        public float IrtC { get; set; } // maps to irt_c in tblSetOfAdminItems
        public string IrtModel { get; set; } // maps to irt_model in tblSetOfAdminItems
        public string ClString { get; set; }
        public long UpdatedTestVersion { get; set; } // maps to updateconfig in tblSetOfAdminItems
        public string BVector { get; set; } // this field is set on line 92 of spLoad_AdminItemMeasurementParms

        public override bool Equals(object obj)
        {
            var setOfAdminItemDTO = obj as SetOfAdminItemDTO;

            if (setOfAdminItemDTO == null)
            {
                return false;
            }

            return this.SegmentKey.Equals(setOfAdminItemDTO.SegmentKey) &&
                this.ItemKey.Equals(setOfAdminItemDTO.ItemKey);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                if (SegmentKey != null)
                {
                    hash = hash * 23 + SegmentKey.GetHashCode();
                }
                if (ItemKey != null)
                {
                    hash = hash * 23 + ItemKey.GetHashCode();
                }
                return hash;
            }
        }

    }
}
