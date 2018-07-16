USE OSS_TIS;

BEGIN TRAN create_new_project;
DECLARE @newProjectId int;

-- In some cases, the max id in QC_ProjectMetaData is higher than what's in Projects.
; WITH
	maxProjectId AS (
	SELECT MAX(_Key) AS project_id FROM Projects
	UNION
	SELECT MAX(_fk_ProjectID) AS project_id FROM QC_ProjectMetaData
	)
SELECT
	@newProjectId = MAX(project_id) + 1
FROM
	maxProjectId;

-- Insert a new Project for the test package XSD
INSERT Projects (_Key, [Description])
VALUES (@newProjectId, 'SBAC Test Package XSD Project Combined');

-- Insert metadata records for the new project
INSERT QC_ProjectMetaData (_fk_ProjectID, GroupName, VarName, IntValue, FloatValue, TextValue, Comment)
VALUES(@newProjectId, 'All', 'Accommodations', 1, NULL, NULL, 'Whether to ONLY include Accommodations in the XML used in the QA System, but not in any XMLs sent downstream');

INSERT QC_ProjectMetaData (_fk_ProjectID, GroupName, VarName, IntValue, FloatValue, TextValue, Comment)
VALUES(@newProjectId, 'DoR', 'IncludeAllDemographics', 1, NULL, NULL, 'Whether to include all student attributes and relationships in the file');

INSERT QC_ProjectMetaData (_fk_ProjectID, GroupName, VarName, IntValue, FloatValue, TextValue, Comment)
VALUES(@newProjectId, 'DW1', 'Order', 1, NULL, '', '');

INSERT QC_ProjectMetaData (_fk_ProjectID, GroupName, VarName, IntValue, FloatValue, TextValue, Comment)
VALUES(@newProjectId, 'QA', 'ScoreInvalidations', 0, NULL, NULL, 'Whether to score both partial and complete invalidations');

INSERT QC_ProjectMetaData (_fk_ProjectID, GroupName, VarName, IntValue, FloatValue, TextValue, Comment)
VALUES(@newProjectId, 'DW1', 'Target', 0, NULL, NULL, 'Turn send to data warehouse on/off');

INSERT QC_ProjectMetaData (_fk_ProjectID, GroupName, VarName, IntValue, FloatValue, TextValue, Comment)
VALUES(@newProjectId, 'HandscoringTSS', 'Transform', 0, NULL, 'TDSQASystemAPI.Resources.ItemScoreRequest.xslt', 'XSLT used to transform the file prior to sending to this target');
COMMIT;