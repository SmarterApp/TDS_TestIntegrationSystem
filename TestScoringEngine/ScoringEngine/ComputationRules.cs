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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using ScoringEngine.ConfiguredTests;
using ScoringEngine.Scoring;
using System.Reflection;

namespace ScoringEngine
{
    public class Scorer
    {

        private TestCollection tc;

        public Scorer(TestCollection tc)
        {
            this.tc = tc;
        }

        public TestCollection GetTestCollection()
        {
            return tc;
        }

        /// <summary>
        /// For TDS
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="testScoreString"></param>
        /// <param name="testEndDate"></param>
        /// <param name="rowSeparator"></param>
        /// <param name="colSeparator"></param>
        /// <returns></returns>
        public string TestScore(string testName, string testScoreString, DateTime testEndDate, char rowSeparator, char colSeparator, string status)
        {
            return TestScore(testName, testScoreString, testEndDate, rowSeparator, colSeparator, status, "", "", DateTime.MinValue, TestMode.Online);
        }

        public string TestScore(string testName, string testScoreString, DateTime testEndDate, char rowSeparator, char colSeparator, string status, TestMode mode)
        {
            return TestScore(testName, testScoreString, testEndDate, rowSeparator, colSeparator, status, "", "", DateTime.MinValue, mode);
        }

        public string TestScore(string testName, string testScoreString, DateTime testEndDate, char rowSeparator, char colSeparator, string status, string enrolledGrade)
        {
            return TestScore(testName, testScoreString, testEndDate, rowSeparator, colSeparator, status, enrolledGrade, "", DateTime.MinValue, TestMode.Online);
        }

        public string TestScore(string testName, string testScoreString, DateTime testEndDate, char rowSeparator, char colSeparator, string status, string enrolledGrade, string form)
        {
            return TestScore(testName, testScoreString, testEndDate, rowSeparator, colSeparator, status, enrolledGrade, form, DateTime.MinValue, TestMode.Online);
        }

        public string TestScore(string testName, string testScoreString, DateTime testEndDate, char rowSeparator, char colSeparator, string status, string enrolledGrade, string form, DateTime dateForceCompleted, TestMode mode)
        {
            // first check whether anything is going to be scored...
            bool toScore = false;
            if (tc.TestScoring.ContainsKey(testName))
                foreach (ComputationSpec comp in tc.TestScoring[testName])
                    if (comp.HasComputationLocation(tc.Environment))
                        toScore = true;
            if (!toScore)
                return "";

            List<ItemScore> testScores = new List<ItemScore>();

            if (testScoreString.StartsWith("<Item itemkey="))
                testScores = ParseXMLScoreString(testName, testScoreString, rowSeparator);
            else
                testScores = ParseScoreString(testName, testScoreString, rowSeparator, colSeparator);

            Dictionary<string, Dictionary<string, MeasureValue>> measureValues = ApplyComputationRules(testName, enrolledGrade, new Dictionary<string, Dictionary<string, MeasureValue>>(), testScores, testEndDate, status, form, dateForceCompleted, mode);

            string scores = "";
            foreach (Dictionary<string, MeasureValue> measures in measureValues.Values)
            {
                foreach (MeasureValue measureValue in measures.Values)
                {
                    scores = scores + measureValue.TDSValue(colSeparator) + rowSeparator;
                }
            }

            if (scores.Length > 0)
            {
                scores = scores.Remove(scores.Length - 1);
            }

            return scores;
        }


        public Dictionary<string, Dictionary<string, MeasureValue>> ApplyComputationRules(string testName, string enrolledGrade, List<ItemScore> testScores, DateTime testDate, string status, string form)
        {
            return ApplyComputationRules(testName, enrolledGrade, new Dictionary<string, Dictionary<string, MeasureValue>>(), testScores, testDate, status, form, DateTime.MinValue, TestMode.Online);
        }

        public Dictionary<string, Dictionary<string, MeasureValue>> ApplyComputationRules(string testName, Dictionary<string, Dictionary<string, MeasureValue>> measuresIn, List<ItemScore> testScores, DateTime testDate, string status)
        {
            return ApplyComputationRules(testName, "", measuresIn, testScores, testDate, status, "", DateTime.MinValue, TestMode.Online);
        }

        public Dictionary<string, Dictionary<string, MeasureValue>> ApplyComputationRules(string testName, string enrolledGrade, Dictionary<string, Dictionary<string, MeasureValue>> measuresIn, List<ItemScore> testScores, DateTime testDate, string status, string form)
        {
            return ApplyComputationRules(testName, enrolledGrade, measuresIn, testScores, testDate, status, form, DateTime.MinValue, TestMode.Online);
        }

        public Dictionary<string, Dictionary<string, MeasureValue>> ApplyComputationRules(string testName, string enrolledGrade, Dictionary<string, Dictionary<string, MeasureValue>> measuresIn, List<ItemScore> testScores, DateTime testDate, string status, string form, DateTime forceCompletionDate, TestMode mode)
        {
            Dictionary<string, string> results = new Dictionary<string,string>();
            SpecificComputationRules rules = null;
            bool first = true;

            if (tc.TestScoring.ContainsKey(testName))
            {
                foreach (ComputationSpec comp in tc.TestScoring[testName])
                {
					if (comp.HasComputationLocation(tc.Environment))
                    {
                        if (first)
                        {
                            rules = new SpecificComputationRules(testName, status, enrolledGrade, form, measuresIn, testScores, testDate, tc, forceCompletionDate, mode);
                            first = false;
                        }
                        Type classType = typeof(SpecificComputationRules);
                        MethodInfo computationMethod = classType.GetMethod(comp.ComputationRule);
                        if (computationMethod == null)
                            throw new ScoringEngineException("No method called " + comp.ComputationRule + " in the SpecificComputationRules class");

                        object[] args = new object[computationMethod.GetParameters().Length];
                        args[0] = comp.MeasureOf;
                        args[1] = comp.MeasureLabel;
                        for (int i = 2; i < args.Length; i++)
                        {
                            List<ParameterValue> par = comp.ParameterValues[i - 1];
                            if (par[0].IndexType == "")
                            {
                                switch (par[0].Type.ToLower())
                                {
                                    case "int":
                                        args[i] = Convert.ToInt32(par[0].Value);
                                        break;
                                    case "double":
                                        args[i] = Convert.ToDouble(par[0].Value);
                                        break;
                                    case "string":
                                        args[i] = par[0].Value;
                                        break;
                                }
                            }
                            else if (par[0].IndexType.ToLower() == "int")
                            {
                                Dictionary<int, int> tmpI = new Dictionary<int, int>();
                                Dictionary<int, double> tmpD = new Dictionary<int, double>();
                                Dictionary<int, string> tmpS = new Dictionary<int, string>();
                                //switch (type)
                                //{
                                //    case "int":
                                //        args[i] = new Dictionary<int, int>();
                                //        break;
                                //    case "double":
                                //        args[i] = new Dictionary<int, double>();
                                //        break;
                                //    case "string":
                                //        args[i] = new Dictionary<int, string>();
                                //        break;
                                //}
                                //foreach (ParameterValue pr in par)
                                //{
                                //    int index = Convert.ToInt32(pr.Index);
                                //    switch (pr.Type.ToLower())
                                //    {
                                //        case "int":
                                //            args[i][index] = Convert.ToInt32(pr.Value);
                                //            break;
                                //        case "double":
                                //            args[i][index] = Convert.ToDouble(pr.Value);
                                //            break;
                                //        case "string":
                                //            args[i][index] = pr.Value;
                                //            break;
                                //    }
                                //}
                                foreach (ParameterValue pr in par)
                                {
                                    int index = Convert.ToInt32(pr.Index);
                                    switch (pr.Type.ToLower())
                                    {
                                        case "int":
                                            tmpI[index] = Convert.ToInt32(pr.Value);
                                            break;
                                        case "double":
                                            tmpD[index] = Convert.ToDouble(pr.Value);
                                            break;
                                        case "string":
                                            tmpS[index] = pr.Value;
                                            break;
                                    }
                                }
                                switch (par[0].Type.ToLower())
                                {
                                    case "int":
                                        args[i] = tmpI;
                                        break;
                                    case "double":
                                        args[i] = tmpD;
                                        break;
                                    case "string":
                                        args[i] = tmpS;
                                        break;
                                }
                            }
                            else if (par[0].IndexType.ToLower() == "string")
                            {
                                Dictionary<string, int> tmpI = new Dictionary<string, int>();
                                Dictionary<string, double> tmpD = new Dictionary<string, double>();
                                Dictionary<string, string> tmpS = new Dictionary<string, string>();

                                foreach (ParameterValue pr in par)
                                {
                                    string index = pr.Index;
                                    switch (pr.Type.ToLower())
                                    {
                                        case "int":
                                            tmpI[index] = Convert.ToInt32(pr.Value);
                                            break;
                                        case "double":
                                            tmpD[index] = Convert.ToDouble(pr.Value);
                                            break;
                                        case "string":
                                            tmpS[index] = pr.Value;
                                            break;
                                    }
                                }
                                switch (par[0].Type.ToLower())
                                {
                                    case "int":
                                        args[i] = tmpI;
                                        break;
                                    case "double":
                                        args[i] = tmpD;
                                        break;
                                    case "string":
                                        args[i] = tmpS;
                                        break;
                                }

                            }
                        }

                        try
                        {
                            results[comp.MeasureLabel] = (string)computationMethod.Invoke(rules, args);
                        }
                        catch (Exception e)
                        {
                            throw new ScoringEngineException("Error during application of computation rule " + comp.ComputationRule + ", measureOf " + comp.MeasureOf + ", measureLabel " + comp.MeasureLabel + ". Error: " + e.InnerException.Message, e.InnerException);
                        }
                    }
                }
            }

            if (rules != null)
                return rules.MeasureValues;
            else
                return new Dictionary<string, Dictionary<string, MeasureValue>>();
        }

        private List<ItemScore> ParseXMLScoreString(string testName, string testScoreString, char rowSeparator)
        {
            List<ItemScore> testScores = new List<ItemScore>();
            /*
            char[] responseSeparator = { rowSeparator };
            foreach (string response in testScoreString.Split(responseSeparator))
            {
                XmlReaderSettings set = new XmlReaderSettings();
                set.ConformanceLevel = ConformanceLevel.Fragment;
                XPathDocument xmlDoc = new XPathDocument(XmlReader.Create(new StringReader(response), set));
                XPathNavigator xmlNav = xmlDoc.CreateNavigator();
            */

            XmlReaderSettings set = new XmlReaderSettings();
            set.ConformanceLevel = ConformanceLevel.Fragment;
            XPathDocument xmlDoc = new XPathDocument(XmlReader.Create(new StringReader(testScoreString), set));
            XPathNavigator xmlNav = xmlDoc.CreateNavigator();
            XPathNodeIterator items = xmlNav.Select("/Item");
            foreach (XPathNavigator item in items)
            {
                string response = item.InnerXml;
                string itemName = item.SelectSingleNode("@itemkey").Value;
                TestItem ti = tc.FindItem(testName, itemName);
                if (ti == null)
                    throw new ScoringEngineException("No item '" + itemName + "' on test '" + testName + "'");
                bool isSelected = true;
                string isSelectedAttrib = item.SelectSingleNode("@isSelected").Value;
                if (isSelectedAttrib == "0")
                    isSelected = false;
                else if (isSelectedAttrib != "1")
                    throw new ScoringEngineException("isSelected must have value '0' or '1' in " + response);

                bool isFT = true;
                string fieldTestIndicator = item.SelectSingleNode("@purpose").Value;
                if (fieldTestIndicator == "OP")
                    isFT = false;
                else if (fieldTestIndicator == "FT")
                    isFT = true;
                else
                    throw new ScoringEngineException("Field test indicator must be either 'OP' or 'FT', not '" + fieldTestIndicator + "' in " + response);
                XPathNavigator node = item.SelectSingleNode("@segmentID");
                string segmentID = "";
                if (node != null)
                    segmentID = node.Value;
                node = item.SelectSingleNode("@isDropped");
                bool dropped = false;
                if (node != null)
                {
                    switch (node.Value)
                    {
                        case "0":
                            dropped = false;
                            break;
                        case "1":
                            dropped = true;
                            break;
                        default:
                            throw new ScoringEngineException("isDropped must have value '0' or '1' in " + response);
                    }
                }
                node = item.SelectSingleNode("@position");
                int position = -1;
                if (node != null)
                    position = Convert.ToInt32(node.Value);

                XPathNodeIterator subscores = item.Select("ScoreInfo/SubScoreList/ScoreInfo");
                if (subscores.Count < 2)
                {
                    double score, maxScore;
                    if (!isSelected)
                    {
                        score = 0;
                    }
                    else
                    {
                        try
                        {
                            score = Convert.ToDouble(item.SelectSingleNode("ScoreInfo/@scorePoint").Value);
                            /*
                            maxScore = Convert.ToDouble(item.SelectSingleNode("ScoreInfo/@scoreMaxPoint").Value);
                            if (score > maxScore)
                                throw new ScoringEngineException("scorePoint can't exceed scoreMaxPoint in " + response);
                             */
                        }
                        catch (Exception e)
                        {
                            throw new ScoringEngineException("Can't parse score value from response: " + response);
                        }
                    }
                    string dimension = item.SelectSingleNode("ScoreInfo/@scoreDimension").Value;
                    if (dimension == "overall")
                        dimension = "";
                    else
                        throw new ScoringEngineException("Was expecting scoreDimension = 'overall' in " + response);
                    if (score == -1)
                    {
                        // create dimension scores of -1 (if this was FT, it will allow item counts to be correct, if it was OP and it is 
                        // incorrectly -1, it will error out later with a more meaningfull error);
                        foreach (TestItemScoreInfo si in ti.ScoreInfo)
                        {
                            ItemScore ir = new ItemScore(ti, si, position, score, "", segmentID, isFT, isSelected, true, dropped);
                            if (isFT || ir.TreatAsNotPresented())
                                testScores.Add(ir);
                        }
                    }
                    else
                    {
                        TestItemScoreInfo si = ti.GetScoreInfo(dimension);
                        if (si == null)
                            throw new ScoringEngineException("Item '" + itemName + "' does not have dimension '" + dimension + "'");
                        ItemScore ir = new ItemScore(ti, si, position, score, "", segmentID, isFT, isSelected, true, dropped);
                        // request from Larry: if item wasn't scored (e.g. grid or NL item scoring failed), score will be -1, just
                        // ignore this item but still compute overall scores (needed for starting ability for next opportunity).
                        // Include FT items so that we can get item counts right
                        // Include "treat as not presented" items to get DCAS-Alt attemptedness rules correct (treat-as-not-presented items count towards attemptedness)
                        if (score != -1 || isFT || ir.TreatAsNotPresented())
                            testScores.Add(ir);
                    }
                }
                else
                {
                    foreach (XPathNavigator subscore in subscores)
                    {
                        string dimension = subscore.SelectSingleNode("@scoreDimension").Value;
                        double score, maxScore;
                        if (!isSelected)
                        {
                            score = 0;
                        }
                        else
                        {
                            try
                            {
                                score = Convert.ToDouble(subscore.SelectSingleNode("@scorePoint").Value);
                                /*
                                maxScore = Convert.ToDouble(item.SelectSingleNode("@scoreMaxPoint"));
                                if (score > maxScore)
                                    throw new ScoringEngineException("scorePoint can't exceed scoreMaxPoint in " + response);
                                */
                            }
                            catch (Exception e)
                            {
                                throw new ScoringEngineException("Can't parse score value from response: " + response);
                            }
                        }
                        TestItemScoreInfo si = ti.GetScoreInfo(dimension);
                        if (si == null)
                            throw new ScoringEngineException("Item '" + itemName + "' does not have dimension '" + dimension + "'");
                        ItemScore ir = new ItemScore(ti, si, position, score, "", segmentID, isFT, isSelected, true, dropped);
                        // request from Larry: if item wasn't scored (e.g. grid or NL item scoring failed), score will be -1, just
                        // ignore this item but still compute overall scores (needed for starting ability for next opportunity).
                        // Include FT items so that we can get item counts right
                        // Include "treat as not presented" items to get DCAS-Alt attemptedness rules correct (treat-as-not-presented items count towards attemptedness)
                        if (score != -1 || isFT || ir.TreatAsNotPresented())
                            testScores.Add(ir);
                    }
                }
            }
            return testScores;
        }

        private List<ItemScore> ParseScoreString(string testName, string testScoreString, char rowSeparator, char colSeparator)
        {
            List<ItemScore> testScores = new List<ItemScore>();
            char[] responseSeparator = { rowSeparator };
            char[] innerSeparator = { colSeparator };
            if (testScoreString.Length > 0)
            {
                foreach (string response in testScoreString.Split(responseSeparator))
                {
                    string[] responseParts = response.Split(innerSeparator);
                    if (responseParts.Length < 2 || responseParts.Length > 4)
                        throw new ScoringEngineException("Can't parse response " + response);

                    string itemName = responseParts[0];
                    TestItem ti = tc.FindItem(testName, itemName);
                    if (ti == null)
                        throw new ScoringEngineException("No item '" + itemName + "' on test '" + testName + "'");
                    double score;
                    bool isSelected = true;
                    if (responseParts[1].Length == 0)
                    {
                        // this was added for DE-Alt: non-selected items will come as blank scores to the TestScoringEngine
                        score = 0;
                        isSelected = false;
                    }
                    else
                    {
                        try
                        {
                            score = Convert.ToDouble(responseParts[1]);
                        }
                        catch (Exception e)
                        {
                            throw new ScoringEngineException("Can't parse score value from response: " + response);
                        }
                    }

                    string dimension = "";
                    bool isFT = ti.IsFieldTest;
                    if (responseParts.Length == 3)
                    {
                        // either old item:score:dimension or new item:score:itemUse
                        string fieldTestIndicator = responseParts[2];
                        if (fieldTestIndicator == "OP")
                            isFT = false;
                        else if (fieldTestIndicator == "FT")
                            isFT = true;
                        else
                        {
                            // OK, must be dimension
                            dimension = responseParts[2];
                            // keep FT status from ItemBank
                        }
                    }
                    else if (responseParts.Length == 4)
                    {
                        string fieldTestIndicator = responseParts[2];
                        if (fieldTestIndicator == "OP")
                            isFT = false;
                        else if (fieldTestIndicator == "FT")
                            isFT = true;
                        else
                            throw new ScoringEngineException("Field test indicator must be either 'OP' or 'FT', not '" + fieldTestIndicator + "'");
                        dimension = responseParts[3];
                    }

                    TestItemScoreInfo si = ti.GetScoreInfo(dimension);
                    if (si == null)
                        throw new ScoringEngineException("Item '" + itemName + "' does not have dimension '" + dimension + "'");
                    ItemScore ir = new ItemScore(ti, si, score, isSelected, isFT);
                    // request from Larry: if item wasn't scored (e.g. grid or NL item scoring failed), score will be -1, just
                    // ignore this item but still compute overall scores (needed for starting ability for next opportunity).
                    // Include FT items so that we can get item counts right
                    // Include "treat as not presented" items to get DCAS-Alt attemptedness rules correct (treat-as-not-presented items count towards attemptedness)
                    if (score != -1 || isFT || ir.TreatAsNotPresented())
                    {
                        testScores.Add(ir);
                    }
                }
            }
            return testScores;
        }
    }
}
