USE OSS_Itembank
GO
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = N'spDeleteAssessment' AND ROUTINE_TYPE = N'PROCEDURE')
BEGIN
	DROP PROCEDURE dbo.spDeleteAssessment;
END
GO
CREATE PROCEDURE dbo.spDeleteAssessment (
	@testPackageKey varchar(350)
)
AS
BEGIN
	/***********************************************************************************************************************
	  File: 7_create_procedure_spDeleteAssessment.sql

	  Desc: Delete all assessments from the TIS databases for a given test package key.

	  Example Usage:
		EXEC OSS_Itembank.dbo.spDeleteAssessment @testPackageKey = '(SBAC_PT)SBAC-IRP-MATH-7-COMBINED-Summer-2015-2016';

	***********************************************************************************************************************/
	SET NOCOUNT ON;
	SET ROWCOUNT 0;
	SET XACT_ABORT ON;

	DECLARE
		@assessmentKeys table (
			assessment_key varchar(350) NOT NULL
		);

	DECLARE
		@testIdsAndClientNames table (
			test_id varchar(255) NOT NULL,
			client_name varchar(150) NOT NULL
		);

	-------------------------------------------------------------------------------
	-- Identify all assessment keys, test ids and client names
	-------------------------------------------------------------------------------
	-- If the @testPackageKey is for a "Combined" assessment (e.g. an assessment that has a Performance Task _and_ an Adaptive Test), then collect all the 
	-- assessment keys for the assessments that comprise the combined assessment.  Otherwise, the table can be loaded with the single test package key.
	IF EXISTS(SELECT * FROM OSS_TIS.dbo.CombinationTestMap WHERE CombinationTestName = @testPackageKey)
	BEGIN
		INSERT
			@assessmentKeys(assessment_key)
		SELECT
			DISTINCT sas._Key
		FROM
			dbo.tblSetofAdminSubjects sas
		JOIN
			OSS_TIS.dbo.CombinationTestMap ctm
			ON (sas._Key = ctm.CombinationSegmentName
			OR sas._Key = ctm.CombinationTestName
			OR sas._Key = ctm.ComponentSegmentName
			OR sas._Key = ctm.ComponentTestName)
		WHERE
			ctm.CombinationTestName = @testPackageKey;

	END
	ELSE
	BEGIN
		INSERT 
			@assessmentKeys(assessment_key)
		SELECT
			_Key
		FROM
			dbo.tblSetofAdminSubjects
		WHERE
			_Key = @testPackageKey
			OR VirtualTest = @testPackageKey;
	END

	-- For each assessment key discovered in the previous step, collect all the Test IDs and their associated client names.  
	-- There are some tables that refer to an assessment by its Test ID instead of its key.
	INSERT
		@testIdsAndClientNames(test_id, client_name)
	SELECT 
		TestID,
		_fk_TestAdmin
	FROM 
		tblSetofAdminSubjects sas 
	JOIN
		@assessmentKeys ak
		ON (ak.assessment_key = sas._Key);

	BEGIN TRY
		BEGIN TRANSACTION;

		-------------------------------------------------------------------------------
		-- Delete from the loader tables in the OSS_Itembank database
		-------------------------------------------------------------------------------
		EXEC OSS_Itembank.tp.spLoader_Clear @testPackageKey;

		-------------------------------------------------------------------------------
		-- Delete from the configuration tables in the OSS_Configs database
		-------------------------------------------------------------------------------
		DELETE 
			tm 
		FROM 
			dbo.TDSCONFIGS_Client_TestMode tm 
		JOIN 
			@testIdsAndClientNames tac 
			ON (tm.testId = tac.test_id 
			AND tm.clientname = tac.client_name);

		DELETE 
			sp 
		FROM 
			dbo.TDSCONFIGS_Client_SegmentProperties sp 
		JOIN 
			@testIdsAndClientNames tac 
			ON (sp.parentTest = tac.test_id 
			AND sp.clientname = tac.client_name);

		DELETE 
			tfp 
		FROM 
			dbo.TDSCONFIGS_Client_TestformProperties tfp 
		JOIN 
			@testIdsAndClientNames tac 
			ON (tfp.TestID = tac.test_id 
			AND tfp.clientname = tac.client_name);

		DELETE 
			tw 
		FROM 
			dbo.TDSCONFIGS_Client_TestWindow tw 
		JOIN 
			@testIdsAndClientNames tac 
			ON (tw.TestID = tac.test_id 
			AND tw.clientname = tac.client_name);

		DELETE 
			tg 
		FROM 
			dbo.TDSCONFIGS_Client_TestGrades tg 
		JOIN 
			@testIdsAndClientNames tac 
			ON (tg.TestID = tac.test_id 
			AND tg.clientname = tac.client_name);

		DELETE 
			te FROM 
			dbo.TDSCONFIGS_Client_TestEligibility te 
		JOIN 
			@testIdsAndClientNames tac 
			ON (te.TestID = tac.test_id 
			AND te.Clientname = tac.client_name);

		DELETE 
			it 
		FROM 
			dbo.TDSCONFIGS_Client_Test_Itemtypes it 
		JOIN 
			@testIdsAndClientNames tac 
			ON (it.TestID = tac.test_id 
			AND it.ClientName = tac.client_name);

		DELETE 
			ic 
		FROM 
			dbo.TDSCONFIGS_Client_Test_ItemConstraint ic 
		JOIN 
			@testIdsAndClientNames tac 
			ON (ic.TestID = tac.test_id 
			AND ic.ClientName = tac.client_name);

		DELETE 
			tt 
		FROM 
			dbo.TDSCONFIGS_Client_TestTool tt 
		JOIN 
			@testIdsAndClientNames tac 
			ON (tt.Context = tac.test_id 
			AND tt.ClientName = tac.client_name);

		DELETE 
			ttt 
		FROM 
			dbo.TDSCONFIGS_Client_TestToolType ttt 
		JOIN 
			@testIdsAndClientNames tac 
			ON (ttt.Context = tac.test_id 
			AND ttt.ClientName = tac.client_name);

		DELETE 
			tsf 
		FROM 
			dbo.TDSCONFIGS_Client_TestscoreFeatures tsf 
		JOIN 
			@testIdsAndClientNames tac 
			ON (tsf.TestID = tac.test_id 
			AND tsf.ClientName = tac.client_name);

		-------------------------------------------------------------------------------
		-- Delete from the scoring configuration tables in the OSS_TestScoringConfigs 
		-- database
		-------------------------------------------------------------------------------
		DELETE 
			fcl
		FROM
			dbo.SCORECONFIGS_Feature_ComputationLocation fcl 
		JOIN
			dbo.SCORECONFIGS_TestScoreFeature tsf 
			ON (fcl._fk_TestScoreFeature = tsf._Key)
		JOIN
			@testIdsAndClientNames tac
			ON (tac.test_id = tsf.TestID
			AND tac.client_name = tsf.ClientName);

		DELETE
			crpv
		FROM
			dbo.SCORECONFIGS_ComputationRuleParameterValue crpv 
		JOIN
			dbo.SCORECONFIGS_TestScoreFeature tsf 
			ON (crpv._fk_TestScoreFeature = tsf._Key)
		JOIN
			@testIdsAndClientNames tac
			ON (tac.test_id = tsf.TestID
			AND tac.client_name = tsf.ClientName);

		DELETE 
			tsf 
		FROM 
			dbo.SCORECONFIGS_TestScoreFeature tsf 
		JOIN 
			@testIdsAndClientNames tac 
			ON (tsf.TestID = tac.test_id 
			AND tsf.clientname = tac.client_name);

		DELETE 
			tg 
		FROM 
			dbo.SCORECONFIGS_TestGrades tg 
		JOIN 
			@testIdsAndClientNames tac 
			ON (tg.TestID = tac.test_id 
			AND tg.clientname = tac.client_name);

		DELETE 
			test 
		FROM 
			dbo.SCORECONFIGS_Test test 
		JOIN 
			@testIdsAndClientNames tac 
			ON (test.testID = tac.test_id 
			AND test.clientname = tac.client_name);

		-------------------------------------------------------------------------------
		-- Delete from the item bank tables in the OSS_Itembank database
		-------------------------------------------------------------------------------
		DELETE 
			aaicl 
		FROM 
			dbo.AA_ItemCL aaicl 
		JOIN 
			@assessmentKeys ak 
			ON (ak.assessment_key = aaicl._fk_AdminSubject);

		DELETE 
			setStrands 
		FROM 
			dbo.tblSetofItemStrands setStrands 
		JOIN 
			@assessmentKeys ak 
			ON (ak.assessment_key = setStrands._fk_AdminSubject);

		DELETE 
			setStimuli 
		FROM 
			dbo.tblSetofItemStimuli setStimuli 
		JOIN 
			@assessmentKeys ak 
			ON (ak.assessment_key = setStimuli._fk_AdminSubject);
		

		DELETE
			agi
		FROM
			dbo.AffinityGroupItem agi 
		JOIN
			dbo.AffinityGroup ag 
			ON (agi._fk_AdminSubject = ag._fk_AdminSubject
			AND agi.GroupID = ag.GroupID)
		JOIN
			@assessmentKeys ak
			ON (ak.assessment_key = ag._fk_AdminSubject);

		DELETE ag FROM dbo.AffinityGroup ag JOIN @assessmentKeys ak ON (ak.assessment_key = ag._fk_AdminSubject); -- WHERE _fk_AdminSubject = @testPackageKey;

		DELETE
			imp
		FROM
			dbo.ItemMeasurementParameter imp 
		JOIN
			dbo.ItemScoreDimension isd 
			ON (imp._fk_ItemScoreDimension = isd._Key)
		JOIN
			@assessmentKeys ak
			ON (ak.assessment_key = isd._fk_AdminSubject);

		DELETE 
			isd 
		FROM 
			dbo.ItemScoreDimension isd 
			JOIN @assessmentKeys ak 
			ON (ak.assessment_key = isd._fk_AdminSubject);

		DELETE 
			stg 
		FROM 
			dbo.SetofTestGrades stg 
		JOIN @assessmentKeys ak 
			ON (ak.assessment_key = stg._fk_AdminSubject);

		DELETE 
			ast 
		FROM 
			dbo.tblAdminStimulus ast 
		JOIN 
			@assessmentKeys ak 
			ON (ak.assessment_key = ast._fk_AdminSubject); 

		DELETE 
			asr 
		FROM 
			dbo.tblAdminStrand asr 
		JOIN 
			@assessmentKeys ak 
			ON (ak.assessment_key = asr._fk_AdminSubject);

		DELETE 
			ig 
		FROM 
			dbo.tblItemGroup ig 
		JOIN 
			@assessmentKeys ak 
			ON (ak.assessment_key = ig._fk_AdminSubject);

		DELETE 
			isp 
		FROM 
			dbo.tblItemSelectionParm isp 
		JOIN 
			@assessmentKeys ak 
			ON (ak.assessment_key = isp._fk_AdminSubject);

		DELETE
			tip
		FROM
			dbo.tblItemProps tip 
		JOIN
			dbo.tblSetofAdminItems sai 
			ON (tip._fk_Item = sai._fk_Item)
		JOIN
			@assessmentKeys ak
			ON (ak.assessment_key = sai._fk_AdminSubject);

		DELETE 
			sai 
		FROM 
			dbo.tblSetofAdminItems sai 
		JOIN 
			@assessmentKeys ak 
			ON (ak.assessment_key = sai._fk_AdminSubject);

		DELETE
			tfi
		FROM 
			dbo.TestFormItem tfi 
		JOIN
			dbo.TestForm tf 
			ON (tfi._fk_Testform = tf._Key)
		JOIN
			@assessmentKeys ak
			ON (ak.assessment_key = tf._fk_AdminSubject);

		DELETE 
			tf 
		FROM 
			dbo.TestForm tf 
		JOIN 
			@assessmentKeys ak 
			ON (ak.assessment_key = tf._fk_AdminSubject);

		DELETE 
			tc 
		FROM 
			dbo.TestCohort tc 
		JOIN 
			@assessmentKeys ak 
			ON (ak.assessment_key = tc._fk_AdminSubject);

		DELETE 
			sas 
		FROM 
			dbo.tblSetofAdminSubjects sas 
		JOIN 
			@assessmentKeys ak 
			ON (ak.assessment_key = sas._Key
			OR ak.assessment_key = sas.VirtualTest);

		-- Delete items that are no longer in use by any assessment for the specified
		-- client
		DELETE
			item
		FROM
			OSS_Itembank.dbo.tblItem item
		LEFT JOIN
			OSS_Itembank.dbo.tblSetofAdminItems adminItems
			ON (item._Key = adminItems._fk_Item)
		WHERE
			adminItems._fk_Item IS NULL;

		DELETE
			stim
		FROM
			OSS_Itembank.dbo.tblStimulus stim
		LEFT JOIN
			OSS_Itembank.dbo.tblAdminStimulus adminStim
			ON (stim._Key = adminStim._fk_Stimulus)
		WHERE
			adminStim._fk_Stimulus IS NULL;

		DELETE
			strand
		FROM
			OSS_Itembank.dbo.tblStrand strand
		LEFT JOIN
			OSS_Itembank.dbo.tblAdminStrand adminStrands
			ON (strand._Key = adminStrands._fk_Strand)
		WHERE
			adminStrands._fk_Strand IS NULL;

		-------------------------------------------------------------------------------
		-- Delete from the assessment metadata tables in the OSS_TIS database
		-------------------------------------------------------------------------------
		DELETE  
			ctfm
		FROM 
			OSS_TIS.dbo.CombinationTestFormMap ctfm 
		JOIN
			OSS_TIS.dbo.CombinationTestMap ctm 
			ON (ctfm.ComponentSegmentName = ctm.ComponentSegmentName)
		WHERE
			ctm.ComponentSegmentName = @testPackageKey;
		 
		DELETE FROM 
			OSS_TIS.dbo.CombinationTestMap 
		WHERE 
			ComponentSegmentName = @testPackageKey;

		DELETE FROM 
			OSS_TIS.dbo.TestNameLookUp 
		WHERE 
			TestName = @testPackageKey;

		DELETE
			pmd
		FROM
			OSS_TIS.dbo.QC_ProjectMetaData pmd
		JOIN
			@assessmentKeys ak
			ON (pmd.VarName LIKE '%' + ak.assessment_key + '%');

		DELETE
			pmd
		FROM
			OSS_TIS.dbo.QC_ProjectMetaData pmd
		where 
			pmd.VarName LIKE '%' +  @testPackageKey + '%';


		 COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK;
		END;

		THROW;
	END CATCH
END
GO