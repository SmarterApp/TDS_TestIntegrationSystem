# Welcome to the Test Integration System #
The Test Integration System is responsible for 

* Receiving a test result from TDS (Test Delivery System) 
* Sending it to THSS (Teacher HandScoring System) for hand scoring of items that require human scoring
* Receiving item scores back from THSS
* Inserting item scores into the file received from TDS
* Scoring the test
* Sending the scored test to downstream systems via SFTP 

The TIS (Test Integration System) is broken down into the following 3 modules/parts  
#####1.) TDS Receiver
#####2.) TIS Service 
#####3.) TIS Scoring Daemon      

   
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

## Build & Deploy
TIS requires Visual Studios 2012 to build. The Deployment steps are as follows - 

![2386678942-DeploymnetSteps_Draft-0.png](https://bitbucket.org/repo/AyMdK5/images/2766033395-2386678942-DeploymnetSteps_Draft-0.png)
![1333802654-DeploymnetSteps_Draft-1.png](https://bitbucket.org/repo/AyMdK5/images/3023217018-1333802654-DeploymnetSteps_Draft-1.png)
![3713494634-DeploymnetSteps_Draft-2.png](https://bitbucket.org/repo/AyMdK5/images/3357799014-3713494634-DeploymnetSteps_Draft-2.png)
## Dependencies
Test Integration System has the following dependencies that are necessary for it to compile and run. 

### Compile Time Dependencies
* .Net Framework 4.5
* Microsoft.Practices.EnterpriseLibrary.Data
* WinSCPNet

### Runtime Dependencies
* WinSCPNet

NOTE:  WinSCPNet is not included in the released source code.  It must be acquired and added as a third party library in order for all 3 projects to compile and run.
Following are instructions to add this third party library:
####1) Download the winscp552automation.zip package from http://sourceforge.net/projects/winscp/files/WinSCP/5.5.2/winscp552automation.zip/download.
####2) Copy the WinSCPnet.dll file from the downloaded winscp552automation.zip archive to “\Common\DataAccess\Libraries”.
####3) Download the winscp552.zip package from http://sourceforge.net/projects/winscp/files/WinSCP/5.5.2/winscp552.zip/download.
####4) Copy the WinSCP.com and WinSCP.exe files from the downloaded winscp552.zip archive to “\Common\DataAccess\Libraries”.
####5) Compile the applications.

## Future Enhancements 
The following features and tasks are not included in the 1/2/2015 release:

###1) Fetching initial accommodations
The TIS will be updated to fetch the initial list of accommodations for a student (from the ART system) so that the Scoring Engine can calculate the accommodation codes needed by the open source data warehouse and reporting system.  This is targeted to be in the 01/31/2015 release.

###2) Secure the REST endpoints
Currently the TDS Receiver does not allow for secured communication between other systems.  We will be updating this module (TDS Receiver) using OAuth to make sure all the communication uses secure authorization.  This is targeted to be in the 01/31/2015 release.

###3) System and Integration Testing
The Test Integration System has not undergone system testing or integration testing with the Test Delivery System, Teacher Hand Scoring System, and Data Warehouse.  System and integration testing will be complete (with the features identified above) as of the 01/31/2015 release.

###4) README Documentation
This README documentation is not complete.  It will be updated with additional detail in the 01/31/2015 release.