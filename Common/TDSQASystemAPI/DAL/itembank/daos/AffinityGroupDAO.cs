using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>ItemPropertyDTO</code>s to the <code>OSS_Itembank..tblItemProps</code> table
    /// </summary>
    public class AffinityGroupDAO : TestPackageDaoBase<AffinityGroupDTO>
    {
        public AffinityGroupDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "AffinityGroupTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.AffinityGroup (" +
                "       _fk_adminsubject, " +
                "       groupid, " +
                "       minitems, " +
                "       maxitems, " +
                "       weight, " +
                "       isstrictmax, " +
                "       loadconfig, " +
                "       updateconfig, " +
                "       abilityweight, " +
                "       precisiontarget, " +
                "       startability, " +
                "       startinfo, " +
                "       precisiontargetmetweight, " +
                "       precisiontargetnotmetweight) \n" +
                "SELECT \n" +
                "   SegmentKey, \n" +
                "   GroupId, \n" +
                "   MinItems, \n" +
                "   MaxItems, \n" +
                "   [Weight], \n" +
                "   IsStrictMax, \n" +
                "   TestVersion, \n" +
                "   UpdatedTestVersion, \n" +
                "   AbilityWeight, \n" +
                "   PrecisionTarget, \n" +
                "   StartAbility, \n" +
                "   StartInfo, \n" +
                "   PrecisionTargetMetWeight, \n" +
                "   PrecisionTargetNotMetWeight \n";
        }
    }
}
