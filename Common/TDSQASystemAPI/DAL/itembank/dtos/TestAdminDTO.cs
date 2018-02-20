namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..tblTestAdmin</code> table
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Itembank..tblTestAdmin</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class TestAdminDTO
    {
        public string AdminKey { get; set; } // maps to _key in tblTestAdmin
        public string SchoolYear { get; set; }
        public string Season { get; set; }
        public int ClientKey { get; set; } // maps to _fk_client in tblTestAdmin
        public string Description { get; set; } // calculated on line 46 of tp.spLoad_TestAdmin
        public long TestVersion { get; set; } // maps to loadconfig in tblTestAdmin
    }
}
