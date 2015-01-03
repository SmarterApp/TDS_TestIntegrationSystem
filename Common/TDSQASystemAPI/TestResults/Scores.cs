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

namespace TDSQASystemAPI.TestResults
{
    public class Scores
    {
        Dictionary<string, Dictionary<string, Score>> scores = null;

        public Dictionary<string, Dictionary<string, Score>> TestScores
        {
            get
            {
                return scores;
            }
        }

        public Scores() : this(null)
        {
        }

        public Scores(List<Score> scoreList)
        {
            scores = new Dictionary<string, Dictionary<string, Score>>();
            if (scoreList != null)
                foreach (Score s in scoreList)
                    Add(s);
        }

        public void Add(Score score)
        {
            if (scores == null)
                scores = new Dictionary<string, Dictionary<string, Score>>();
            if (!scores.ContainsKey(score.MeasureOf))
                scores[score.MeasureOf] = new Dictionary<string, Score>();
            scores[score.MeasureOf][score.MeasureLabel] = score;
        }

        public bool AddIfNew(Score score)
        {
            if (scores == null)
                scores = new Dictionary<string, Dictionary<string, Score>>();
            if (!scores.ContainsKey(score.MeasureOf))
                scores[score.MeasureOf] = new Dictionary<string, Score>();
            if (scores[score.MeasureOf].ContainsKey(score.MeasureLabel))
            {
                return false;
            }
            else
            {
                scores[score.MeasureOf][score.MeasureLabel] = score;
                return true;
            }
        }

        /// <summary>
        /// Return a Score if found in the collection and if the value is not empty
        /// </summary>
        /// <param name="measureOf"></param>
        /// <param name="measureLabel"></param>
        /// <returns></returns>
        public Score GetScore(string measureOf, string measureLabel)
        {
            if (TestScores == null)
                return null;

            Score s = null;

            if (TestScores.ContainsKey(measureOf)
                && TestScores[measureOf].ContainsKey(measureLabel))
            {
                if (!String.IsNullOrEmpty(TestScores[measureOf][measureLabel].MeasureValue))
                    s = TestScores[measureOf][measureLabel];
            }

            return s;
        }
    }
}
