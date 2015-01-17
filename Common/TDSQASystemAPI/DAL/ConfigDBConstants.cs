/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace TDSQASystemAPI.DAL
{ 
    public class ConfigDBConstants
    {
        #region Configuration constants

        internal static string LOAD_METADATA =
            @"SELECT _fk_ProjectID,GroupName, VarName, IntValue, FloatValue, TextValue, Comment
              FROM QC_ProjectMetaData";

        // Test admin period from TDS Session
        internal static string LOAD_ADMINISTRATION =
            @"SELECT _Key, StartDate, endDate
              FROM tblAdminPeriod";

        //Test enviroment from TDS configs via TDS item bank synonym 
        internal static string LOAD_TEST_ENV = @"SELECT ClientName, _efk_TestID, OppExpire, OppRestart, OppDelay FROM TDSCONFIGS_Client_TimeLimits";

        //configurable RTS attributes/relationships to grab from RTS
        internal static string LOAD_RTS_ATTRIBUTES =
            @"SELECT _fk_ProjectID,GroupName,Context,ContextDate,Decrypt,XMLName,EntityType,Relationship,FieldName,FetchIfNotInXml
              FROM RTSAttributes";

        #endregion Configuration Constants

        #region business rule constants

		public const string LOAD_CHILD_RULES =
            @"SELECT ChildRuleID FROM QC_RuleHierarchy
                WHERE ParentRuleID = @RuleID and ParentProjectID = @ProjectID";

        public const string GET_RULES =
                @"SELECT B.RuleID, b.projectID, b.ruleClass, B.Parm1, B.Parm2, B.Parm3, B.Parm4, B.Parm5, B.useStartDate,
                  StartDate, EndDate, B.usedInQCStaging, B.IsRootRule
                  FROM QC_Rules B 
                  LEFT OUTER JOIN QC_RulesRelaxValidation ON b.RuleID = _fk_RuleId AND b.ProjectId = _fk_ProjectId
                  WHERE EndDate >= StartDate  OR StartDate is null
                  ORDER BY RuleID, startDate";

        #endregion business rule constants

        #region Archive DB constants
        internal static string INSERT_ALIGNMENT_DATA =
            @"INSERT AlignmentData (OppID, DateRecorded, _fk_Strand, Count, AlignMeasure) VALUES(@OppID, @DateRecorded, @_fk_Strand, @Count, @AlignMeasure)";
        #endregion Archive DB constants
    }
}
