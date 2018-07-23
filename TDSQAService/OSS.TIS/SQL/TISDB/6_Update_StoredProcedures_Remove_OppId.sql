-- Modifies TIS Stored Procedures to generate the OppId for each scored exam

USE [OSS_TIS]
GO
/****** Object:  StoredProcedure [dbo].[InsertTestOpportunityStatus]    Script Date: 8/24/2017 11:12:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[InsertTestOpportunityStatus]
    @TesteeEntityKey bigint,
    @TestName varchar(255),
    @Opportunity int,
    @OppID varchar(50),
    @Status varchar(50),
    @PassedQAValidation bit,
    @OpportunityStartDate datetime = NULL,
    @OpportunityStatusDate datetime,
    @OpportunityDateCompleted datetime = NULL,
    @DatePassedValidation datetime2(7) = NULL, -- AM: will not use this anymore; will use default instead; leaving in the sig so as not to upset the code.
    @Message varchar(1024),
    @TestID varchar(255),
    @FileID BIGINT,
    @isDemo bit,
    @TestWindowID VARCHAR(50),
    @WindowOpportunity int = null,
    @DoRRecordID bigint = null,
    @Mode varchar(50),
    @ArchiveFileID BIGINT = null,
    @SentToRB bit
AS
BEGIN
INSERT INTO TestOpportunityStatus
([_efk_Testee]
           ,[_efk_TestID]
           ,[Opportunity]
           ,[OppID]
           ,[Status]
           ,[PassedQAValidation]
           ,[OpportunityStartDate]
           ,[OpportunityStatusDate]
           ,[OpportunityDateCompleted]
           --,[DateRecorded]
           ,[Message]
           ,[TestID]
           ,[_fk_XMLRepository]
           ,[isDemo]
           ,[TestWindowID]
           ,[WindowOpportunity]
           ,[_efk_RecordID]
           ,[Mode]
           ,[_fk_XMLRepository_Archive]
           ,[SentToRB])
    SELECT 
        @TesteeEntityKey, @TestName, @Opportunity, 
        Max(OppId), 
        @Status, @PassedQAValidation, 
        @OpportunityStartDate, @OpportunityStatusDate, @OpportunityDateCompleted, /*@DatePassedValidation,*/ @Message, @TestID, @FileID, 
        @isDemo, @TestWindowID, @WindowOpportunity, @DoRRecordID, @Mode, @ArchiveFileID, @SentToRB
    FROM XMLRepository
    WHERE
        TestName = @TestName
        and _efk_Testee = @TesteeEntityKey
END

GO
/****** Object:  StoredProcedure [dbo].[InsertXmlRepository]    Script Date: 8/23/2017 4:01:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[InsertXmlRepository] 
    @Location varchar(50)
    ,@TestName VARCHAR(255)
   ,@OppId varchar(50)
   ,@TesteeKey bigint = NULL   
   ,@StatusDate datetime
   ,@IsDemo bit
   ,@Contents XML
   ,@CallbackURL varchar(500)=null
AS
BEGIN
    
  DECLARE @nextOppId VARCHAR(50)

    IF @OppId = 0 
        SELECT @nextOppId = CAST(MAX(CAST(OppId as int))+1 as varchar(50)) FROM XMLRepository WHERE OppId < 5000000
    ELSE
        SET @nextOppId = @OppId

    INSERT INTO XMLRepository
           ([Location]
           ,[TestName]
           ,[OppId]
           ,[_efk_Testee]           
           ,[StatusDate]
           ,[isDemo]
           ,[Contents]
           ,[CallbackURL])
    SELECT 
        @Location, 
        @TestName, 
        @nextOppId,
        @TesteeKey,
        @StatusDate,
        @IsDemo,
        CAST(replace(CAST(@Contents as varchar(MAX)), 'oppId="0"', concat('oppId="', @nextOppId, '"')) as XML),
        @CallbackURL

     SELECT @@identity
END
