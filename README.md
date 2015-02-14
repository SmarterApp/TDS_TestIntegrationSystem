# Welcome to the Test Integration System (TIS)

The TIS is responsible for:

* Receiving a test result from TDS (Test Delivery System) 
* Sending it to THSS (Teacher HandScoring System) for hand scoring of items that require human scoring
* Receiving item scores back from THSS
* Inserting item scores into the file received from TDS
* Scoring the test
* Sending the scored test to downstream systems via SFTP 

The TIS consists of the following 3 modules/parts:

1. TDS Receiver
1. TIS Service 
1. TIS Scoring Daemon      

## License ##
This project is licensed under the [AIR Open Source License v1.0](http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf).

## Getting Involved ##
We would be happy to receive feedback on its capabilities, problems, or future enhancements:

* For general questions or discussions, please use the [Forum](http://forum.opentestsystem.org/viewforum.php?f=20).
* Use the **Issues** link to file bugs or enhancement requests.
* Feel free to **Fork** this project and develop your changes!

## Usage 
### TDS Receiver
This is a REST endpoint that receives test results in an XML format from the Test Delivery System.  Each result received is inserted into a database where it is picked up and processed by the TIS Service.  The TDS Receiver solution can be found here:  TISServices\TISServices\TISServices.sln.

### TIS Service
This is a Windows service that continuously looks for new test results in the database that have not yet been processed.  Once it finds these, it picks them up and processes them (either by sending to THSS, inserting scores from THSS, scoring the test or sending the test downstream). 
As part of the deployment of this application, we need to set up the database and deploy the code in app server. A Windows service needs to be installed and started for this app.
The TIS Service solution can be found here:  TDSQAService/OSSTIS.sln.

### TIS Scoring Daemon      
This is a web application that talks to the THSS (Teacher HandScoring system) and is responsible for receiving item scores from THSS and sending it to the TIS Service for further processing.  The TIS Scoring Daemon solution can be found here:  TISScoringDaemon\TISScoringDaemon.sln.

### REST EndPoint communication with [TDS, ART & Data warehouse]
The Test Integration System is built to communicate with all the peer and down-stream systems using a secured REST APIs (using OAuth). The token for secured communiation would be supported/provided by the OpenAM system.
TISServices\TISServices\TISServices.sln 

## Build & Deploy
TIS requires Visual Studio 2012 to build. The Deployment steps are as follows - 

1) Create the following databases [DB Server]:

* `OSS_TIS`
* `OSS_Itembank`
* `OSS_Configs`
* `OSS_TestScoringConfigs`

            - TIS Service will need an account with R/W/X to OSS_TIS and R/X to the other 3 dbs
            - TIS services REST endpoint will need an account with R/W/X access to OSS_TIS
            - The Scoring Daemon will need an account with R/W/X access to OSS_TIS

2) Create these folders on the application server (if they don't already exist): [App server]

* `/Services/tis_opentestsystem`
* `/oss_tis_itemscoring`
* `/oss_tisservices`

3) Deploy the `[Db server].OSS_TIS` database objects by running the following scripts in order: [DB server]

* `<root>\TDSQAService\OSS.TIS\SQL\TISDB\1_Create_Objects.sql`
* `<root>\TDSQAService\OSS.TIS\SQL\TISDB\2_Configuration.sql`  
* `<root>\TDSQAService\OSS.TIS\SQL\TISDB\3_ScoringDaemonConfiguration.sql`   (** this script will require a couple of variables to be set prior to running)

4) Deploy the `[Db server].OSS_TestScoringConfigs` database objects by running the following scripts in order:

* `<root>\TDSQAService\OSS.TIS\SQL\TestScoringConfigs\1_Tables.sql`
* `<root>\TDSQAService\OSS.TIS\SQL\TestScoringConfigs\2_Views.sql`
* `<root>\TDSQAService\OSS.TIS\SQL\TestScoringConfigs\3_Configuration.sql`

5) Deploy the `[Db server].OSS_Configs` database objects by running the following scripts in order:

* `<root>\TDSQAService\OSS.TIS\SQL\TDSConfigs\1_Create_Objects.sql`  (** Ignore the SQL warnings.)
* `<root>\TDSQAService\OSS.TIS\SQL\TDSConfigs\2_Configuration.sql`
 	
	
6) Deploy the `[Db server].OSS_Itembank` database objects by running the following scripts in order:

* `<root>\TDSQAService\OSS.TIS\SQL\TDSItemBank\1_Create_Synonyms_Sproc.sql`
* `<root>\TDSQAService\OSS.TIS\SQL\TDSItemBank\2_Create_Synonyms_Config.sql`  (** this script will require a couple of variables to be set prior to running)
* `<root>\TDSQAService\OSS.TIS\SQL\TDSItemBank\3_Create_Objects.sql`
* `<root>\TDSQAService\OSS.TIS\SQL\TDSItemBank\4_Configuration.sql`
* `<root>\TDSQAService\OSS.TIS\SQL\TDSItemBank\5_LoadPackages.sql`
* `<root>\TDSQAService\OSS.TIS\SQL\TDSItemBank\6_TestToolConfiguration.sql`

7) Deploy TISService code at `tis_opentestsystem/` [App server]

8) To deploy the 'TIS Scoring Daemon' - 

- Create a web application on the App server at `/oss_tis_itemscoring'
- Publish the 'TIS Scoring Daemon' application to `/oss_tis_itemscoring`

9) To deploy the 'TIS services REST endpoint' - 

- Create a web application on the App server at `/oss_tisservices'
- Publish the 'REST endpoint application' to `/oss_tisservices`

10) Run InstallUtil for .Net 4.5, 32-bit on `/tis_opentestsystem/TDSQAService.exe` to install the windows service. [App server]

11) Verify that the TIS service has privileges to write to the event log [App server].

## Dependencies
Test Integration System has the following dependencies that are necessary for it to compile and run. 

### Compile Time Dependencies
* .Net Framework 4.5
* Microsoft.Practices.EnterpriseLibrary.Data

### Runtime Dependencies
None

## Items/Updates included in 02/13/2015 release:

1)	\TDSQAService\OSS.TIS\SQL\TISDB\1_Create_Objects.sql: modified InsertAndArchiveXML stored procedure to copy the CallbackURL to the destination file location when archiving a file.  Otherwise, if the file in the "destination" location is resubmitted within TIS, no acknowledgement will be sent to TDS.


## Items/Updates included in 02/11/2015 release:


1)	\TDSQAService\OSS.TIS\SQL\TDSItemBank\6_TestToolConfiguration.sql: added combo test tool configuration and fixed an issue with the existing config where 'null' was being inserted instead of null.

2)	\TDSQAService\OSS.TIS\SQL\TDSItemBank\3_Create_Objects.sql: fixed a bug in the spLoader_ExtractXML sproc that was setting the TestScoreFeature.MeasureOf to the TestID for test-level measures, rather than to the required value of ‘Overall’.

3)	Fixed bug in ItemResponse where ScorePoints was never returning a value, which caused item scores of -1 to be passed to the test scoring engine, generating an error.

4)	A couple serialization fixes to handle null attribute values.

5)	If ART returns an accommodation that does not exist in the test tool configuration for the current test, skip it instead of throwing an exception.  Since ART specifies accommodations at the subject level, it’s conceivable that an accommodation may be specified for a subject in ART but not configured for all tests that the student is eligible for in that subject.

6)	Fixed null ref exception when attempting to pass the student’s accommodations from ART to the test scoring engine.


## Items/Updates included in 02/09/2015 release:

The following previously known issues have been fixed - 

1) Combination tests are being created but currently don’t include examinee attributes or relationships.

 - The following script has been updated - \TDSQAService\OSS.TIS\SQL\TISDB\2_Configuration.sql

2) An exception will be thrown when attempting to gather the inputs to the test scoring engine if any items have more than one dimension.


## Items/Updates included in 02/06/2015 release:

1)	Added SEBasedPLWithRounding scoring rule to test scoring engine and test scoring configuration script

2)	TDS will now send the primary student ID in the StudentIdentifier examinee attribute and a secondary/alternate SSID in the AlternateSSID attribute.  Previously we were only getting the primary student ID in the AlternateSSID attribute.  Modified the fetching of student accommodations from ART to attempt to use the StudentIdentifier first, then the AlternateSSID if the StudentIdentifier is not available (which should never be the case; primarily for backward compatibility).

3)	The v_RescoreAppeals view is referenced by a stored proc that is used in the OSS TIS system, so I added this back to the TIS DDL script.  The view will always be empty.

4)	Handling exceptions in sending of acknowledgement to TDS.  Previously the file would have been left in the “processing” location in the XmlRepository.  Will log as a warning by default.  Can be configured to treat as an error by setting TreatAcknowledgementFailureAsError = “true” in the app.config file; this will move the file into the “reject” location.  Note that a combination test may already have been created and submitted to TIS by this point though.

5)	Added RTSAttribute configuration for HandscoringTSS target to TIS config script.

6)	Added Transform config to the TIS configuration script for the HandscoringTSS target.

7)	Added TDSItemBank\6_TestToolConfiguration script.  This is a preliminary script that we may need to update.  Testing is underway.

8)	Fixed bug with the handling of accommodations at the segment level that override accommodations at the test level.  If the same accommodation exists at segment=”0” (test-level) and segment > 0, the accommodation with segment > 0 overrides for that segment.

9)	Removed pre-build step in Release configuration from AIR.Common project; this step is n/a for the OSS systems.

10)	Removed publish profile from TISServices project (TIS) and bin dir from TISService (REST API) project

11)	Fixed SBACItemResolutionRuleWER compile issue with overloaded ItemScoreInfo constructor.

12)	Various bug fixes for null refs and bad format strings.


------------------------------------------------------------------
## Known Issues (as of 02/11/2015):


1)	Changes will be required for creating Summative test combinations to support 1/N possible component tests.  These changes are currently being tested.


## Future Enhancements 

####1) System and Integration Testing - 

The Test Integration System has not undergone a complete system testing or integration testing with the Test Delivery System, Teacher Hand Scoring System, and Data Warehouse.  We intend to complete the System and integration testing in an upcoming release.