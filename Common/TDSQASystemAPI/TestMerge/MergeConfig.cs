/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScoringEngine.ConfiguredTests;
using TDSQASystemAPI.TestResults;
using ScoringEngine;

namespace TDSQASystemAPI.TestMerge
{
    /// <summary>
    /// A class to allow configuring the tests which need to be merged to have a different target test
    /// </summary>
    public class MergeConfig
    {
        /// <summary>
        /// Target test for this merge 
        /// </summary>
        public string TargetTestName = null;

        /// <summary>
        /// List of tests which will be merged 
        /// </summary>
        public List<string> SourceTestNamesForMerge = new List<string>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="targetTest"></param>
        public MergeConfig(string targetTest)
        {
            TargetTestName = targetTest;
        }

        /// <summary>
        /// Add particular config elements for the target test
        /// </summary>
        /// <param name="sSourceTestName"></param>
        /// <param name="sSourceSegmentName"></param>
        /// <param name="sTargetSegmentName"></param>
        /// <param name="sSourceFormKey"></param>
        /// <param name="sTargetFormKey"></param>
        public void AddConfig(string sSourceTestName,
                               string sSourceSegmentName, string sTargetSegmentName,
                               string sSourceFormKey, string sTargetFormKey)
        {
            // Add the source test to the list of source tests
            if (!SourceTestNamesForMerge.Exists(x => x.Equals(sSourceTestName)))
                SourceTestNamesForMerge.Add(sSourceTestName);

            // Add the source segment to the 'segments list' for source test
            if (SourceTestToSourceSegmentNamesMap.ContainsKey(sSourceTestName))
                SourceTestToSourceSegmentNamesMap[sSourceTestName].Add(sSourceSegmentName);
            else
            {
                HashSet<string> segments = new HashSet<string>();
                segments.Add(sSourceSegmentName);
                SourceTestToSourceSegmentNamesMap.Add(sSourceTestName, segments);
            }

            // Map the source segment to source test name
            if (!SourceSegmentToSourceTestNamesMap.ContainsKey(sSourceSegmentName))
                SourceSegmentToSourceTestNamesMap.Add(sSourceSegmentName, sSourceTestName);

            // Map the source segment to the target segment
            if (!SourceSegmentToTargetSegmentNamesMap.ContainsKey(sSourceSegmentName))
                SourceSegmentToTargetSegmentNamesMap.Add(sSourceSegmentName, sTargetSegmentName);

            // Add the source form to the source segment.
            if (!string.IsNullOrEmpty(sSourceFormKey) && !string.IsNullOrEmpty(sTargetFormKey))
            {
                if (SourceSegmentToSourceFormKeyNamesMap.ContainsKey(sSourceSegmentName))
                    SourceSegmentToSourceFormKeyNamesMap[sSourceSegmentName].Add(sSourceFormKey);
                else
                {
                    HashSet<string> formKeys = new HashSet<string>();
                    formKeys.Add(sSourceFormKey);
                    SourceSegmentToSourceFormKeyNamesMap.Add(sSourceSegmentName, formKeys);
                }

                // Map the source form key to the target form key
                if (!SourceFormToTargetFormKeysMap.ContainsKey(sSourceFormKey))
                    SourceFormToTargetFormKeysMap.Add(sSourceFormKey, sTargetFormKey);
            }
        }

        /// <summary>
        /// Get the target segment name given the source segment name 
        /// </summary>
        /// <param name="sourceSegmentName"></param>
        public string GetTargetSegmentName(string sourceSegmentName)
        {
            return SourceSegmentToTargetSegmentNamesMap.ContainsKey(sourceSegmentName) ? SourceSegmentToTargetSegmentNamesMap[sourceSegmentName] : null;
        }

        /// <summary>
        /// Get the target form key given the source form key
        /// </summary>
        /// <param name="sourceFormKey"></param>
        public string GetTargetFormKey(string sourceFormKey)
        {
            return SourceFormToTargetFormKeysMap.ContainsKey(sourceFormKey) ? SourceFormToTargetFormKeysMap[sourceFormKey] : null;
        }

        /// <summary>
        /// Get source test name given the source segment name
        /// </summary>
        /// <param name="sSourceSegmentName"></param>
        /// <returns></returns>
        public string GetSourceTestName(string sSourceSegmentName)
        {
            return SourceSegmentToSourceTestNamesMap.ContainsKey(sSourceSegmentName) ? SourceSegmentToSourceTestNamesMap[sSourceSegmentName] : null;
        }

        /// <summary>
        /// Get source segment names for the given source test 
        /// </summary>
        /// <param name="sSourceTestName"></param>
        /// <returns></returns>
        public List<string> GetSourceSegmentNames(string sSourceTestName)
        {
            return SourceTestToSourceSegmentNamesMap.ContainsKey(sSourceTestName) ? SourceTestToSourceSegmentNamesMap[sSourceTestName].ToList() : new List<string>();
        }

        /// <summary>
        /// Get source form keys for the given segment
        /// </summary>
        /// <param name="sSourceSegmentName"></param>
        /// <returns></returns>
        public List<string> GetSourceFormKeys(string sSourceSegmentName)
        {
            return SourceSegmentToSourceFormKeyNamesMap.ContainsKey(sSourceSegmentName) ? SourceSegmentToSourceFormKeyNamesMap[sSourceSegmentName].ToList() : new List<string>();
        }

        /// <summary>
        /// Mapping of source test to list of source segment names for the test
        /// </summary>
        private Dictionary<string, HashSet<string>> SourceTestToSourceSegmentNamesMap = new Dictionary<string, HashSet<string>>();

        /// <summary>
        /// Mapping of source segment to source test name map
        /// </summary>
        private Dictionary<string, string> SourceSegmentToSourceTestNamesMap = new Dictionary<string, string>();

        /// <summary>
        /// Mapping of source segment name to target segment
        /// </summary>
        private Dictionary<string, string> SourceSegmentToTargetSegmentNamesMap = new Dictionary<string, string>();

        /// <summary>
        /// Mapping of source segment name to list of source form keys
        /// </summary>
        private Dictionary<string, HashSet<string>> SourceSegmentToSourceFormKeyNamesMap = new Dictionary<string, HashSet<string>>();

        /// <summary>
        /// Mapping of source form key to target form key
        /// </summary> 
        private Dictionary<string, string> SourceFormToTargetFormKeysMap = new Dictionary<string, string>();
    }
}
