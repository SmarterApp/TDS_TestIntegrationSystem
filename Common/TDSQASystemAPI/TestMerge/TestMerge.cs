/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using ScoringEngine;
using ScoringEngine.ConfiguredTests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.Data;
using TDSQASystemAPI.TestResults;

namespace TDSQASystemAPI.TestMerge
{
    /// <summary>
    /// Merge a test result with related test results using merge configuration
    /// </summary>
    public class TestMerge
    {
        /// <summary>
        /// Constructor - Called from TestMergeConfiguration to initialize with read only config applicable for this merge
        /// </summary>
        /// <param name="mergeConfig"></param>
        public TestMerge(MergeConfig mergeConfig)
        {
            _mergeConfig = mergeConfig;
        }

        /// <summary>
        /// Merge the current opportunity test result
        /// </summary>
        /// <param name="qaSystemConfigSettings"></param>
        /// <param name="tc"></param>
        /// <param name="currentTestResult"></param>
        public void CreateCombinedTest(TestCollection tc, TestResult currentTestResult, ITestResultSerializerFactory serializerFactory)
        {
            try
            {
                // PreMerge - Validation - If the current test is in 'handscoring' or 'appeal' status - Nothing to do, return 
                //  AM: added handscored status, which is an intermediate status that is logged when all hand-scorable items
                //  are scored but prior to the file being re-processed by QA with the scored items.
                if (currentTestResult.Opportunity.Status.Equals("handscoring")
                    || currentTestResult.Opportunity.Status.Equals("appeal")
                    || currentTestResult.Opportunity.Status.Equals("handscored"))
                    return;

                BL.XmlRepository xmlRepo = new BL.XmlRepository(QASystemConfigSettings.Instance.LongDbCommandTimeout);

                // Gather all the opportunities for merge,
                // If null is returned then the current opp is in 'reset' and there exists some other opp in the past, so combo would have been already created
                Dictionary<string, RelatedTestOpportunity> RelatedXMLFileInfo = GetOppsForMerge(currentTestResult);
                if (RelatedXMLFileInfo == null)
                    return;
                
                // Gather all the test results and statuses for merge participants 
                Dictionary<string, TestResult> sourceTestNameToResultsMap = new Dictionary<string, TestResult>();
                Dictionary<string, string> sourceTestNameToStatusesMap = new Dictionary<string, string>();

                // Add the current one 
                sourceTestNameToResultsMap.Add(currentTestResult.Name, currentTestResult);
                sourceTestNameToStatusesMap.Add(currentTestResult.Name, currentTestResult.Opportunity.Status);

                // Get all the test results for merge based on config.
                foreach (var configuredSourceTestName in _mergeConfig.SourceTestNamesForMerge)
                {
                    // Check, if we have the file ID for the test
                    if (RelatedXMLFileInfo.ContainsKey(configuredSourceTestName))
                    {
                        RelatedTestOpportunity  relatedTestOpportunity = RelatedXMLFileInfo[configuredSourceTestName];
                        // Check, if it is already added 
                        if (!sourceTestNameToResultsMap.ContainsKey(configuredSourceTestName))
                        {
                            if (relatedTestOpportunity.LatestStatus.Equals("handscoring")
                                || relatedTestOpportunity.LatestStatus.Equals("appeal")
                                || relatedTestOpportunity.LatestStatus.Equals("handscored")) //AM: added handscored
                                continue;

                            TestResult tr = CreateTestResult(tc, xmlRepo, relatedTestOpportunity.LatestFileId, serializerFactory);
                            sourceTestNameToResultsMap.Add(configuredSourceTestName, tr);
                            sourceTestNameToStatusesMap.Add(configuredSourceTestName, tr.Opportunity.Status);
                        }
                        
                        // If the test is in 'scored' then previosly it is 'handscored' or 'appealed' - get the status prior to that to decide overall status
                        string currentStatus = sourceTestNameToStatusesMap[configuredSourceTestName];
                        if (currentStatus.Equals("scored"))
                        {
                            if ((relatedTestOpportunity.LatestStatus.Equals("handscoring")
                                || relatedTestOpportunity.LatestStatus.Equals("scored"))
                                && relatedTestOpportunity.HandScoredFileId != -1)
                            {
                                TestResult tr = CreateTestResult(tc, xmlRepo, relatedTestOpportunity.HandScoredFileId, serializerFactory);
                                sourceTestNameToStatusesMap[configuredSourceTestName] = tr.Opportunity.Status;
                            }
                            else if ((relatedTestOpportunity.LatestStatus.Equals("appeal") 
                                      || relatedTestOpportunity.LatestStatus.Equals("scored"))
                                      && relatedTestOpportunity.PreAppealFileId != -1)
                            {
                                TestResult tr = CreateTestResult(tc, xmlRepo, relatedTestOpportunity.PreAppealFileId, serializerFactory);
                                sourceTestNameToStatusesMap[configuredSourceTestName] = tr.Opportunity.Status;
                            }
                        }
                    }
                }

                // Validate, Filter and Sort the test results 
                List<TestResult> sourceTestResults = ValidateAndSortTestResults(sourceTestNameToResultsMap.Values.ToList());

                // Merge, only if we have atleast one test for merging
                if (sourceTestResults.Count > 0)
                {
                    // Get the overall status 
                    string sTargetTestStatus = GetMergedTestStatus(sourceTestNameToStatusesMap.Values.ToList());
                    
                    // Merge the test result
                    TestResult mergedResult = MergeTestResults(tc, sourceTestResults, sTargetTestStatus);
                    if (mergedResult != null)
                    {
                        // Need the QA project of the combo test in order for the serlializer to fetch the correct config.
                        //  For now, set the QA projectID using the OSS loader, which is all we currently need for combos
                        //  TODO:
                        mergedResult.SetProject(new Config.ProjectMetaDataLoader(mergedResult.HandscoringProjectID, mergedResult.test.TestName, mergedResult.Opportunity.Status, null));
                        xmlRepo.InsertXml(BL.XmlRepository.Location.source, mergedResult, serializerFactory);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex; // Rethrow the exception, QA system will handle it.
            }
        }

        /// <summary>
        /// Gather opportunities for merge
        /// </summary>
        /// <param name="currentTestResult"></param>
        /// <returns></returns>
        private Dictionary<string, RelatedTestOpportunity> GetOppsForMerge(TestResult currentTestResult)
        {
            // Return null, if merge cannot be done, otherwise return chosen opportunties for merge 
            Dictionary<string, RelatedTestOpportunity> RelatedXMLFileInfo = new Dictionary<string, RelatedTestOpportunity>();
            
            // Get the current test information 
            String sCurrentTestID = currentTestResult.TestID;
            int nCurrentOpportunityNumber = currentTestResult.Opportunity.OpportunityNumber;
            String sCurrentStatus = currentTestResult.Opportunity.Status;

            // Conditions to see if we can skip merging:
            // Skip merging if the current opp is in 'reset' status and there exists another
            // opp in the past for the same testid and opportunitynumber 
            bool bCheckForMergeSkipOnReset = sCurrentStatus.CompareTo("reset") == 0;
            bool bPastExistsForCurrentResetOpp = false;

            // Get all the related opportunities from the database 
            List<RelatedTestOpportunity> listRelatedXMLFileInfo = TestMergeDataAccess.GetLatestXMLFileInfo(currentTestResult.Testee.EntityKey);

            // Enumerate and select the desired opp for merge
            foreach (RelatedTestOpportunity opp in listRelatedXMLFileInfo)
            {
                // We already found past for the current reset opp - Just loop through to exit
                if (bPastExistsForCurrentResetOpp)
                    continue;

                // All of the opportunities selected for merge should have the same opportunity number as the current one
                if (opp.OpportunityNumber != nCurrentOpportunityNumber)
                    continue;

                // If we need to check bCheckForMergeSkipOnReset then see if this one has the same testid as the candidate
                if (bCheckForMergeSkipOnReset && opp.TestId == sCurrentTestID)
                    bPastExistsForCurrentResetOpp = true;

                // Candidate should be the most recent one with same opportunity number
                if (RelatedXMLFileInfo.ContainsKey(opp.TestId))
                {
                    if (RelatedXMLFileInfo[opp.TestId].DateRecorded < opp.DateRecorded)
                        RelatedXMLFileInfo[opp.TestId] = opp; // Replace it with the latest
                }
                else
                    RelatedXMLFileInfo.Add(opp.TestId, opp);
            }

            // Current opp is 'reset' and there exists something in the past 
            // 'nullify' the candidate set
            if (bCheckForMergeSkipOnReset && bPastExistsForCurrentResetOpp)
                RelatedXMLFileInfo = null; 

            // Return the candidates for merge
            return RelatedXMLFileInfo;
        }

        /// <summary>
        /// Get the test result for the given XML file id
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="xmlRepo"></param>
        /// <param name="nFileID"></param>
        /// <returns></returns>
        private TestResult CreateTestResult(TestCollection tc, BL.XmlRepository xmlRepo, long nFileID, ITestResultSerializerFactory serializerFactory)
        {
            XmlDocument testXml = xmlRepo.GetXmlContent(Convert.ToInt64(nFileID));
            if (testXml == null)
                throw new ApplicationException(string.Format("Could not get the xml document for FileID: {0}.", nFileID));

            // create the TestResult instance from the xml file
            XMLAdapter xmlAdaptor = serializerFactory.CreateDeserializer(testXml);
            bool isValid = false;
            TestResult tr = xmlAdaptor.CreateTestResult(tc, out isValid, false);
            if (tr == null || !isValid)
                throw new ApplicationException(string.Format("Could not inflate TestResult for FileID: {0}.", nFileID));

            return tr;
        }

        /// <summary>
        /// Validate and sort the given list of test results for merge
        /// </summary>
        /// <param name="sourceTestResults"></param>
        private List<TestResult> ValidateAndSortTestResults(List<TestResult> sourceTestResults)
        {            
            // Only 'one test' or 'no test at all' met the merge criteria trivially
            int nCount = sourceTestResults.Count;
            if (nCount < 2)
                return sourceTestResults;

            // Select the first as a candidate for comparison 
            TestResult candidate = sourceTestResults[0];

            // Check if all the tests have the same mode and window id as the candidate 
            bool bValid = sourceTestResults.All(x => x.Mode.Equals(candidate.Mode) && x.Opportunity.WindowID.Equals(candidate.Opportunity.WindowID));

            // If the conditions are not met, clear the list so that the caller can skip processing further
            if (!bValid)
                sourceTestResults.Clear();

            // Return the validated tests
            return sourceTestResults;
        }

        /// <summary>
        /// Merge the givent list of test results
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="sourceTestResults"></param>
        /// <param name="sTargetOpportunityStatus"></param>
        /// <returns></returns>
        private TestResult MergeTestResults(TestCollection tc, List<TestResult> sourceTestResults, string sTargetOpportunityStatus)
        {
            // Remove all the tests which has status as reset when the overall thing is not reset
            if (!sTargetOpportunityStatus.Equals("reset"))
                sourceTestResults.RemoveAll(x => x.Opportunity.Status.Equals("reset"));

            // Nothing to do.
            if (sourceTestResults.Count <= 0)
                return null;

            // Get reference test result to gather common attributes 
            TestResult referenceTestResult = sourceTestResults[0];
            
            // Get blueprint for the combined test
            TestBlueprint targetBlueprint = tc.GetBlueprint(_mergeConfig.TargetTestName);
            if (targetBlueprint == null)
                throw new ApplicationException(string.Format("Test merge: Blueprint not available for the combined test {0}", _mergeConfig.TargetTestName));

            // Create a combined test result 
            TestResult targetTestResult = new TestResult(targetBlueprint, _mergeConfig.TargetTestName, referenceTestResult.ItemBank, referenceTestResult.Contract, referenceTestResult.Mode);

            // Customize the config for the currently selected tests for merge 
            Initialize(targetBlueprint, sourceTestResults, targetTestResult);

            // Set the opportunity for the target 
            SetTargetOpportunity(sourceTestResults, targetTestResult, sTargetOpportunityStatus);

            // Set the target testee 
            SetTargetTestee(sourceTestResults, targetTestResult);

            // Merge item responses to create target responses
            MergeItemResponsesAndSetItemPositionMapping(sourceTestResults, targetTestResult);

            // Merge comments - Item positions are now established 
            MergeComments(sourceTestResults, targetTestResult);

            // Merge Tool usages
            MergeToolUsage(sourceTestResults, targetTestResult);

            return targetTestResult;
        }

        /// <summary>
        /// Initialize the configuration based on current set of information 
        /// </summary>
        /// <param name="targetBlueprint"></param>
        /// <param name="sourceTests"></param>
        /// <param name="targetTest"></param>
        private void Initialize(TestBlueprint targetBlueprint, List<TestResult> sourceTests, TestResult targetTest)
        {
            // We need to establish mappings of the following
            // Source segment name -> Source Test Segment (for available test segments)
            // Source Segment name -> Target Segment (for all configured target segments)
            SourceSegmentNameToTargetSegmentMap.Clear();
            SourceSegmentNameToSourceSegmentMap.Clear();

            // Create/Map source and target TestSegment objects for all configured target segments
            foreach (string configuredSourceTestName in _mergeConfig.SourceTestNamesForMerge)
            {
                // Check and get if the merge component test is available 
                TestResult availableSourceTestResult = sourceTests.Find(x => x.Name.Equals(configuredSourceTestName));

                // Get all the available source segments of the source test
                List<TestSegment> availableSourceTestSegments = availableSourceTestResult != null ?
                                        availableSourceTestResult.Opportunity.Segments.Values.ToList() : new List<TestSegment>();

                // Get all the configured source segment names for the configured test (which may or may not be participating this time)
                List<string> configuredSourceSegmentNames = _mergeConfig.GetSourceSegmentNames(configuredSourceTestName);
                foreach (string configuredSourceSegmentName in configuredSourceSegmentNames)
                {
                    // Check if the segment is available for this particular merge operation 
                    TestSegment availableSourceSegment = availableSourceTestSegments.Find(x => x.ID.Equals(configuredSourceSegmentName));
                    string sourceFormKey = null;
                    if (availableSourceSegment != null)
                    {
                        // Add the available segment to the mapping
                        SourceSegmentNameToSourceSegmentMap.Add(availableSourceSegment.ID, availableSourceSegment);
                        // Get if a form is available for this segment 
                        sourceFormKey = availableSourceSegment.FormKey;
                    }
                    // Get the target segment name and form key as per configuration 
                    string configuredTargetSegmentName = _mergeConfig.GetTargetSegmentName(configuredSourceSegmentName);
                    string configuredTargetFormKey = string.IsNullOrEmpty(sourceFormKey) ? null :_mergeConfig.GetTargetFormKey(sourceFormKey);

                    // Get the target segment from blueprint
                    TestSegment configuredTargetSegment = targetBlueprint.GetTestSegment(configuredTargetSegmentName, configuredTargetFormKey);
                    if (configuredTargetSegment != null)
                        // Add the target segment to configured source to target mapping 
                        SourceSegmentNameToTargetSegmentMap.Add(configuredSourceSegmentName, configuredTargetSegment);
                }
            }

            // Now that segment objects are mapped, 
            // Establish the mapping for Source Test -> Position in the target test
            SourceTestToTargetPositionMap.Clear();

            // Gather the maximum segment position for each of the available source tests
            List<KeyValuePair<string, int>> sourceTestMaxSegmentPositions = new List<KeyValuePair<string, int>>();
            foreach (TestResult testResult in sourceTests)
            {
                int nMaxPosition = GetTargetSegments(testResult.Name).Select(x => x.Position).DefaultIfEmpty(0).Max();
                sourceTestMaxSegmentPositions.Add(new KeyValuePair<string, int>(testResult.Name, nMaxPosition));
            }
            // Sort based on the max position 
            sourceTestMaxSegmentPositions.Sort((x, y) => (x.Value.CompareTo(y.Value)));
            // Re-establish 1 based position for source tests
            int nPosition = 1;
            foreach (var kvp in sourceTestMaxSegmentPositions)
            {
                SourceTestToTargetPositionMap.Add(kvp.Key, nPosition);
                ++nPosition;
            }
        }

        /// <summary>
        /// Set opportunity for the target test 
        /// </summary>
        /// <param name="sourceTestResults"></param>
        /// <param name="targetTestResult"></param>
        /// <param name="sTargetOpportunityStatus"></param>
        private void SetTargetOpportunity(List<TestResult> sourceTestResults, TestResult targetTestResult, string sTargetOpportunityStatus)
        {
            // Get reference test result to gather common attributes 
            TestResult referenceTestResult = sourceTestResults[0];

            // These are summed from all participating tests for merge
            int nItemCount = 0;
            int nPauses = 0;
            int nFieldTestCount = 0;
            int nGracePeriodRestarts = 0; 
            int nAbnormalStarts = 0;
            
            // Record the last TAID, TAName, and SessionID received from TDS for merged test
            string sTAID = "";
            string sTAName = "";
            string sSessionID = "";

            // Mode, ClientName, Window, WindowOppNumber, server, database should be same for all tests - Validated tests
            string sMode = referenceTestResult.Mode;
            string sClientName = referenceTestResult.Opportunity.ClientName;
            string sWindowID = referenceTestResult.Opportunity.WindowID;
            int? nWindowOpportunity = null;
            string sServer = referenceTestResult.Opportunity.ServerName;
            string sDatabase = referenceTestResult.Opportunity.DatabaseName;
            string sQALevel = "";
            
            // The status date of the "whole test" shall be the max(statusDate) 
            DateTime dateTestStatus = DateTime.MinValue;

            // Minimum start date
            DateTime dateTestStarted = DateTime.MaxValue;

            // Start is minimum start, force complete is maximum, QA date is current
            DateTime dateTestOriginalStart = DateTime.MaxValue;
            DateTime dateTestForceCompleted = DateTime.MinValue;
            DateTime dateQA = DateTime.Now;

            DateTime? dateTestCompleted = GetCompletedDateForMergedTest(sourceTestResults, sTargetOpportunityStatus);

            // For all tests for merge
            foreach(TestResult testResult in sourceTestResults)
            {
                // Sum all the counts
                if (sTargetOpportunityStatus.Equals("invalidated") || !testResult.Opportunity.Status.Equals("invalidated"))
                {
                    nItemCount += testResult.Opportunity.ItemCount;
                    nPauses += testResult.Opportunity.Pauses;
                    nFieldTestCount += testResult.Opportunity.FieldTestCount;
                    nGracePeriodRestarts += testResult.Opportunity.GracePeriodRestarts;
                    nAbnormalStarts += testResult.Opportunity.AbnormalStarts;
                }

                // TAID, TAName and Session ID from the last recorded test
                // Set the status date as the maximum of the status dates of the participating tests
                if (testResult.Opportunity.StatusDate > dateTestStatus)
                {
                    sTAID = testResult.Opportunity.TAID;
                    sTAName = testResult.Opportunity.TAName;
                    sSessionID = testResult.Opportunity.SessionID;
                    dateTestStatus = testResult.Opportunity.StatusDate;
                }

                // Set Test start as minimum
                if (testResult.Opportunity.StartDate < dateTestStarted)
                    dateTestStarted = testResult.Opportunity.StartDate;

                // Set test original force complete as maximum
                if (testResult.Opportunity.DateForceCompleted > dateTestForceCompleted)
                    dateTestForceCompleted = testResult.Opportunity.DateForceCompleted;
            }

            int nOpportunityNumber = referenceTestResult.Opportunity.OpportunityNumber; 
            long nOppID = -1;
            Guid key ;
            TestMergeDataAccess.GetCombinedTestOppId(targetTestResult.Name, referenceTestResult.Testee.EntityKey, nOpportunityNumber, out nOppID, out key);
            if (nOppID == -1)
                throw new ApplicationException(string.Format("Test Merge: Opportunity cannot be obtained or created for the combined test {0}", targetTestResult.Name));

            targetTestResult.Opportunity = new Opportunity(nOppID.ToString(), dateTestStarted, nOpportunityNumber, sTargetOpportunityStatus, dateTestStatus, nPauses,
                nItemCount, nFieldTestCount, dateTestCompleted, nGracePeriodRestarts, nAbnormalStarts, dateQA, sServer, sDatabase, key, sTAID,
                sTAName, sSessionID, dateTestForceCompleted, sWindowID, nWindowOpportunity, sQALevel, sMode, sClientName, 0);
                
            AddSegments(targetTestResult);
            MergeAccomodations(sourceTestResults, targetTestResult);
            MergeGenericVariables(sourceTestResults, targetTestResult);
        }

        /// <summary>
        /// Get the status of the merged test based on various conditions
        /// </summary>
        /// <param name="sourceTestResults"></param>
        /// <returns></returns>
        private string GetMergedTestStatus(List<string> sourceTestStatuses)
        {
            bool bAllTestsReceived = _mergeConfig.SourceTestNamesForMerge.Count == sourceTestStatuses.Count;
            bool bAllTestsReset = sourceTestStatuses.All(x => x.Equals("reset"));
            bool bAnyTestsReset = sourceTestStatuses.Exists(x => x.Equals("reset"));
            bool bAnyTestsAppeal = sourceTestStatuses.Exists(x => x.Equals("appeal"));
            bool bAnyTestsHandScoring = sourceTestStatuses.Exists(x => x.Equals("handscoring"));
            bool bAnyTestsHandScored = sourceTestStatuses.Exists(x => x.Equals("handscored"));
            bool bAnyExpiredTests = sourceTestStatuses.Exists(x => x.Equals("expired"));
            bool bMetConditionForInvalidated = sourceTestStatuses.All(x => x.Equals("invalidated") || x.Equals("reset"));
            
            string status = "";
            if (bAllTestsReset)
                status = "reset";
            else if (bAnyExpiredTests)
                status = "expired";
            else if (bMetConditionForInvalidated)
                status = "invalidated";
            else if (!bAllTestsReceived || bAnyTestsAppeal || bAnyTestsHandScoring || bAnyTestsHandScored || bAnyTestsReset)
                status = "pending";
            else if (bAllTestsReceived)
            {
                bool bMetConditionForCompleted = sourceTestStatuses.All(x => x.Equals("completed")
                                                                            || x.Equals("submitted")
                                                                            || x.Equals("reported")
                                                                            || x.Equals("scored")
                                                                            || x.Equals("invalidated"));
                if (bMetConditionForCompleted)
                    status = "completed";
            }            
            return status;
        }

        /// <summary>
        /// Get completed date for the merged test
        /// </summary>
        /// <param name="sourceTestResults"></param>
        /// <param name="mergedTestStatus"></param>
        /// <returns></returns>
        private DateTime? GetCompletedDateForMergedTest(List<TestResult> sourceTestResults, string mergedTestStatus)
        {
            DateTime? completedDate = null;
            bool bHasNullCompletedDate = sourceTestResults.Exists(x=> x.Opportunity.IsCompletedDateValid() == false);
            bool bAllNonNullCompletedDate = sourceTestResults.All(x=> x.Opportunity.IsCompletedDateValid());            
            bool bAnyInvalidatedCompHaveNullCompletedDate = sourceTestResults.Exists(x=> x.Opportunity.Status.Equals("invalidated") && x.Opportunity.IsCompletedDateValid() == false);
            bool bAllInvalidatedCompHaveCompletedDate = sourceTestResults.Where(x=>x.Opportunity.Status.Equals("invalidated")).All(y => y.Opportunity.IsCompletedDateValid());            
            var maxTest = sourceTestResults.Where(x => x.Opportunity.IsCompletedDateValid()).OrderByDescending(x => x.Opportunity.CompletedDate).FirstOrDefault();
            DateTime? maxCompletedDate = maxTest == null? null: maxTest.Opportunity.CompletedDate;

            if ( mergedTestStatus.Equals("pending")
                  || mergedTestStatus.Equals("expired")
                  || (mergedTestStatus.Equals("reset") && bHasNullCompletedDate)
                  || (mergedTestStatus.Equals("invalidated") && bAnyInvalidatedCompHaveNullCompletedDate)) 
                completedDate = null;
            else if ((mergedTestStatus.Equals("reset") && bAllNonNullCompletedDate)
                     ||(mergedTestStatus.Equals("invalidated") && bAllInvalidatedCompHaveCompletedDate))
                completedDate = maxCompletedDate;
            else if (mergedTestStatus.Equals("completed"))
                completedDate = maxCompletedDate;

            return completedDate;
        }

        /// <summary>
        /// Add segments of the target test
        /// </summary>
        /// <param name="targetTestResult"></param>
        private void AddSegments(TestResult targetTestResult)
        {
            List<TestSegment> targetSegments = GetAllConfiguredTargetSegments(); // These are new objects created specifically for this target
            foreach (TestSegment targetTestSegment in targetSegments)
                targetTestResult.Opportunity.AddTestSegment(targetTestSegment);
        }
        
        /// <summary>
        /// Merge accomodations
        /// </summary>
        /// <param name="sourceTestResults"></param>
        /// <param name="targetTestResult"></param>
        private void MergeAccomodations(List<TestResult> sourceTestResults, TestResult targetTestResult)
        {
            // Source test accomodations which has segment 0, value is a dictionary indexed by accomodation code 
            Dictionary<string, Dictionary<string, TestAccomodation>> sourceTestCommonAccomodations = new Dictionary<string, Dictionary<string, TestAccomodation>>();

            // Unique set of segment 0 - accomodation codes
            HashSet<string> uniqueCommonAccomodationCodes = new HashSet<string>();

            // Result list of generated target test accomodations 
            List<TestAccomodation> targetTestAccomdations = new List<TestAccomodation>();

            // Tests and their accommodation codes which are expanded explicitly for all segments eventhough they have common code "segment 0" specified
            Dictionary<string, HashSet<string>> explicitlyExpandedCommonAccommodationCodesForTests = new Dictionary<string, HashSet<string>>();

            // Split/Add segment specific accomodations and common accomodations of source tests
            foreach (TestResult sourceTestResult in sourceTestResults)
            {
                Opportunity sourceOpp = sourceTestResult.Opportunity;

                // Accommodation code and a list of target segments on which they are currently applied through explicit specification in the source test
                Dictionary<string, HashSet<int>> accommodationsOnSpecificTargetSegments = new Dictionary<string, HashSet<int>>();
                foreach (var sourceAccom in sourceOpp.Accomodations)
                {
                    // This accommodation is specific to a particular source segment
                    if (sourceAccom.Segment > 0)
                    {
                        // Find the corresponding target segment and add it with specific target segment position (non-zero)
                        TestSegment targetSegment = GetTargetSegment(sourceTestResult.Name, sourceAccom.Segment);
                        if (targetSegment == null)
                            throw new ApplicationException("Test Merge: Cannot merge accomodations");
                        targetTestAccomdations.Add(new TestAccomodation(sourceAccom.Type, sourceAccom.Description, sourceAccom.Code, targetSegment.Position, sourceAccom.Source, 0));

                        // Add this accommodation code and its target segment position for bookkeeping
                        if (!accommodationsOnSpecificTargetSegments.ContainsKey(sourceAccom.Code))
                            accommodationsOnSpecificTargetSegments.Add(sourceAccom.Code, new HashSet<int>());
                        accommodationsOnSpecificTargetSegments[sourceAccom.Code].Add(targetSegment.Position);
                    }
                    else
                    {
                        // Get the 'common code' dictionary for this test
                        Dictionary<string, TestAccomodation> sourceCommonAccomodations =
                            sourceTestCommonAccomodations.ContainsKey(sourceTestResult.Name) ? sourceTestCommonAccomodations[sourceTestResult.Name] : null;
                        if (sourceCommonAccomodations == null)
                        {
                            sourceCommonAccomodations = new Dictionary<string, TestAccomodation>();
                            sourceTestCommonAccomodations.Add(sourceTestResult.Name, sourceCommonAccomodations);
                        }

                        // Add the 0 code accomodation to this particulare test as well as to unique code collection.
                        if (sourceCommonAccomodations.ContainsKey(sourceAccom.Code))
                            throw new ApplicationException(string.Format("Test merge: Duplicate common accomdation code: {0} found for the test: {1}", sourceAccom.Code, sourceTestResult.Name));
                        sourceCommonAccomodations.Add(sourceAccom.Code, sourceAccom);
                        uniqueCommonAccomodationCodes.Add(sourceAccom.Code);
                    }
                }

                // Get the 'common code' dictionary for this test
                Dictionary<string, TestAccomodation> testCommonAccommodations =
                    sourceTestCommonAccomodations.ContainsKey(sourceTestResult.Name) ? sourceTestCommonAccomodations[sourceTestResult.Name] : null;
                if (testCommonAccommodations != null)
                {
                    foreach (KeyValuePair<string, TestAccomodation> kvp in testCommonAccommodations)
                    {
                        string accommodationCode = kvp.Key;
                        TestAccomodation accommodation = kvp.Value;

                        // Check, if we have overrides for this common code
                        if (accommodationsOnSpecificTargetSegments.ContainsKey(accommodationCode))
                        {
                            HashSet<int> overriddenTargetSegments = accommodationsOnSpecificTargetSegments[accommodationCode];
                            List<TestSegment> targetSegments = GetTargetSegments(sourceTestResult.Name);

                            // Add this accommodation to the target segments which are not added explicitly
                            foreach (TestSegment target in targetSegments)
                            {
                                if (overriddenTargetSegments.Contains(target.Position))
                                    continue;
                                targetTestAccomdations.Add(new TestAccomodation(
                                    accommodation.Type, accommodation.Description, accommodation.Code, target.Position, accommodation.Source, 0));
                            }

                            // Add this accommodation code to the explictly expanded list of accommodations for this particular test
                            if (!explicitlyExpandedCommonAccommodationCodesForTests.ContainsKey(sourceTestResult.Name))
                                explicitlyExpandedCommonAccommodationCodesForTests.Add(sourceTestResult.Name, new HashSet<string>());
                            explicitlyExpandedCommonAccommodationCodesForTests[sourceTestResult.Name].Add(accommodationCode);
                        }
                    }
                }
            }

            // Add the common code with 0 segment position or to corresponding target segments depending on availability of common code in 
            // all the source tests with same description
            if (sourceTestCommonAccomodations.Count > 0)
            {
                Dictionary<string, TestAccomodation> referenceTestAccomodations = sourceTestCommonAccomodations.First().Value;

                // Work on the common code and generate target accomdations 
                foreach (string uniqueAccomCode in uniqueCommonAccomodationCodes)
                {
                    // Check if the accom. code exists in all tests and the description matches 
                    bool bAllTestsHasSameCode = referenceTestAccomodations.ContainsKey(uniqueAccomCode) &&
                                                    sourceTestCommonAccomodations.All(
                                                            x => x.Value.ContainsKey(uniqueAccomCode)
                                                              && x.Value[uniqueAccomCode].Description.Equals(referenceTestAccomodations[uniqueAccomCode].Description));
                    // If the code is available in all source tests and the description matches then 
                    // Add it once with segment code as 0 otherwise add it to all corresponding target segments
                    if (bAllTestsHasSameCode)
                    {
                        TestAccomodation sample = referenceTestAccomodations[uniqueAccomCode];
                        targetTestAccomdations.Add(new TestAccomodation(
                                    sample.Type, sample.Description, sample.Code, 0, sample.Source, 0));
                    }
                    else
                    {
                        foreach (KeyValuePair<string, Dictionary<string, TestAccomodation>> kvp in sourceTestCommonAccomodations)
                        {
                            // If the code is not in this test then continue 
                            if (!kvp.Value.ContainsKey(uniqueAccomCode))
                                continue;

                            string sourceTestName = kvp.Key;
                            TestAccomodation sourceTestAccomodation = kvp.Value[uniqueAccomCode];

                            // If the code is already expanded for this test then skip generating it for the target segments for this test
                            if (explicitlyExpandedCommonAccommodationCodesForTests.ContainsKey(sourceTestName) &&
                                explicitlyExpandedCommonAccommodationCodesForTests[sourceTestName].Contains(sourceTestAccomodation.Code))
                                continue;

                            // Get the target segments corresponding to the segments applicable for this source test
                            List<TestSegment> targetSegments = GetTargetSegments(sourceTestName);
                            // Add the accomodation to each of the target segments
                            foreach (TestSegment target in targetSegments)
                                targetTestAccomdations.Add(new TestAccomodation(
                                    sourceTestAccomodation.Type, sourceTestAccomodation.Description, sourceTestAccomodation.Code, target.Position, sourceTestAccomodation.Source, 0));
                        }
                    }
                }
            }

            // Add the newly generated accomodations to the opportunity
            Opportunity targetOpp = targetTestResult.Opportunity;
            foreach (var targetTestAccomodation in targetTestAccomdations)
                targetOpp.AddAccomodation(targetTestAccomodation);
        }


        /// <summary>
        /// Merge generic variables
        /// </summary>
        /// <param name="sourceTestResults"></param>
        /// <param name="targetTestResult"></param>
        private void MergeGenericVariables(List<TestResult> sourceTestResults, TestResult targetTestResult)
        {
            Opportunity targetOpp = targetTestResult.Opportunity;
            foreach (TestResult sourceTestResult in sourceTestResults)
            {
                int nTargetPosition = SourceTestToTargetPositionMap.ContainsKey(sourceTestResult.Name) ? SourceTestToTargetPositionMap[sourceTestResult.Name] : -1;
                Opportunity sourceOpp = sourceTestResult.Opportunity;
                string contextFmt = "{0}_{1}";
                foreach (var genericVar in sourceOpp.GenericVariables)
                {
                    string sTargetContext = string.Format(contextFmt, genericVar.ContextString, nTargetPosition);
                    targetOpp.AddGenericVariable(sTargetContext, genericVar.Name, genericVar.Value);
                }
                //add the source oppID, status, and testID as generic variables on the target
                string context = string.Format(contextFmt, "COMPONENT", nTargetPosition);
                targetOpp.AddGenericVariable(context, "OPPID", sourceOpp.OpportunityID);
                targetOpp.AddGenericVariable(context, "STATUS", sourceOpp.Status);
                targetOpp.AddGenericVariable(context, "TESTID", sourceTestResult.TestID);
            }
        }

        /// <summary>
        /// Set the combined test testee attributes
        /// </summary>
        /// <param name="sourceTestResults"></param>
        /// <param name="targetTestResult"></param>
        private void SetTargetTestee(List<TestResult> sourceTestResults, TestResult targetTestResult)
        {
            // Entity keys should be same for all tests which are merged 
            Testee testee = new Testee(sourceTestResults[0].Testee.EntityKey, sourceTestResults[0].Testee.IsDemo);

            // Merge testee attributes and relationships 
            MergeTesteeAttribute(sourceTestResults, testee);
            MergeTesteeRelationship(sourceTestResults, testee);

            // Assign the testee object to the result
            targetTestResult.Testee = testee;
        }

        /// <summary>
        /// Merge the testee attributes to set the target attributes
        /// </summary>
        /// <param name="sourceTestResults"></param>
        /// <param name="testee"></param>
        private void MergeTesteeAttribute(List<TestResult> sourceTestResults, Testee testee)
        {
            // Gather testee attributes
            Dictionary<string, TesteeAttribute> targetTesteeAttributesINITIAL = new Dictionary<string, TesteeAttribute>();
            Dictionary<string, TesteeAttribute> targetTesteeAttributesFINAL = new Dictionary<string, TesteeAttribute>();
            foreach (TestResult sourceTestResult in sourceTestResults)
            {
                foreach (var sourceTesteeAttr in sourceTestResult.Testee.TesteeAttributes)
                {
                    if (sourceTesteeAttr.Context.Equals("INITIAL"))
                    {
                        TesteeAttribute currentTargetTesteeInitialAttr = targetTesteeAttributesINITIAL.ContainsKey(sourceTesteeAttr.Name)
                            ? targetTesteeAttributesINITIAL[sourceTesteeAttr.Name] : null;
                        if (currentTargetTesteeInitialAttr == null || currentTargetTesteeInitialAttr.ContextDate > sourceTesteeAttr.ContextDate)
                        {
                            currentTargetTesteeInitialAttr = new TesteeAttribute(sourceTesteeAttr.Context, sourceTesteeAttr.ContextDate, sourceTesteeAttr.Name, sourceTesteeAttr.Value);
                            if (targetTesteeAttributesINITIAL.ContainsKey(sourceTesteeAttr.Name))
                                targetTesteeAttributesINITIAL.Remove(sourceTesteeAttr.Name);
                            targetTesteeAttributesINITIAL.Add(sourceTesteeAttr.Name, currentTargetTesteeInitialAttr);
                        }
                    }
                    else if (sourceTesteeAttr.Context.Equals("FINAL"))
                    {
                        TesteeAttribute currentTargetTesteeFinalAttr = targetTesteeAttributesFINAL.ContainsKey(sourceTesteeAttr.Name)
                            ? targetTesteeAttributesFINAL[sourceTesteeAttr.Name] : null;
                        if (currentTargetTesteeFinalAttr == null || currentTargetTesteeFinalAttr.ContextDate < sourceTesteeAttr.ContextDate)
                        {
                            currentTargetTesteeFinalAttr = new TesteeAttribute(sourceTesteeAttr.Context, sourceTesteeAttr.ContextDate, sourceTesteeAttr.Name, sourceTesteeAttr.Value);
                            if (targetTesteeAttributesFINAL.ContainsKey(sourceTesteeAttr.Name))
                                targetTesteeAttributesFINAL.Remove(sourceTesteeAttr.Name);
                            targetTesteeAttributesFINAL.Add(sourceTesteeAttr.Name, currentTargetTesteeFinalAttr);
                        }
                    }
                }
            }

            // Add the collected initial and final attributes to the target testee
            foreach (var targetTesteeInitialAttr in targetTesteeAttributesINITIAL.Values)
                testee.AddAttribute(targetTesteeInitialAttr);
            foreach (var targetTesteeFinalAttr in targetTesteeAttributesFINAL.Values)
                testee.AddAttribute(targetTesteeFinalAttr);
        }

        /// <summary>
        /// Merge the testee relationships to set the target relationships
        /// </summary>
        /// <param name="sourceTestResults"></param>
        /// <param name="testee"></param>
        private void MergeTesteeRelationship(List<TestResult> sourceTestResults, Testee testee)
        {
            // Set testee relationships
            Dictionary<string, TesteeRelationship> targetTesteeRelationshipINITIAL = new Dictionary<string, TesteeRelationship>();
            Dictionary<string, TesteeRelationship> targetTesteeRelationshipFINAL = new Dictionary<string, TesteeRelationship>();
            foreach (TestResult sourceTestResult in sourceTestResults)
            {
                foreach (var sourceTesteeRelationship in sourceTestResult.Testee.TesteeRelationships)
                {
                    if (sourceTesteeRelationship.Context.Equals("INITIAL"))
                    {
                        TesteeRelationship currentTargetTesteeRelationshipInitial = targetTesteeRelationshipINITIAL.ContainsKey(sourceTesteeRelationship.Name)
                            ? targetTesteeRelationshipINITIAL[sourceTesteeRelationship.Name] : null;
                        if (currentTargetTesteeRelationshipInitial == null || currentTargetTesteeRelationshipInitial.ContextDate > sourceTesteeRelationship.ContextDate)
                        {
                            currentTargetTesteeRelationshipInitial = new TesteeRelationship(sourceTesteeRelationship.Context, sourceTesteeRelationship.ContextDate,
                                                        sourceTesteeRelationship.Name, sourceTesteeRelationship.Value, sourceTesteeRelationship.EntityKey);
                            if (targetTesteeRelationshipINITIAL.ContainsKey(sourceTesteeRelationship.Name))
                                targetTesteeRelationshipINITIAL.Remove(sourceTesteeRelationship.Name);
                            targetTesteeRelationshipINITIAL.Add(sourceTesteeRelationship.Name, currentTargetTesteeRelationshipInitial);
                        }
                    }
                    else if (sourceTesteeRelationship.Context.Equals("FINAL"))
                    {
                        TesteeRelationship currentTargetTesteeRelationshipFinal = targetTesteeRelationshipFINAL.ContainsKey(sourceTesteeRelationship.Name)
                            ? targetTesteeRelationshipFINAL[sourceTesteeRelationship.Name] : null;
                        if (currentTargetTesteeRelationshipFinal == null || currentTargetTesteeRelationshipFinal.ContextDate < sourceTesteeRelationship.ContextDate)
                        {
                            currentTargetTesteeRelationshipFinal = new TesteeRelationship(sourceTesteeRelationship.Context, sourceTesteeRelationship.ContextDate,
                                                        sourceTesteeRelationship.Name, sourceTesteeRelationship.Value, sourceTesteeRelationship.EntityKey);
                            if (targetTesteeRelationshipFINAL.ContainsKey(sourceTesteeRelationship.Name))
                                targetTesteeRelationshipFINAL.Remove(sourceTesteeRelationship.Name);
                            targetTesteeRelationshipFINAL.Add(sourceTesteeRelationship.Name, currentTargetTesteeRelationshipFinal);
                        }
                    }
                }
            }

            // Add the collected initial and final testee relationship attributes to the target testee
            foreach (var targetTesteeRelaionshipInitial in targetTesteeRelationshipINITIAL.Values)
                testee.AddRelationship(targetTesteeRelaionshipInitial);
            foreach (var targetTesteeRelaionshipFinal in targetTesteeRelationshipFINAL.Values)
                testee.AddRelationship(targetTesteeRelaionshipFinal);

        }

        /// <summary>
        /// Merge item responses
        /// </summary>
        /// <param name="sourceTestResults"></param>
        /// <param name="targetTestResult"></param>
        private void MergeItemResponsesAndSetItemPositionMapping(List<TestResult> sourceTestResults, TestResult targetTestResult)
        {
            // Create target item responses (without position information) by deep copying the source item responses
            List<ItemResponse> createdTargetItemResponses = new List<ItemResponse>();
            foreach (TestResult sourceTestResult in sourceTestResults)
            {
                if (sourceTestResult.Opportunity.Status.Equals("invalidated") && !targetTestResult.Opportunity.Status.Equals("invalidated"))
                    continue; 
                foreach (ItemResponse sourceItemResponse in sourceTestResult.ItemResponses)
                {
                    ItemResponse targetItemResponse = CreateTargetItemResponse(sourceTestResult, sourceItemResponse);
                    if (targetItemResponse == null)
                        throw new ApplicationException(string.Format("Test Merge: Transforming item response failed for test {0}", sourceTestResult.Name));
                    createdTargetItemResponses.Add(targetItemResponse);
                }
            }

            // We need to establish item position mapping and set target item positions 
            SourceTestItemPositionToTargetItemPositionMap.Clear();
            List<ItemResponse> targetItemResponses = new List<ItemResponse>();

            // Set the positions based only on the available targets segments i.e target segments of source segments which are currently participating in merge
            int nPosition = 1;
            List<TestSegment> availableTargetSegments = GetAvailableTargetSegments();
            foreach (TestSegment targetSegment in availableTargetSegments)
            {
                // Get the source segement name and test name 
                string sourceSegmentName = _mergeConfig.GetSourceSegmentName(targetSegment.ID);
                string sourceTestName = _mergeConfig.GetSourceTestName(sourceSegmentName);
                
                // Check/create if we have item position mapping for this source test
                Dictionary<int, int> itemPositionMap = null;
                if (SourceTestItemPositionToTargetItemPositionMap.ContainsKey(sourceTestName))
                    itemPositionMap = SourceTestItemPositionToTargetItemPositionMap[sourceTestName];
                else
                {
                    itemPositionMap = new Dictionary<int, int>();
                    SourceTestItemPositionToTargetItemPositionMap.Add(sourceTestName, itemPositionMap);
                }

                // Gather all the responses for this particular target segment 
                List<ItemResponse> thisSegmentItemResponses = createdTargetItemResponses.Where(x => x.SegmentID.Equals(targetSegment.ID)).ToList();

                // Sort them based on their position 
                thisSegmentItemResponses.Sort((x, y) => (x.Position.CompareTo(y.Position)));

                // Re-establish the position based on their sorted order in the source test
                foreach (ItemResponse itemResponse in thisSegmentItemResponses)
                {
                    // Add the mapping for use to map the comments 
                    itemPositionMap.Add(itemResponse.Position, nPosition);                     

                    // Set the new position 
                    itemResponse.Position = nPosition;
                    itemResponse.PageNumber = nPosition;

                    // Increment the position#
                    ++nPosition;
                    
                    // Add the modified response to the target list
                    targetItemResponses.Add(itemResponse);                    
                }                
            }

            // Finally set the responses list to the target result
            targetTestResult.Opportunity.ItemResponses = targetItemResponses;
        }

        /// <summary>
        /// Transform and create a new item response for target
        /// NOTE: Item Position will be set after all the item responses are created based on the target segment
        /// </summary>
        /// <param name="sourceItemResponse"></param>
        /// <returns></returns>
        private ItemResponse CreateTargetItemResponse(TestResult sourceTestResult, ItemResponse sourceItemResponse)
        {
            // Target item response
            ItemResponse targetItemResponse = null;

            // Get the target segment for the source segment 
            TestSegment targetSegment = SourceSegmentNameToTargetSegmentMap.ContainsKey(sourceItemResponse.SegmentID) ?
                                                SourceSegmentNameToTargetSegmentMap[sourceItemResponse.SegmentID] : null;

            // Get the target segments item response
            if (targetSegment != null)
            {
                // Construct a copy of the source response 
                targetItemResponse = sourceItemResponse.DeepCopy();

                // Set the segment id as 'targets' segment ID 
                targetItemResponse.SegmentID = targetSegment.ID;
            }

            // Return the constructed target item response
            return targetItemResponse;
        }

        /// <summary>
        /// Merge comments
        /// </summary>
        /// <param name="sourceTestResults"></param>
        /// <param name="targetTestResult"></param>
        private void MergeComments(List<TestResult> sourceTestResults, TestResult targetTestResult)
        {
            List<Comment> comments = new List<Comment>();
            foreach (TestResult testResult in sourceTestResults)
            {
                if (testResult.Opportunity.Status.Equals("invalidated") && !targetTestResult.Opportunity.Status.Equals("invalidated"))
                    continue; 
                foreach (Comment comment in testResult.Comments)
                {
                    Comment targetComment = CreateTargetComment(testResult, comment);
                    if (targetComment == null)
                        throw new ApplicationException("Test Merge: Transforming comment failed");
                    comments.Add(targetComment);                    
                }
            }
            targetTestResult.Comments = comments;            
        }

        /// <summary>
        /// Create target for the comment in the source
        /// </summary>
        /// <param name="sourceTestResult"></param>
        /// <param name="sourceComment"></param>
        /// <returns></returns>
        private Comment CreateTargetComment(TestResult sourceTestResult, Comment sourceComment)
        {
            // Get the current position in the source comment as target 
            int? nTargetItemPosition = sourceComment.ItemPosition;

            // If the comment is not global, remap it to the target position 
            if (sourceComment.ItemPosition != null && sourceComment.ItemPosition > 0)
            {
                nTargetItemPosition = GetTargetItemPosition(sourceTestResult.Name, sourceComment.ItemPosition.Value);
                if (nTargetItemPosition == -1)
                    throw new ApplicationException(string.Format("Test Merge: Transforming comment failed, cannot map target item position for item {0} in test {1}", sourceComment.ItemPosition.Value, sourceTestResult.Name));
            }

            return new Comment(sourceComment.Context, sourceComment.Date, nTargetItemPosition, sourceComment.CommentValue);
        }

        /// <summary>
        /// Merge tool usage nodes in the source tests
        /// </summary>
        /// <param name="sourceTestResults"></param>
        /// <param name="targetTestResult"></param>
        private void MergeToolUsage(List<TestResult> sourceTestResults, TestResult targetTestResult)
        {
            Dictionary<string, ToolUsage> targetToolUsages = new Dictionary<string,ToolUsage>();
            
            foreach (TestResult sourceTestResult in sourceTestResults)
            {
                if (sourceTestResult.Opportunity.Status.Equals("invalidated") && !targetTestResult.Opportunity.Status.Equals("invalidated"))
                    continue; 
                foreach (ToolUsage sourceToolUsage in sourceTestResult.ToolUsages)
                {
                    ToolUsage targetToolUsage = null;
                    if (targetToolUsages.ContainsKey(sourceToolUsage.Code))
                        targetToolUsage = targetToolUsages[sourceToolUsage.Code];
                    else 
                    {
                        targetToolUsage = new ToolUsage(sourceToolUsage.Type, sourceToolUsage.Code);
                        targetToolUsages.Add(sourceToolUsage.Code, targetToolUsage);
                    }
                    foreach(ToolPage sourceToolPage in sourceToolUsage.ToolPages) 
                    {
                        int nTargetPageNumber = GetTargetItemPosition(sourceTestResult.Name, sourceToolPage.PageNumber);
                        if (nTargetPageNumber == -1)
                            throw new ApplicationException(string.Format("Test Merge: Transforming tool usage failed, cannot map target item position for item {0} in test {1}", sourceToolPage.PageNumber, sourceTestResult.Name));
                        targetToolUsage.AddToolPage(new ToolPage(nTargetPageNumber, sourceToolPage.GroupID, sourceToolPage.Count));
                    }
                }
            }
            targetTestResult.ToolUsages = targetToolUsages.Values.ToList();      
        }

        /// <summary>
        /// Get target segments in positional order
        /// </summary>
        /// <returns></returns>
        private List<TestSegment> GetAllConfiguredTargetSegments()
        {
            List<TestSegment> targetTestSegments = SourceSegmentNameToTargetSegmentMap.Values.ToList();
            targetTestSegments.Sort((x, y) => x.Position.CompareTo(y.Position));
            return targetTestSegments;
        }

        /// <summary>
        /// Get available target segments in positional order
        /// </summary>
        /// <returns></returns>
        private List<TestSegment> GetAvailableTargetSegments()
        {
            // Get only the available target segments as kvp 
            List<TestSegment> availableTargetSegments = SourceSegmentNameToTargetSegmentMap.Where(
                                                            x => SourceSegmentNameToSourceSegmentMap.ContainsKey(x.Key)).Select(y=>y.Value).ToList();
            availableTargetSegments.Sort((x, y) => x.Position.CompareTo(y.Position));
            return availableTargetSegments;
        }

        /// <summary>
        /// Get target segment given the source test name and the position of the segment
        /// </summary>
        /// <param name="sourceTestName"></param>
        /// <param name="sourceSegmentPosition"></param>
        /// <returns></returns>
        private TestSegment GetTargetSegment(string sourceTestName, int sourceSegmentPosition)
        {
            // Get all the source segment names for this test
            List<string> sourceSegmentNames = _mergeConfig.GetSourceSegmentNames(sourceTestName);

            // Find the source segment at this given position 
            KeyValuePair<string, TestSegment> kvp = SourceSegmentNameToSourceSegmentMap.ToList().Find(x => sourceSegmentNames.Contains(x.Key) && x.Value.Position == sourceSegmentPosition);

            // Get the corresponding target segment
            return (!kvp.Equals(default(KeyValuePair<string, TestSegment>)) && SourceSegmentNameToTargetSegmentMap.ContainsKey(kvp.Key)) ? SourceSegmentNameToTargetSegmentMap[kvp.Key] : null;
        }

        /// <summary>
        /// Get target segments for the given source test
        /// </summary>
        /// <param name="sourceTestName"></param>
        /// <returns></returns>
        private List<TestSegment> GetTargetSegments(string sourceTestName)
        {
            // Get all the source segment names for the test
            List<string> sourceSegmentNames = _mergeConfig.GetSourceSegmentNames(sourceTestName);
            
            // Return the matching target segments for the source segments
            return SourceSegmentNameToTargetSegmentMap.Where(x => sourceSegmentNames.Contains(x.Key)).Select(y=>y.Value).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceTestName"></param>
        /// <param name="sourceItemPosition"></param>
        /// <returns></returns>
        private int GetTargetItemPosition(string sourceTestName, int sourceItemPosition)
        {
            Dictionary<int, int> sourceTestItemPositions = 
                SourceTestItemPositionToTargetItemPositionMap.ContainsKey(sourceTestName) ? SourceTestItemPositionToTargetItemPositionMap[sourceTestName] : null ;
            return (sourceTestItemPositions != null && sourceTestItemPositions.ContainsKey(sourceItemPosition)) ? sourceTestItemPositions[sourceItemPosition] : -1;
        }

        /// <summary>
        /// Map of source segment name to target segment objects
        /// </summary>
        private Dictionary<string, TestSegment> SourceSegmentNameToTargetSegmentMap = new Dictionary<string, TestSegment>();

        /// <summary>
        /// Map of (available) source segment name to its test segment object
        /// </summary>
        private Dictionary<string, TestSegment> SourceSegmentNameToSourceSegmentMap = new Dictionary<string, TestSegment>();

        /// <summary>
        /// Mapping of each <test, itemPosition> pair to -> target item position
        /// </summary>
        private Dictionary<string, Dictionary<int, int>> SourceTestItemPositionToTargetItemPositionMap = new Dictionary<string, Dictionary<int, int>>();
 
        /// <summary>
        /// Mapping of source test to target position map (for generic variables) - This assumes segments are mapped in order
        /// </summary>
        private Dictionary<string, int> SourceTestToTargetPositionMap = new Dictionary<string,int>();

        /// <summary>
        /// Related XML file info the current test being processed
        /// </summary>
        private Dictionary<string, RelatedTestOpportunity> RelatedXMLFileInfo = null;

        /// <summary>
        /// Configuration for merging
        /// </summary>
        private MergeConfig _mergeConfig;
    }
}