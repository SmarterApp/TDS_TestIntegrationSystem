# Welcome to the Test Integration System (TIS)

The TIS is responsible for 

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

### TIS Service:
This is a Windows service that continuously looks for new test results in the database that have not yet been processed.  Once it finds these, it picks them up and processes them (either by sending to THSS, inserting scores from THSS, scoring the test or sending the test downstream). 
As part of the deployment of this application, we need to set up the database and deploy the code in app server. A Windows service needs to be installed and started for this app.
The TIS Service solution can be found here:  TDSQAService\OSSTIS.sln.

### TIS Scoring Daemon      
This is a web application that talks to the THSS (Teacher HandScoring system) and is responsible for receiving item scores from THSS and sending it to the TIS Service for further processing.  The TIS Scoring Daemon solution can be found here:  TISScoringDaemon\TISScoringDaemon.sln.

### REST EndPoint communication with [TDS, ART & Data warehouse]
The Test Integration System is built to communicate with all the peer and down-stream systems using a secured REST APIs (using OAuth). The token for secured communiation would be supported/provided by the OpenAM system.


## Build & Deploy
TIS requires Visual Studios 2012 to build. The Deployment steps are as follows - 

1) Create the following databases [DB Server]:

* `OSS_QC`
* `OSS_Itembank`
* `OSS_Configs`
* `OSS_TestScoringConfigs`

Create an App User account and grant that account dbo access on the above DBs. [DB server]

2) Create these folders on the application server (if they don't already exist): [Web server]

* `/Services/tis_opentestsystem`
* `/oss_tis_itemscoring`
* `/oss_tisservices`

3) Create the following subfolder structure within the Services folder created above (if they do not exist): [Web server]
/Services/:

* `/tis_common/`
* `/tis_opentestsystem/Service`

4) Deploy the `[Db server].OSS_QC` database objects by running the following scripts in order: [DB server]

* `<root>\OSS.TIS\SQL\TISDB\1_Create_Objects.sql`
* `<root>\OSS.TIS\SQL\TISDB\2_Configuration.sql`

5) Deploy the [Db server].OSS_Configs database objects by running the following scripts in order:

* `<root>\OSS.TIS\SQL\TDSConfigs\1_Create_Objects.sql`
* `<root>\OSS.TIS\SQL\TDSConfigs\2_Configuration.sql`
 	
6) Deploy the [Db server].OSS_TestScoringConfigs database objects by running the following scripts in order:

* `<root>\OSS.TIS\SQL\TDSConfigs\Tables.sql`
* `<root>\OSS.TIS\SQL\TDSConfigs\Views.sql`
* `<root>\OSS.TIS\SQL\TDSConfigs\StoredProcedures.sql`
	
7) Deploy the [Db server].OSS_Itembank database objects by running the following scripts in order:

* `<root>\OSS.TIS\SQL\TDSConfigs\1_Create_Objects.sql`
* `<root>\OSS.TIS\SQL\TDSConfigs\2_Configuration.sql`

8) Load the test Package by running the Stored Procedure `[tp].[spLoader_Main]`

9) Run QA configuration script: at `<root>\OSS.TIS\SQL\TISDB\Deployment\QACoreConfiguration.sql` [Db server]

10) Deploy TISService code at `tis_opentestsystem/Service` [Web server]

11) Run InstallUtil for .Net 4.5, 32-bit on `_tis_opentestsystem/Service/TDSQAService.exe` [Web server]

12) Verify that the QA system installed in the previous step has access to execute `/_Services/tis_common/WinSCP/WinSCP.exe`. [Web server]

13) Verify that the QA service has privileges to write to the event log and to the `/_Services/tis_opentestsystem_org/` dir and subdirs. [Web server]

## Dependencies
Test Integration System has the following dependencies that are necessary for it to compile and run. 

### Compile Time Dependencies
* .Net Framework 4.5
* Microsoft.Practices.EnterpriseLibrary.Data

### Runtime Dependencies
None

## Items included in this release (01/16) -
1) DB scripts to create and configure the following databases - 

         - OSS_QC
         - OSS_Itembank
         - OSS_Configs
         - OSS_TestScoringConfigs

2) Code Update to add the following items [Listed as Future Enhancements in the last release]- 

         - Fetching initial accommodations 
         - Secure the REST endpoints

3) Code update to the SendTo Data-warehouse module to use secured REST End-Point API (instead of SFTP). 

4) Sample Config files (Web-Config & App-Config) 


## Future Enhancements 

The following features and tasks are not included in the 1/16/2015 release:

####1) System and Integration Testing - 

The Test Integration System has not undergone a complete system testing or integration testing with the Test Delivery System, Teacher Hand Scoring System, and Data Warehouse.  System and integration testing will be complete (with the features identified above) as of the 01/31/2015 release.

####2) ItemID and BankID mismatch across systems (known issue) - 
ItemIDs and BankIDs are currently auto generated when loading the Admin package, which is causing an issue between TDS and TIS.
We are working deriving these values from the Keys in the Test package, so that they are always in sync across all systems. We intend to fix this bug ASAP and have the fix released by 01/31/2015.


####3) README Documentation - 
This README documentation is not complete.  It will be updated with additional detail by the 01/31/2015 release.