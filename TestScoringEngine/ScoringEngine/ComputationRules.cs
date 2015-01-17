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

        public Dictionary<string, Dictionary<string, MeasureValue>> ApplyComputationRules(string testName, string enrolledGrade, Dictionary<string, Dictionary<string, MeasureValue>> measuresIn, List<ItemScore> testScores, DateTime testDate, string status, string form, DateTime forceCompletionDate, TestMode mode, List<TestAccomodation> TDSAccomms, List<TestAccomodation> ARTAccoms)
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
                            rules = new SpecificComputationRules(testName, status, enrolledGrade, form, measuresIn, testScores, testDate, tc, forceCompletionDate, mode, TDSAccomms, ARTAccoms);
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
    }
}
