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
using System.Text;
using System.Xml;
using System.Xml.XPath;

using TDSQASystemAPI.Config;
using ScoringEngine;
using ScoringEngine.ConfiguredTests;

namespace TDSQASystemAPI.TestResults
{
    internal class XMLAdapterITS : XMLAdapter
    {
        //<Result><Event><TestName>
        private static string _pathTestName = @"/Result/Event/TestName";
        private static string _pathTestSubject = @"/Result/Event/TestName";
        private static string _pathTestGrade = @"/Result/Event/TestName";

        //<Result><Candidate><StudentIdentifier>

        private static string _pathSSID = @"/Result/Candidate/StudentIdentifier";
        private static string _pathLastName = @"/Result/Candidate/LastName";
        private static string _pathFirstName = @"/Result/Candidate/FirstName";


        private static string _pathOppsOppID = @"/Result/Event/ResultID";
        private static string _pathOppsStartDate = @"/Result/Event/StartTime";
        private static string _pathOppsStatus = @"/Result/Event/Completed";
        //private static string _pathOppsStatusDate = @"/Result/Event/StartTime";
        //private static string _pathOppsItemCount = @"/tdsreport/opportunity/@itemcount";
        //private static string _pathOppsFTCount = @"/tdsreport/opportunity/@ftcount";
        //private static string _pathOppsAccomodations = @"/tdsreport/opportunity/accomodation";
        //private static string _pathOppsAccomodationType = "@Type";
        //private static string _pathOppsAccomodationValue = "@Value";

        //  <score subject="Algebraic Relationships" itemcount="4" scoredCount="4" rawscore="1" itsstrandKey="Math-3-AR"
        //    theta="0.570459667993595" thetase="1.16802125088779" scaled="205.704596679936" scaledse="11.6802125088779" /> 
        //scores 

        private static string _pathScores = @"/Result/Scores/Groups/Group";
        private static string _pathScoresSubject = @"Description";
        private static string _pathScoresItemCount = @"NumItems";
        private static string _pathScoresScoredCount = @"NumMarkedCorrect";
        private static string _pathScoresRawScore = @"RawScore";
        private static string _pathScoresTheta = @"RawScore";
        private static string _pathScoresThetaSE = @"AdaptiveStdError";
        private static string _pathScoresScaleScore = @"ScaledScore";
        private static string _pathScoresScaleScoreSE = @"AdaptiveStdError";
        //private static string _pathScoresStrandKey = "@itsstrandKey";
        private static string _pathScoresType = @"Type";



        internal XMLAdapterITS(string xmlString)
            : base(xmlString)
        {
        }

        internal override void AddScoreToXML(Score score)
        {
        }

        public override TestResult TestFromXML(TestCollection tc, out bool isValid, bool loadBlueprint)
        {
            string name, subject, grade;
            bool allgood = true;
            ValidateResult vr = ValidateResult.Unknown;
            vr = Value(_pathTestName, out name);
            allgood = (vr != ValidateResult.Valid) ? false : allgood;
            vr = Value(_pathTestSubject, out subject);
            allgood = (vr != ValidateResult.Valid) ? false : allgood;
            vr = Value(_pathTestGrade, out grade);
            allgood = (vr != ValidateResult.Valid) ? false : allgood;
            //vr = Value(_pathTestItemBank, out itemBank);
            //allgood = (vr != ValidateResult.Valid) ? false : allgood;

            TestBlueprint blueprint = tc.GetBlueprint(name);
            if (blueprint == null)
            {
                vr = ValidateResult.UnknownTest;
                allgood = false;
                AddValidationRecord(ValidationRecord.ValidationType.Semantic, vr, "", "no blueprint available for test name " + name);
            }

            TestResult tst = null;
            if (allgood)
            {
                tst = new TestResult(blueprint, name, subject, grade, 1, 0, string.Empty, -1);//its bank key 1 ***Anil

                Testee testee = TesteeFromXML();
                if (testee != null) tst.Testee = testee;
                else allgood = false;
            }

            //if (allgood)
            //{
            //    List<ItemResponse> responses = ItemResponsesFromXML(blueprint);
            //    if (responses != null) tst.ItemResponses = responses;
            //    else allgood = false;
            //}

            if (allgood)
            {
                Opportunity opp = OpportunityFromXML();
                if (opp != null) tst.Opportunity = opp;
            }

            if (allgood)
            {
                Scores scores = ScoresFromXML(blueprint);
                if (scores != null) tst.Scores = scores;
            }
            isValid = allgood;
            if (allgood) return tst;
            return null;
        }

        internal override Opportunity OpportunityFromXML()
        {
            string oppid, status, formID = string.Empty;
            int itemcount = 0, ftcount = 0, gracePeriodRestarts = 0, abnormalStarts = 0;
            DateTime startdate;
            bool allgood = true;
            ValidateResult vr = ValidateResult.Unknown;

            vr = Value(_pathOppsOppID, out oppid);
            allgood = (vr != ValidateResult.Valid) ? false : allgood;

            vr = Value(_pathOppsStartDate, out startdate);
            allgood = (vr != ValidateResult.Valid) ? false : allgood;
            //vr = Value(_pathOppsOpportunity, out opportunity);
            //allgood = (vr != ValidateResult.Valid) ? false : allgood;
            vr = Value(_pathOppsStatus, out status);
            allgood = (vr != ValidateResult.Valid) ? false : allgood;
            //vr = Value(_pathOppsStatusDate, out statusDate);
            //allgood = (vr != ValidateResult.Valid) ? false : allgood;
            //vr = Value(_pathOppsPauseCount, out pausecount);
            //allgood = (vr != ValidateResult.Valid) ? false : allgood;
            //vr = Value(_pathOppsItemCount, out itemcount);
            //allgood = (vr != ValidateResult.Valid) ? false : allgood;
            //vr = Value(_pathOppsFTCount, out ftcount);
            //allgood = (vr != ValidateResult.Valid) ? false : allgood;

            if (allgood)
            {
				Opportunity opp = new Opportunity(oppid, startdate, 1, status, startdate, 1, itemcount, ftcount, DateTime.Now, formID, gracePeriodRestarts, abnormalStarts, DateTime.Now, string.Empty, string.Empty);
                //XmlNodeList nodes = _xmlDoc.DocumentElement.SelectNodes(_pathOppsAccomodations);
                //for (int i = 0; i < nodes.Count; i++)
                //{
                //    string type, value;
                //    vr = Value(nodes[i], _pathOppsAccomodationType, out type);
                //    allgood = (vr != ValidateResult.Valid) ? false : allgood;
                //    vr = Value(nodes[i], _pathOppsAccomodationValue, out value);
                //    allgood = (vr != ValidateResult.Valid) ? false : allgood;
                //    if (allgood)
                //    {
                //        opp.AddAccomodation(type, value);
                //    }
                //}

                return opp;
            }
            return null;
        }


        internal override Testee TesteeFromXML()
        {
            string ssid, firstname, lastname;
            long entityKey = -1;

            bool allgood = true;
            ValidateResult vr = ValidateResult.Unknown;
            vr = Value(_pathSSID, out ssid);
            allgood = (vr != ValidateResult.Valid) ? false : allgood;
            vr = Value(_pathFirstName, out firstname);
            allgood = (vr != ValidateResult.Valid) ? false : allgood;
            vr = Value(_pathLastName, out lastname);
            allgood = (vr != ValidateResult.Valid) ? false : allgood;
            //vr = Value(_pathEntityKey, out entityKey);
            //allgood = (vr != ValidateResult.Valid) ? false : allgood;

            if (allgood)
            {
                Testee testee = new Testee(ssid, entityKey, firstname, lastname);
                return testee;
            }
            return null;
        }


        internal override Scores ScoresFromXML(TestBlueprint blueprint)
        {
            int itemCount, scoredCount;
            string strand, type;
            double rawscore, theta, scaleScore, thetaSE, scaleScoreSE;

            ValidateResult vr = ValidateResult.Unknown;
            bool allgood = true;
            Scores lstScores = new Scores();
            XmlNodeList nodes = _xmlDoc.SelectNodes(_pathScores);

            //todo: check -- will we always have a score?
			//TODO: Writing Field Test will not have a score
            if (nodes.Count == 0)
            {
                vr = ValidateResult.NotFound;
                AddValidationRecord(ValidationRecord.ValidationType.Syntactic, vr, _pathScores, "No items found or missing item tag in xml");
                allgood = false;
            }
            if (!allgood) return null;

            for (int i = 0; i < nodes.Count; i++)
            {
                vr = Value(nodes[i], _pathScoresSubject, out strand);
                allgood = (vr != ValidateResult.Valid) ? false : allgood;
                vr = Value(nodes[i], _pathScoresItemCount, out itemCount);
                allgood = (vr != ValidateResult.Valid) ? false : allgood;
                vr = Value(nodes[i], _pathScoresScoredCount, out scoredCount);
                allgood = (vr != ValidateResult.Valid) ? false : allgood;
                vr = Value(nodes[i], _pathScoresRawScore, out rawscore);
                allgood = (vr != ValidateResult.Valid) ? false : allgood;
                vr = Value(nodes[i], _pathScoresTheta, out theta);
                allgood = (vr != ValidateResult.Valid) ? false : allgood;
                vr = Value(nodes[i], _pathScoresThetaSE, out thetaSE);
                allgood = (vr != ValidateResult.Valid) ? false : allgood;
                vr = Value(nodes[i], _pathScoresScaleScore, out scaleScore);
                allgood = (vr != ValidateResult.Valid) ? false : allgood;
                vr = Value(nodes[i], _pathScoresScaleScoreSE, out scaleScoreSE);
                allgood = (vr != ValidateResult.Valid) ? false : allgood;
                //vr = Value(nodes[i], _pathScoresStrandKey, out scaleStrankKey);
                //allgood = (vr != ValidateResult.Valid) ? false : allgood;
                vr = Value(nodes[i], _pathScoresType, out type);
                allgood = (vr != ValidateResult.Valid) ? false : allgood;

                if (strand != "Overall")
                {
                    if (type.Equals(string.Empty) || type != "0")
                    {
                        if (!blueprint.HasFeature("Strand", strand))
                        {
                            AddValidationRecord(ValidationRecord.ValidationType.Semantic, ValidateResult.ScoreForUnknownFeature, "Recieved score for unknown feature: " + strand);
                            allgood = false;
                        }
                    }
                }

                if (allgood)
                {
                    Score score = new Score((strand == "Overall") ? null : "Strand", (strand == "Overall") ? null : strand,
                                            itemCount, scoredCount, rawscore, theta, thetaSE, scaleScore, scaleScoreSE, string.Empty, -1);
                    lstScores.Add(score);
                }
            }
            if (allgood) return lstScores;
            return null;
        }

        internal override List<ItemResponse> ItemResponsesFromXML(TestBlueprint blueprint, string subject)
        {
            return null;//*** Anil
        }

        //internal override List<ItemResponse> ItemResponsesFromXML(TestBlueprint blueprint)
        //{
        //    int position, responseTime;
        //    int operational;
        //    double scorepoints, score;
        //    string response, answerkey, comment, clientid, strand;
        //    long itemName;
        //    DateTime adminDate, responseDate;

        //    ValidateResult vr = ValidateResult.Unknown;
        //    bool allgood = true;
        //    List<ItemResponse> lstResponses = new List<ItemResponse>();
        //    XmlNodeList nodes = _xmlDoc.SelectNodes(_pathItems);

        //    if (nodes.Count == 0)
        //    {
        //        vr = ValidateResult.NotFound;
        //        AddValidationRecord(ValidationRecord.ValidationType.Syntactic, vr, _pathItems, "No items found or missing item tag in xml");
        //        allgood = false;
        //    }
        //    if (!allgood) return null;

        //    for (int i = 0; i < nodes.Count; i++)
        //    {
        //        vr = Value(nodes[i], _pathItemPosition, out position);
        //        allgood = (vr != ValidateResult.Valid) ? false : allgood;
        //        vr = Value(nodes[i], _pathItemItemName, out itemName);
        //        allgood = (vr != ValidateResult.Valid) ? false : allgood;
        //        vr = Value(nodes[i], _pathItemOperational, out operational);
        //        allgood = (vr != ValidateResult.Valid) ? false : allgood;
        //        vr = Value(nodes[i], _pathItemScorepoints, out scorepoints);
        //        allgood = (vr != ValidateResult.Valid) ? false : allgood;
        //        vr = Value(nodes[i], _pathItemScore, out score);
        //        allgood = (vr != ValidateResult.Valid) ? false : allgood;
        //        vr = Value(nodes[i], _pathItemResponse, out response);
        //        allgood = (vr != ValidateResult.Valid) ? false : allgood;
        //        vr = Value(nodes[i], _pathItemAnswerKey, out answerkey);
        //        allgood = (vr != ValidateResult.Valid) ? false : allgood;
        //        vr = Value(nodes[i], _pathItemAdminDate, out adminDate);
        //        allgood = (vr != ValidateResult.Valid) ? false : allgood;
        //        vr = Value(nodes[i], _pathItemResponseDate, out responseDate);
        //        allgood = (vr != ValidateResult.Valid) ? false : allgood;
        //        vr = Value(nodes[i], _pathItemResponseTime, out responseTime);
        //        allgood = (vr != ValidateResult.Valid) ? false : allgood;
        //        vr = Value(nodes[i], _pathItemComment, out comment);
        //        allgood = (vr != ValidateResult.Valid) ? false : allgood;
        //        vr = Value(nodes[i], _pathItemClientID, out clientid);
        //        allgood = (vr != ValidateResult.Valid) ? false : allgood;
        //        vr = Value(nodes[i], _pathItemStrand, out strand);
        //        allgood = (vr != ValidateResult.Valid) ? false : allgood;

        //        TestItem item = blueprint.GetItem(itemName);
        //        if (item == null)
        //        {
        //            allgood = false;
        //            AddValidationRecord(ValidationRecord.ValidationType.Semantic, ValidateResult.UnknownItem, "", "Item named in result xml not found: " + itemName);
        //        }
        //        else
        //        {
        //            ItemResponse ir = new ItemResponse(item, position, itemName, clientid, operational, scorepoints, score, response, answerkey,
        //                                                adminDate, responseDate, comment);
        //            lstResponses.Add(ir);
        //        }
        //    }

        //    if (allgood) return lstResponses;
        //    else return null;
        //}

    }
}
