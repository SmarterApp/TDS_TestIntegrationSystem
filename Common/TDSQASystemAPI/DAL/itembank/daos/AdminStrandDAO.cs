using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>AdminStrandDTO</code>s to the <code>OSS_Itembank..tblAdminStrand</code> table
    /// </summary>
    public class AdminStrandDAO : TestPackageDaoBase<AdminStrandDTO>
    {
        public AdminStrandDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "AdminStrandTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblAdminStrand (" +
                "       _key, " +
                "       _fk_adminsubject, " +
                "       _fk_strand, " +
                "       minitems, " +
                "       maxitems, " +
                "       isstrictmax, " +
                "       bpweight, " +
                "       adaptivecut, " +
                "       startability, " +
                "       startinfo, " +
                "       scalar, " +
                "       loadconfig, " +
                "       loadmin, " +
                "       loadmax, " +
                "       precisiontarget, " +
                "       precisiontargetmetweight, " +
                "       precisiontargetnotmetweight, " +
                "       abilityweight)\n" +
                "SELECT \n" +
                "   AdminStrandKey, \n" +
                "   SegmentKey, \n" +
                "   StrandKey, \n" +
                "   MinItems, \n" +
                "   MaxItems, \n" +
                "   IsStrictMax, \n" +
                "   BlueprintWeight, \n" +
                "   AdaptiveCut, \n" +
                "   StartAbility, \n" +
                "   StartInfo, \n" +
                "   Scalar, \n" +
                "   TestVersion, \n" +
                "   LoadMin, \n" +
                "   LoadMax, \n" +
                "   PrecisionTarget, \n" +
                "   PrecisionTargetMetWeight, \n" +
                "   PrecisionTargetNotMetWeight, \n" +
                "   AbilityWeight \n";
        }

        public override void Insert(IList<AdminStrandDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
