namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..tblSubject</code> table.
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Itembank..tblSubject</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class SubjectDTO
    {
        public string Name { get; set; }
        public string Grade => ""; // line 25 of spLoad_Subject
        public string SubjectKey { get; set; } // maps to _Key
        public long ClientKey { get; set; } // maps to _fk_client
        public long TestVersion { get; set; } // maps to LoadConfig
    }
}
