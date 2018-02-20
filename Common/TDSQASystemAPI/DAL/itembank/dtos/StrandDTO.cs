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
        public string SubjectKey { get; set; }
        public string Name { get; set; } // maps to bpElementName in the loader_testblueprint table
        public string ParentId { get; set; }
        public string BlueprintElementId { get; set; } // maps to _key in tblStrand
        public long ClientKey { get; set; } // maps to _fk_client in tblStrand
        public int TreeLevel { get; set; }
        public long TestVersion { get; set; } // maps to loadconfig in tblStrand
    }
}
