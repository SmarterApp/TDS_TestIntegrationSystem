using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>TestAdminDTO</code>s to the <code>OSS_Itembank..tblSetOfAdminSubject</code> table
    /// </summary>
    public class SetOfAdminSubjectDAO : TestPackageDaoBase<SetOfAdminSubjectDTO>
    {
        public SetOfAdminSubjectDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "SetOfAdminSubjectTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblSetOfAdminSubjects (" +
                "       _key, " +
                "       _fk_testadmin, " +
                "       _fk_subject, " +
                "       testid, " +
                "       startability, " +
                "       startinfo, " +
                "       minitems, " +
                "       maxitems, " +
                "       slope, " +
                "       intercept, " +
                "       ftstartpos, " +
                "       ftendpos, " +
                "       ftminitems, " +
                "       ftmaxitems, " +
                "       selectionalgorithm, " +
                "       blueprintweight, " +
                "       cset1size, " +
                "       cset2random, " +
                "       cset2initialrandom, " +
                "       virtualtest, " +
                "       testposition, " +
                "       issegmented, " +
                "       itemweight, " +
                "       abilityoffset, " +
                "       cset1order, " +
                "       loadconfig, " +
                "       [contract], " +
                "       testtype, " +
                "       precisiontarget, " +
                "       adaptivecut, " +
                "       toocloseses," +
                "       abilityweight, " +
                "       computeabilityestimates, " +
                "       rcabilityweight, " +
                "       precisiontargetmetweight, " +
                "       precisiontargetnotmetweight, " +
                "       terminationoverallinfo, " +
                "       terminationrcinfo, " +
                "       terminationmincount, " +
                "       terminationtooclose, " +
                "       terminationflagsand, " +
                "       bpmetricfunction) \n" +
                "SELECT \n" +
                "   SegmentKey, \n" +
                "   TestAdminKey, \n" +
                "   SubjectKey, \n" +
                "   TestId, \n" +
                "   StartAbility, \n" +
                "   StartInfo, \n" +
                "   MinItems, \n" +
                "   MaxItems, \n" +
                "   Slope, \n" +
                "   Intercept, \n" +
                "   FieldTestStartPosition, \n" +
                "   FieldTestEndPosition, \n" +
                "   FieldTestMinItems, \n" +
                "   FieldTestMaxItems, \n" +
                "   SelectionAlgorithm, \n" +
                "   BlueprintWeight, \n" +
                "   CSet1Size, \n" +
                "   CSet2Random, \n" +
                "   CSet2InitialRandom, \n" +
                "   VirtualTest, \n" +
                "   TestPosition, \n" +
                "   IsSegmented, \n" +
                "   ItemWeight, \n" +
                "   AbilityOffset, \n" +
                "   CSet1Order, \n" +
                "   TestVersion, \n" +
                "   [Contract], \n" +
                "   TestType, \n" +
                "   PrecisionTarget, \n" +
                "   AdaptiveCut, \n" +
                "   TooCloseSEs, \n" +
                "   AbilityWeight, \n" +
                "   ComputeAbilityEstimates, \n" +
                "   RcAbilityWeight, \n" +
                "   PrecisionTargetMetWeight, \n" +
                "   PrecisionTargetNotMetWeight, \n" +
                "   TerminationOverallInfo, \n" +
                "   TerminationRcInfo, \n" +
                "   TerminationMinCount, \n" +
                "   TerminationTooClose, \n" +
                "   TerminationFlagsAnd, \n" +
                "   BlueprintMetricFunction \n";
        }

        public override void Insert(IList<SetOfAdminSubjectDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
