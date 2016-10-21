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

3) Deploy the `[Db server].OSS_TIS` database objects by running the following scripts in order: [DB server]. Note that the IAB- and ICA/Operational- specific configurations are only required if TIS is to process those assessment types.

* `<root>\TDSQAService\OSS.TIS\SQL\TISDB\1_Create_Objects.sql`
* `<root>\TDSQAService\OSS.TIS\SQL\TISDB\2_Configuration_IAB Tests.sql`
* `<root>\TDSQAService\OSS.TIS\SQL\TISDB\2_Configuration_ICA_OP Tests.sql`
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

## Items/Updates - 08/17/2015:

1) 	Modified TISExtender.ShouldScore method to only score the test if it's not a reset and there are no items that need scoring (operational, selected, not dropped, not marked as notForScoring)

2) 	Changed the way assessmentVersion value is derived for combo tests.  It will now be an ordered, pipe-delimited string of the assessmentVersion values from all components currently included in the combo.

3) 	Added sample Summative test configuration to \TDSQAService\OSS.TIS\SQL\TISDB\2_Configuration.sql.  Includes project mapping and combo mapping.  Does not include project settings.

# Loading Test Packages

Both the Administration and Scoring Test Packages need to be loaded into TIS.  For summative tests, the Performance and CAT Administration packages and the Combined Administration and Scoring packages need to be loaded.

## Implementation Readiness Package Example

The latest Implementation Readiness test packages can be found here: <ftp://ftps.smarterbalanced.org/~sbacpublic/Public/ImplementationReadiness/>.  Download and unzip this file.

As an example, to load the Grade 3 Math summative tests into TIS, you will need to load the following packages:

* /Test Packages
	* /TDS/Administration
		* (SBAC_PT)SBAC-IRP-CAT-MATH-3-Summer-2015-2016.xml
		* (SBAC_PT)SBAC-IRP-Perf-MATH-3-Summer-2015-2016.xml
	* /TIS/Administration
		* (SBAC_PT)SBAC-IRP-MATH-3-COMBINED-Summer-2015-2016.xml
	* /TIS/Scoring
		* (SBAC_PT)SBAC-IRP-MATH-3-COMBINED-Summer-2015-2016.xml

## Loading script

To load the test packages you will copy and paste the XML from each file and run the following against the `OSS_Itembank` database:

> EXEC tp.spLoader_Main '`[Test Package file(XML file)]`'
> 
> EXEC dbo.UpdateTDSConfigs 1

After loading the tests the `OSS_TIS.dbo.TestNameLookUp`, `OSS_Configs.dbo.Client_TestTool` and `OSS_Configs.dbo.Client_TestToolType` will need to be populated.  See the database configuration section below for more details.

# Configuration

## Applications

### TDS Receiver

#### TISServices\TISServices\Web.config

**WebServiceSettings**  

The `<WebService>` element is used to define the connection to OpenAM.

Name		| Url	| Description
:-------- | :--- | :-----------
OpenAM		| https://`[OPENAM BASE URL]`/auth/oauth2/tokeninfo | The URL for the token information endpoint of OpenAM.  Replace [OPENAM BASE URL] with the appropriate URL.

**connectionStrings**  

There is a single connection string to configure that must point to the `OSS_TIS` database.

Name		| Example Value	| Description
:-------- | :--- | :-----------
TDSQC		| Data Source=`[DATABASE IP OR URL]`;Initial Catalog=OSS_TIS;User id=`[USERNAME]`;Password=`[PASSWORD]` | Use any valid SQL Server connecion string that connects to the OSS_TIS database.

**appSettings**  

Name          	| Example Value   	| Description
:------------- 	| :------------- 	| :-------
LogFilePath		| C:\Logs\TDSReceiveLog.txt| Full path to the log file. 
AuthTokenCache:MaxSize  | 10	| The max number of tokens to store in the cache. 
AuthTokenCache:PurgeCount | 5	| The number of entries to purge if the count is >= MaxSize. 
AuthTokenCache:SlidingExpirationMinutes | 90 | If the cache is not accessed in this number of minutes, it will be dumped from memory.

NOTE: The AuthTokenCache stores authenticaion tokens sent with requests in order to decrease the number of calls to OpenAM in order to validate the token.

### TIS Service
#### TDSQAService\TISService\App.config

**WebServiceSettings**  

The `<WebService>` element is used to define the connection to the Teacher Handscoring System (THSS), ART and the Data Warehouse.  

NOTE: The `name` of the handscoring webservices are used as the `target` of the `<ItemScoring>` element.  The `authSettingName` refers to the `name` of the `<Authorization>` element defined below, and is used to determine how to authenticate before calling a particular web service.

Name		| Url	| AuthSettingName | Description
:-------- | :--- | :----------- | :-----
HandscoringTSS		| https://`[THSS BASE URL]`/api/test/submit | OAuth | The endpoint where items that need to be hand scored will be sent.
HandscoringTDSUnscored | https://`[TDS BASE URL]`/itemscoring/Scoring/ItemScoring | OAuth | The URL to the [Item Scoring Engine](https://github.com/SmarterApp/TDS_ItemScoring).  This assumes it is installed on the TDS server.
ART		| https://`[ART_BASE_URL]`/rest	| OAuth | ART rest endpoint used to get the student package.
DW1 (DW2, DW3, etc.)	| https://`[DATA_WAREHOUSE_URL]`/services/xml | OAuth | Data warehouse Score Batcher endpoint

**connectionStrings**  

There are two connection strings to configure that point to the `OSS_ItemBank` and the `OSS_TIS` databases.

Name		| Example Value	| Description
:-------- | :--- | :-----------
ITEMBANK		| Data Source=`[DATABASE IP OR URL]`;Initial Catalog=OSS_ItemBank;User id=`[USERNAME]`;Password=`[PASSWORD]` | Use any valid SQL Server connecion string that connects to the OSS_ItemBank database.
TDSQC		| Data Source=`[DATABASE IP OR URL]`;Initial Catalog=OSS_TIS;User id=`[USERNAME]`;Password=`[PASSWORD]` | Use any valid SQL Server connecion string that connects to the OSS_TIS database.

**AuthorizationSettings**

Multiple `<Authorization>` elements can be configured here and are used by the `<WebService>` elements above to get an OAuth token and allow access to the web service. Each attribute is defined below.

Attribute Name	| Example Value	| Description
:-------- | :------------- 	| :-------
name		| OAuth | Name to use as a reference.  Matches the `WebServiceSettings\WebService authSettingName` attribute
url			| https://`[SSO_URL]`/ | OpenAM base url.  **IMPORTANT:** This is only the base url and does not include the /auth/oauth2 path as that is added within the code itself.
realm		| /sbac	| The realm used by OpenAM
grantType	| password	| Should always be the text "password" (that is not a placeholder) for these use cases
username	| admin	| A user in the system that has elevated privileges
password	| somepassword | The password for this user
clientId	| pm	| The OpenAM OAuth configured client id.  The installation process configures a "pm" client which can be used out of the box.  You can view the clients in the OpenAM console by going to the Access Control tab -> click **sbac** link -> Agents -> OAuth2.0/OpenID Connect tab.
clientSecret | sbac12345 | This is the default password for the "pm" client and should have been changed during installation.
passwordEncrypted | false | A boolean value that defines whether the passwords provided  in this file are encrypted.

Below is an example of the main `OAuth` authorization element that is needed when configuring the ART web service shown above.

> \<Authorization name="OAuth" url="https://sso.sbtds.org/" realm="/sbac" grantType="password" username="adminuser" password="somepassword" clientId="pm" clientSecret="sbac12345" passwordEncrypted="false" />


**ItemScoringSettings**

Multiple `<ItemScoring>` elements can be configured here and are used by TIS to determine where to sent items that still need to be scored.  In most situations there will be two elements configured; one for sending items to the Handscoring System and the other for sending Math Equations to be machine scored.  Examples of these are provided after each attribute is described in the table below.

Attribute Name	| Example Value	| Description
:-------- | :------------- 	| :-------
target		| HandscoringTSS | WebService target that should be used.  Matches the `WebServiceSettings\WebService name` attribute
callbackUrl			| https://`[TIS_SCORING_DAEMON_URL]`/ ItemScoringCallbackRcv.axd | The callback URL that the scoring server will use to notify TIS of the results.   `ItemScoringCallbackRcv.axd` should be used for the Teacher Handscoring target and `ItemScoringCallback.axd` should be used for the unscored items sent to the Item Scoring Engine.
itemTypes | SA;WER;TI:200-25662,200-19678,200-6117 | Defines the items that should be included in this group and therefore sent to the defined target.  See the **"ItemTypes Format"** section below for more detail on the format itself.
scoreStatuses | NotScored,ScoringError | A comma-separated list of score statuses that should be included.  Defaults to "NotScored" if no value is provided.  **IMPORTANT** If items that are not SA or WER are included to be handscored, you will need to include `ScoringError` in the statuses since TDS will try to score these items and mark them as an error with a message of "Rubric does not exist."
scoreInvalidations | True | Defines if invalidations should be scored.  Defaults to True if no value is provided.
updateSameReportingVersion | True | Defaults to True if no value is provided.
isHandscoringTarget | True | Set to True for the HandscoringTSS target and to False for the HandscoringTDSUnscored (e.g. Item Scoring Engine).  Defaults to False if value is not provided.
batchRequest | False | Defines if results should be sent as batches or individually.  The Teacher Handscoring System can handle batch requests and therefore can be set to True.  The Item Scoring Engine target can not and therefore must be set to False or left blank.  Defaults to False if not provided.

_**ItemTypes Format**_  

The item types string follows a specific format that can be summarized like so: `{itemType}:{itemKey},{itemKey}:{isExcludedItems};{itemType}...` where only the `{itemType}` is required.

In it's simplest form, the string will contain a semicolon separated list of item types that should be sent to this particular target.  This would look like: `SA;WER;TI`.

Including one or more `{itemKey}`'s limits the items that will be included for this `{itemType}` to only those listed.  So `SA;WER;TI:200-25662,200-19678` means that all SA and WER items are included and only item 200-25662 and 200-19678 are included for the TI type.

The last option `{isExcludedItems}` is a boolean value that defines if the list of `{itemKey}`'s provided should be included or exceluded.  If it isnt provided it defaults to `true`, meaning the items are included. Therefore `SA;WER;TI:200-25662,200-19678:true` means that all SA and WER items are included and all TI items except 200-25662 and 200-19678 will be included.


**appSettings**  

Name          	| Example Value   	| Description
:------------- 	| :------------- 	| :-------
ServiceName		| OSS_TISService | This should only be changd in rare circumstances where there are multiple Services accessing the same TIS database.  This value is used in the `OSS_TIS.dbo.TestNameLookUp` `InstanceName` column to determine which tests this service processes.
MaxErrorEMails  | 25	| The max number of error emails to send. 
FatalErrorsTo | email@email.com	| Email address where fatal errors are sent.
FatalErrorsCc | email@email.com | Email address where fatal errors are sent via CC.
EmailAlertForWarnings | True | Boolean value defining whether warning notifications should be sent via email.
WarningsTo | email@email.com | Email address where warnings are sent if `EmailAlertForWarnings` is set to True.
WarningsCc | email@email.com | Email address where warnings are sent via CC if `EmailAlertForWarnings` is set to True.
ScoringEnvironment | TIS | Must match the value in `OSS_TestScoringConfigs.dbo.ComputationLocations` table which is TIS by default.
ClientName | SBAC | The client name of the tests TIS is processing.  For IRP packages this would be "SPAC_PT."  When tests are loaded, the client name from the test is used to populate the `OSS_COnfigs.dbo.Client_TestMode`.  If you are not sure what to set this value to check this table after loading up your tests.
EventLogName | OSS_TISEventLog | The name of the Windows Event Log that will be created and used for warnings and errors.  NOTE: The user that this service is running as will need to have access to create and write to the Event Log.  By default the Local System should have access.
EventLogSource | OSS_TIS | The source name used in the Event Log.
ErrorLog | C:\logs\OSS\_TIS_ResultLog.txt | Path to a log file used by the service.  Make sure the user running the service has write access to this path.
SendToHandscoring | True | Boolean value that controls whether items are sent to the Teacher Handscoring System or not.
IgnoreHandscoringDuplicates | True | Should duplicates be ignored when processing handscoring results.
Environment | Production | Set to Production when deployed live.  If set to "Local" or "Dev" then it allows for the TDS server to not be validated.  See `TDSSessionDatabases` below.
IdleSleepTime | 1000 | The amount of time (in milliseconds) to sleep when there is no work to be done.
LoopSleepTime | 10 | The amount of time the thread sleeps (in milliseconds) at each iteration of the loop
NumberOfGeneralThreads | 20 | The total number of threads used for all subjects, except writing.
WaitForWorkerThreadsOnStopSeconds | 120 | When the service is stopped, the system will wait this long for all worker threads to complete.  Defaults to 120 seconds.
LongDbCommandTimeout | 90 | Database command timeout in seconds
TDSSessionDatabases | tds-web01,session;tds-web02,session | List of one or more TDS applications that are allowed to send test results to this TIS intance. The format is `{server},{database};{server},{database};`  The database should almost always be set to **"session"** and the `{server}` should be set to the machine name of the TDS server.  TIS validates that the data coming in is from one of those servers by looking at the TRT file `<Opportunity>` `server` and `database` values.   **IMPORTANT:** If `Environment` is starts with "Dev" or "Local" then this validation is skipped.

**system.net/mailSettings/smtp**  

In order to receive email notifications, the SMTP settings must be set appropriately.  For more information please refer to the MSDN Microsoft page here: <https://msdn.microsoft.com/en-us/library/ms164240(v=vs.110).aspx>


### TIS Scoring Daemon

#### TISScoringDaemon\TIS.ScoringDaemon.Web.UI\Web.config

**machineKey**  

For security reasons, the `validationKey` and `decryptionKey` values should be changed when TIS is deployed.  This can easily be done from within IIS as described here: <https://blogs.msdn.microsoft.com/amb/2012/07/31/easiest-way-to-generate-machinekey/>.

#### TISScoringDaemon\TIS.ScoringDaemon.Web.UI\Configuration\database.config

**connectionStrings**  

There is a single connection string to configure that point to the `OSS_TIS` databases.

Name		| Example Value	| Description
:-------- | :--- | :-----------
GEO:Cloud		| Data Source=`[DATABASE IP OR URL]`;Initial Catalog=OSS_TIS;User id=`[USERNAME]`;Password=`[PASSWORD]` | Use any valid SQL Server connecion string that connects to the OSS_TIS database.

#### TISScoringDaemon\TIS.ScoringDaemon.Web.UI\Configuration\logging.config

You will need to update the `filePath` attribute of the `<sharedListeners><add name="fileTrace">` element to point to an appropriate logging directory.  NOTE: This is actually a directory and not a full file path.

If necessary, the logging levels can be set in the `<switches>` element.

#### TISScoringDaemon\TIS.ScoringDaemon.Web.UI\Configuration\settings.config

**appSettings**  

Name          	| Example Value   	| Description
:------------- 	| :------------- 	| :-------
ScoringDaemon.HubTimerIntervalSeconds | 90 | Time in seconds that the daemon waits before checking for data to process.
ScoringDaemon.MachineName  | MyTISScoringDaemon	| The machine name.  If this is not set, the machine name will be retrieved and used (using C# `Environment.MachineName`. 
ScoringDaemon.PendingMins | 15 | Determines the number of minutes since the last attempt at scoring when finding new items to rescore.  Defaults to 15 minutes if no value is provided.  Used for machine scoring, and not relevant for hand scored items.
ScoringDaemon.MinAttempts | 0 | The minimum number of attempts at rescoring machine scored items.  Defaults to 0 if no value is provided.
ScoringDaemon.MaxAttempts | 10 | The maximum number of attempts at rescoring machine scored items.  If scoring attempts is above this value, the item is marked with a status of `ScoringError`.  Defaults to 10 if no value is provided.
ScoringDaemon.SessionType | 0 | Defaults to 0 if not provided.  0 means online.
ScoringDaemon.MaxItemsReturned | 500 | The maximum number of tests to retrieve at a time when looking for pending items that need to be scored.
ScoringDaemon.ItemScoringConfigCacheSeconds | 14400 | Seconds to cache the item scoring configuration.  Defaults to 14400 (4 hours).
ScoringDaemon.ItemScoringCallBackHostUrl | https://`[TIS_SCORING_DAEMON_URL]`/ | The URL of the TIS Scoring Daemon (which is this website).  **IMPORTANT:** The URL must end with a /
ScoringDaemon.ItemFormatsRequiringItemKeysForRubric | EQ,GI | Defines the item types that need to have their rubrics retrieved from the student application. Defaults to "ER" if not provided.
ScoringDaemon.StudentAppUrlOverride | https://`[STUDENT_APP_URL]`/ | Allows the student application to be overriden instead of using the value sent in the TRT.  This setting is optional.
ScoringDaemon.ItemScoringServerUrlOverride | https://`[URL]`/ | Allows the item scoring server URL to be overriden.  This setting is optional.
ScoringDaemon.EnableLocalHostUsageForColocatedApps | False | Allows the use of localhost instead of a specific URL when the student application and the item scoring are colocated on the same server.  Defaults to False.

## Database

### OSS_TIS.dbo.TestNameLookUp

In order for TIS to score a new test that has been loaded into the system, you must manually add the test into the `TestNameLookUp` table.  

**IMPORTANT:** The instance name must match what was set in the TIS Server AppSettings for `ServiceName`.  By default, this should be kept as `OSS_TISService`.

### OSS_TIS.dbo.CombinationTestMap, OSS_TIS.dbo.CombinationTestFormMap

For summative tests, TIS combines the Performance Task with the Computer Adaptive Test (CAT) into a Combined Test when performing the test scoring.  The test and segment mapping that allow this combination to happen are defined in these two tables.

For new tests loaded into the system, these tables will need to be populated accordingly.

### OSS_Configs.dbo.Client_TestTool, OSS_Configs.dbo.Client_TestToolType

These tables are similar to the TDS configs.client_testtool and configs.client_testtooltype tables.  They hold the available accommodations and need to be added for each test.  The Context column is used to make it available for a particular test or for all tests by using "*".  This comes pre-populated with example data from 2014-15 that can be used as a guide for each new test loaded into the system.

### OSS_TIS.dbo.QC\_ProjectMetaData

This table holds settings and meta-data for projects in the system.  A project is a way to group tests that have the same processing rules.  Generally the grouping rules are by test name and status.

Metadata values can include the following:

Group Name | Variable Name | Notes
:-----     | :------       | :----
HandscoringTSS | Target | "1" means it will send items to the Teacher Handscoring System.  "0" means it will not.
HandscoringTSS | XMLVersion | "OSS" is the only value used in the open source system and stands for "Open Source System"
HandscoringTSS | Transform |
HandscoringTSS | IncludeAllDemographics | "1" means that all student demographic data will be sent.  If set to "0" then it will get data from the `OSS_TIS.dbo.RTSAttributes` table
HandscoringTSS | TargetType |
HandscoringTSS | Order | Integer representing the order that these will be processed.  This value is compared to the HandScoringTDSUnscored value only.
HandscoringTDSUnscored | Target | "1" means it will send items that were not scored back to the Item Scoring engine on TDS.  "0" means it will not.
HandscoringTDSUnscored | Order | Integer representing the order that these will be processed.  This value is compared to the HandScoringTSS value only.
QA | MergeIetmScores |
QA | ScoreInvalidations |
QA | UpdateAppealStatus |
QA | AutoAppealTrigger |
QA | IgnoreWrongServer | If this is set to "1" (meaning true), then TIS will not validate the server sending the results is in th approved whitelist.
All | Accommodations |
DW1 | Target |
DW1 | XMLVersion |
DW1 | Transform |
DW1 | IncludeAllDemographics | "1" means that all student demographic data will be sent.  If set to "0" then it will get data from the `OSS_TIS.dbo.RTSAttributes` table
DW1 | TargetType |
DW1 | Order |
DW2 | Target |
DW2 | XMLVersion |
DW2 | Transform |
DW2 | IncludeAllDemographics | "1" means that all student demographic data will be sent.  If set to "0" then it will get data from the `OSS_TIS.dbo.RTSAttributes` table
DW2 | TargetType |
DW2 | Order |