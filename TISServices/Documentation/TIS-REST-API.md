# Test Integration System (TIS) API Guide


## Purpose

This document is intended to help integrate TDS solutions more easily with TIS. Each of TIS's endpoints is detailed, describing its purpose and its request format.


## TIS Overview

The TIS is responsible for:



*   Receiving a test result from TDS (Test Delivery System)
*   Sending it to THSS (Teacher Hand Scoring System) for hand scoring of items that require human scoring
*   Receiving item scores back from THSS
*   Inserting item scores into the file received from TDS
*   Scoring the test
*   Sending the scored test to downstream systems via SFTP


## TIS Components

The TIS consists of the following 3 modules/parts:


### TDS Receiver

This is a REST endpoint that receives test results in an XML format from the Test Delivery System. Each result received is inserted into a database where it is picked up and processed by the TIS Service.


### TIS Service

This is a Windows service that continuously looks for new test results in the database that have not yet been processed. Once it finds these, it picks them up and processes them (either by sending to THSS, inserting scores from THSS, scoring the test or sending the test downstream).


### TIS Scoring Daemon

This is a web application that talks to the THSS (Teacher HandScoring system) and is responsible for receiving item scores from THSS and sending it to the TIS Service for further processing.


## TIS Operational Overview

A TRT is submitted by TDS. It may or may not have individual items scored in that TRT.

Several use cases:



*   tests completely scored
*   tests partially scored
*   multiple (usually two) assessments that need scoring

TDS sent TRT to TIS - some or all items have scores. TIS' 'TDS Receiver' webapp writes into DB as a row into xml repository table. Column called 'location' - when from TIS, it's set to 'source'. This represents the initial state of TRT processing, before TIS changes it.

The TIS windows service takes the 'source' TRT from the xmlrepository DB and validates it, which can change the status to 'reject' and log into several places, including the filesystem and the QC_ValidationException table.

If valid and all items are scored for a single-assessment exam, it creates a new row with location 'destination', setting the location of the old row from 'source' to 'archive' to store the original TRT. The destination TRT will have an aggregate score added that is calculated by TIS. For a combined assessment with multiple TRT's, TIS will wait for all the TRT's to arrive before creating the destination row.

If valid but some items need to be scored, it changes the location to 'destination'.

TIS adds <Score> elements to TRT associated to the exam/opportunity level as opposed to the item level.

Once all items are scored and saved into the TRT, TIS will add a new row to the DB with location 'destination', adding all the scores provided by THSS for each individual item, and the aggregate score for all items at the assessment level.Once a TRT is fully scored, the TRT is sent to RDW for storage and analysis.


### Interaction with THSS

TIS leans on THSS to provide the scores for non-machine-scorable items. TIS looks through the destination TRT for unscored items and sends those items with the student's responses to THSS where they will sit until a teacher manually scores them. As a teacher scores responses, they are individually sent back to the TIS scoring daemon's API, and the scores are saved into the destination TRT.


## REST endpoints and communication with external systems

The Test Integration System is built to communicate with all the peer and down-stream systems using a secured REST APIs (using OAuth). The token for secured communication would be supported/provided by the OpenAM system.


## TIS Receiver API Endpoints


### **POST /api/testresult:** Accept incoming TRT files

Receive a TDSReport from TDS.  A callback URL is provided (url encoded) which will be saved with the file and used to send a acknowledgement when TIS has processed the file.


#### Request Format:

**URL**: http://<hostname>/api/testresult?statusCallback={statusCallbackUriEncoded}

**Content-Type**: application/xml

**Method**: POST

**Body**: xml formatted TRT / TDSReport

The TRT format is documented at [http://www.smarterapp.org/documents/TestResultsTransmissionFormat.pdf]()

#####Example:

```
<TDSReport>
           <Test name="(SBAC_PT)SBAC-Perf-MATH-11-Spring-2013-2015" subject="MATH" testID="SBAC-Perf-MATH-11" airbank="1" handscoreproject="" contract="SBAC_PT" m  ode="online" grade="11" assessmentType="" academicYear="" assessmentVersion="5644" />
          <Examinee airkey="8" >
              <ExamineeAttribute context = "INITIAL" name = "Birthdate" value = "01012000" contextDate = "2014-11-07 16:12:19.432" />
          ...
      </TDSReport>
```



#### Returns

An HTTP status code and a message (optional) as the content if there's an error.


#### Codes:



*   200: OK
*   400: request is not formatted correctly or does not contain the expected data.
*   500: an unhandled exception occurred while attempting to insert the file into the database


#### Processing

If the file is accepted, it will be submitted to TIS for processing.

Once TIS has processed the file, it will send acknowledgement message to a REST endpoint exposed by TDS at the address specified in the initial request's statusCallback parameter value.  The acknowledgement will indicate whether TIS succeeded or failed in processing the test file. If it failed, an error message will be returned.

TIS will log any errors that may occur while attempting to callback to TDS with an acknowledgement and members of the TIS development team can alert TDS as to issues. TDS may also notice that test opps have not changed status as expected and contact the TIS team to investigate.

Once any issues are resolved, tests can be resubmitted within TIS in order to resend the acknowledgement.  TIS will log exceptions on 500 errors, but will not attempt to retry. Once any issues are resolved, the file will need to be resubmitted from TDS.


### Reply from TIS to TDS


#### Request Format:

**URL**: [statusCallbackURI]

**Content-Type**: application/json

**Method**: POST

**Body**: json formatted


```
    {"oppKey" : "AA19A641-C782-4426-8586-E7D77EDF2D02", "success" : false, "error" : "too late, student left the building"}
```
* **'success'** element is boolean.
* **'error'** element is optional.


##### Example:

http://1.2.3.4:8080/student/TIS/API/tisReply


#### Returns (from TDS)

An HTTP status code and a message (optional) as the content if there's an error.


#### Codes:



*   200: OK
*   400: request is not formatted correctly or does not contain the expected data.


#### Processing

The TIS reply is processed by the ERT service in TDS. If TIS Reply parameters passed validation, TDS tries to find record for given opportunity key in testopportunity table.

If not found, TDS places a record in its application log and finishes processing. Otherwise, if success parameter is set to false, TDS inserts record detailed into opportunityaudit table and finishes processing.

If success parameter is set to true, TDS checks if opportunity status is 'expired' or is not 'submitted', record to that regard is inserted into opportunityaudit table and processing is finished. Otherwise, test opportinuty status in testopporunity table is set to 'reported' and record is inserted into opportunityaudit table. 

Any exceptions while processing validated TIS Reply will be logged into Student server log, but not reported to TIS server.


### **POST /api/testpackage**: Load test package into TIS

This endpoint loads an administrative test package into TIS. This endpoint is accessed by the support tool loader, and does not need to be accessible to the rest of TDS.


#### Request Format:

**URL**: http://<hostname>/api/testresult?statusCallback={statusCallbackUriEncoded}

**Content-Type**: application/xml

**Method**: POST

**Body**: xml formatted test package

* see [https://github.com/SmarterApp/TDS_SupportTool/blob/develop/client/src/main/resources/xsd/v4-test-package.xsd]()


#### Returns

An HTTP status code and a message (optional) as the content if there's an error.


#### Codes:



*   201: CREATED
*   400: request is not formatted correctly or does not contain the expected data.
*   500: an unhandled exception occurred while attempting to load the package


#### Processing

If the file is accepted TRT's can be received and processed for the assessment. 


### **DELETE /api/assessments:** Delete assessment from TIS

This endpoint removes an assessment from TIS.  This endpoint is accessed by the support tool loader, and does not need to be accessible to the rest of TDS.


#### Request Format:

**URL**: http://<hostname>/api/assessments/[assessmentId]

**Content-Type**: application/xml

**Method**: POST

**Body**: xml formatted test package


##### Example:

http://1.2.3.4:8080/api/assessments/(SBAC_PT)MSB-Multiform-Mathematics-3


#### Returns

An HTTP status code and a message (optional) as the content if there's an error.


#### Codes:



*   200: OK if the delete was successful
*   400: Bad Request if the key is null or empty
*   500: Internal Server Error if the delete operation failed.


## TIS Scoring Daemon API Endpoints

The TIS Service makes requests to THSS for item scores. In response to an ItemScoreRequest, THSS will POST an ItemScoreResponse to the provided callback in the TIS Scoring Daemon.


### ItemScoreRequest

This request is sent from TIS to THSS to request manual scoring on items. The student responses are included verbatim in the document so the grading can be completed. THSS will store these items and present them to the teacher for scoring.


#### Request Format:

**URL**: [thss uri]/api/test/submit

**Content-Type**: application/xml

**Method**: POST

**Body**: xml formatted


```
    <ItemScoreRequest callbackUrl="http://localhost/ItemScoreClient/Scored.axd">
      <ResponseInfo itemIdentifier="I-100-1001" itemFormat="GI">
        <StudentResponse encrypted="true"><![CDATA[<Question></Question>]]></StudentResponse>
        <Rubric type="Uri" cancache="true" encrypted="true">c:\rubrics\rubric.xml</Rubric>
        <ContextToken><![CDATA[xxxxxxx]]></ContextToken>
            <IncomingBindings>
                <Bind name="chosenoption" type="string" value="A"/>
            </IncomingBindings>
            <OutgoingBindings>
                <Binding name="objectsetACount"/>
            </OutgoingBindings>
      </ResponseInfo>
    </ItemScoreRequest>
```



##### Example:

http://thss.example.com:8080/api/test/submit


#### Returns (from THSS)

An HTTP status code and a message (optional) as the content if there's an error.


#### Codes:



*   200: OK
*   400: request is not formatted correctly or does not contain the expected data.


### ItemScoreResponse

When THSS gets a set of items scores from a user's grading session, it will send the scores back to TIS using the callback previously received in the ItemScoreRequest. TIS will record these scores into its DB.


#### **POST /ItemScoringCallbackRcv.axd:** Accept hand-scored items from THSS


#### Request Format:

**URL**: [callbackURL from ItemScoreRequest]

**Content-Type**: application/xml

**Method**: POST

**Body**: xml formatted ItemScoreResponse


```
    <ItemScoreResponse>
        <Score>
            <ScoreInfo scorePoint="0" maxScore="2" scoreDimension="overall" confLevel="23.5,45.6" scoreStatus="Scored">
                <ScoreRationale>
                    <Propositions>
                        <Proposition name="Jar" description="The student correctly orders the liquids by density." state="NotAsserted" />
                        <Proposition name="Earth" description="The student correctly ranks the layers of Earth by density." state="NotAsserted" />
                        <Proposition name="Match" description="The student understands that Veg. Oil is the Atmosphere, Dish Soap is the Ocean, Milk is the Crust, and Honey is the Core." state="NotAsserted" />
                    </Propositions>
                    <Bindings />
                    <Message>
                        <![CDATA[Your response earned a score of 0. Full credit requires: BothOrder]]>
                    </Message>
                </ScoreRationale>
                <SubScoreList>
                    <!-- a list of children ScoreInfo instances -->
                </SubScoreList>
            </ScoreInfo>
            <ContextToken>
                <![CDATA[ This token is what we received with the score request ]]>
            </ContextToken>
            <ScoreLatency>10</ScoreLatency>
        </Score>
    </ItemScoreResponse>
```



##### Example

http://tis-web-deployment.sbtds.org:8080/ItemScoringCallbackRcv.axd


#### Returns

An HTTP status code and a message (optional) as the content if there's an error.


#### Codes:



*   200: OK
*   400: request is not formatted correctly or does not contain the expected data.


#### Processing

The scores in the document are saved into the TIS test response repository.


### Submit scored TRT to Data Warehouse

When an opportunity has completed scoring, the resulting TRT with scores is sent to all configured Data Warehouse web services for long term storage and analysis.

**URL**: configured WebServiceSettings in TISService\App.config under DW1/DW2 WebService nodes

**Content-Type**: application/xml

**Method**: POST (outgoing)

**Body**: xml formatted TRT / TDSReport

The TRT format is documented at [http://www.smarterapp.org/documents/TestResultsTransmissionFormat.pdf]()

