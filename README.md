------------------------------------------------------------------

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

------------------------------------------------------------------

## License ##
This project is licensed under the [AIR Open Source License v1.0](http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf).


------------------------------------------------------------------

## Getting Involved ##
We would be happy to receive feedback on its capabilities, problems, or future enhancements:

* For general questions or discussions, please use the [Forum](http://forum.opentestsystem.org/viewforum.php?f=20).
* Use the **Issues** link to file bugs or enhancement requests.
* Feel free to **Fork** this project and develop your changes!


------------------------------------------------------------------

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
TISServices\TISServices\TISServices.sln 


------------------------------------------------------------------

## Build & Deploy
TIS requires Visual Studios 2012 to build. The Deployment steps are as follows - 

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

* `<root>\TDSQAService\OSS.TIS\SQL\TDSConfigs\1_Create_Objects.sql`
* `<root>\TDSQAService\OSS.TIS\SQL\TDSConfigs\2_Configuration.sql`
 	
	
6) Deploy the `[Db server].OSS_Itembank` database objects by running the following scripts in order:

* `<root>\TDSQAService\OSS.TIS\SQL\TDSItemBank\1_Create_Synonyms_Sproc.sql`
* `<root>\TDSQAService\OSS.TIS\SQL\TDSItemBank\2_Create_Synonyms_Config.sql`  (** this script will require a couple of variables to be set prior to running)
* `<root>\TDSQAService\OSS.TIS\SQL\TDSItemBank\3_Create_Objects.sql`
* `<root>\TDSQAService\OSS.TIS\SQL\TDSItemBank\4_Configuration.sql`
* `<root>\TDSQAService\OSS.TIS\SQL\TDSItemBank\5_LoadPackages.sql`



7) Deploy TISService code at `tis_opentestsystem/` [App server]


8) To deploy the 'TIS Scoring Daemon' - 

- Create a web application on the App server at `/oss_tis_itemscoring'
- Publish the 'TIS Scoring Daemon' applicaton to `/oss_tis_itemscoring`


9) To deploy the 'TIS services REST endpoint' - 

- Create a web application on the App server at `/oss_tisservices'
- Publish the 'REST endpoint applicaton' to `/oss_tisservices`


10) Run InstallUtil for .Net 4.5, 32-bit on `/tis_opentestsystem/TDSQAService.exe` to install the windows service. [App server]

11) Verify that the TIS service has privileges to write to the event log [App server].


------------------------------------------------------------------

## Dependencies
Test Integration System has the following dependencies that are necessary for it to compile and run. 

### Compile Time Dependencies
* .Net Framework 4.5
* Microsoft.Practices.EnterpriseLibrary.Data

### Runtime Dependencies
None

------------------------------------------------------------------

## Items included in this release (01/28) -

####1) Config Updates
##### - [OSS_Configs]
- 2_Configuration.sql 

         - updated to remove extraneous configuration

##### - [OSS_Itembank]
- Scripts broken out to add a couple extra steps to make sure that synonym references can be resolved when creating objects, and to make the loading of packages a separate step
- Configuration script modified to remove some extraneous values
- Create_Objects.sql script updated:
 
         - fixes to loader sprocs
         - an unused view was removed (which had a reference to a missing UDF).
         - objects were reordered to eliminate dependency errors

##### - [OSS_TestScoringConfigs]
- Scripts numbered
- 3_Configuration.sql script added

##### - [OSS_TIS]
- 1_Create_Objects.sql

        - added missing GetWarningsSummary sproc
        - Removed unused view and added a new view

- 2_Configuration.sql 

        - updated config for mapping combo test forms as well as for configuring which student attributes should flow downstream.

- 3_ScoringDaemonConfiguration.sql 

	- Added a new script.

####2)Code Updates 
- ScoringEnvironment in the TIS’s app.config file changed from “QA” to “TIS”.
- Bug in a format string fixed
- Reverted back to original approach for configuring which RTSAttributes should be included in the file for a particular target: only include configured attributes. 

####3)'Build & Deploy' steps have been updated.
-  Steps 3 -- Removed the step to create /tis_common/
- Steps 5 & 7 have been updated with the new scripts (also numbered).
-  Step 8 (previous version) has been removed
- Step 12 (previous version) has been removed
- Steps 8 & 9 have been added.

------------------------------------------------------------------

## Future Enhancements 

The following features and tasks are not included in the 1/28/2015 release:

####1) Issue with the loader Script (Build & Deploy Step 6.5) - 
The Loader script is currently not loading all accommodations. It is currently only loading the following accommodations - `Language` and `Print Size`. We intend to fix this bug ASAP and have the fix released by 01/30/2015.

####1) System and Integration Testing - 

The Test Integration System has not undergone a complete system testing or integration testing with the Test Delivery System, Teacher Hand Scoring System, and Data Warehouse.  We intend to complete the System and integration testing in an upcoming release.


####2) README Documentation - 
This README documentation is not complete.  It will be updated with additional detail by the 01/30/2015 release.

------------------------------------------------------------------