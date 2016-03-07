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
using System.Threading.Tasks;
using System.Xml;

namespace TIS.ScoringDaemon.Abstractions.Extensions
{
    public static class ExtensionMethods
    {
        public static int[] GetItemKeyTokens(this TDS.ScoringDaemon.Abstractions.ScorableResponse scorableResponse)
        {
            return GetItemKeyTokens(scorableResponse.ItemKey);
        }

        public static int[] GetItemKeyTokens(this TDS.ScoringDaemon.Abstractions.ScoredResponse scoredResponse)
        {
            return GetItemKeyTokens(scoredResponse.ItemKey);
        }

        private static int[] GetItemKeyTokens(string itemKey)
        {
            return (itemKey ?? "").Split(new char[] { '-' }).Select(s => int.Parse(s)).ToArray();
        }

        public static XmlDocument ScoreInfoAsXml(this TDS.ScoringDaemon.Abstractions.ScoredResponse scoredResponse)
        {
            if (String.IsNullOrEmpty(scoredResponse.ScoreDimensions))
                return null;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(scoredResponse.ScoreDimensions);
            return doc;
        }

        public static string AsString(this TDS.ScoringDaemon.Abstractions.ScoredResponse scoredResponse)
        {
            return String.Format("Opp: {0}, Item: {1}, ReportingVersion: {2}, ScoreStatus: {3}, Score: {4}, ScoreInfo: {5}, ScoreRationale: {6}", 
                scoredResponse.OppKey, scoredResponse.ItemKey, scoredResponse.Sequence, scoredResponse.ScoreStatus, scoredResponse.Score,
                scoredResponse.ScoreDimensions, scoredResponse.ScoreRationale);
        }
    }
}
