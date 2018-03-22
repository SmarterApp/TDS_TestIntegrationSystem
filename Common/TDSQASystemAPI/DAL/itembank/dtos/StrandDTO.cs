using System.Xml.Serialization;

namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..tblStrand</code> table.
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Itembank..tblStrand</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class StrandDTO
    {
        public string Key { get; set; } // maps to _key in tblStrand
        public string SubjectKey { get; set; } // maps to _fk_subject in tblStrand
        public string Name { get; set; } // maps to name in tblStrand , maps to bpElementName in the loader_testblueprint table
        public string ParentId { get; set; } // maps to _fk_parent in tblStrand
        public string BlueprintElementId { get; set; } // maps to _key in tblStrand
        public long ClientKey { get; set; } // maps to _fk_client in tblStrand
        public int TreeLevel { get; set; }
        public long TestVersion { get; set; } // maps to loadconfig in tblStrand

        [XmlIgnore]
        public string Type { get; set; }

        /// <summary>
        /// Used to determine if this <code>StrandDTO</code> is a "leaf" target node.  This flag is evaluated as part of the
        /// loading process.
        /// <remarks>
        /// From lines 150 & 162 of <code>AssessmentItemBankGenericDataLoaderServiceImpl</code>
        /// </remarks>
        /// <see cref="https://github.com/SmarterApp/TDS_AssessmentService/blob/1dc03698a2a608437d63b491757d366b1c217abc/service/src/main/java/tds/assessment/services/impl/AssessmentItemBankGenericDataLoaderServiceImpl.java#L150"/>
        /// </summary>
        [XmlIgnore]
        public bool IsLeafTarget { get; set; }
    }
}
