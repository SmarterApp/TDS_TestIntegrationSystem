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

namespace ScoringEngine.ConfiguredTests
{
    public class ItemScore
    {
        TestItem ti;
        TestItemScoreInfo si;
        double score;
        string conditionCode = "";
        bool isSelected;
        bool isAttempted = true;
        bool isFieldTest = true;
        bool isDropped = false;
        int position = -1;
        string segmentID = "";

        public TestItem Item
        {
            get
            {
                return ti;
            }
        }

        public TestItemScoreInfo ScoreInfo
        {
            get
            {
                return si;
            }
        }

        public double Score
        {
            get
            {
                return score;
            }
        }

        public string ConditionCode
        {
            get
            {
                return conditionCode;
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
        }

        public bool IsAttempted
        {
            get
            {
                return isAttempted;
            }
        }

        /// <summary>
        /// Whether this was a field test item at the time it was presented. Note this might be different from ItemScore.TestItem.IsFieldTest.
        /// </summary>
        public bool IsFieldTest
        {
            get
            {
                return isFieldTest;
            }
        }

        /// <summary>
        /// Whether this response was dropped after it was administered to the student.
        /// </summary>
        public bool IsDropped
        {
            get
            {
                return isDropped;
            }
        }

        public int Position
        {
            get
            {
                return position;
            }
        }

        public string SegmentID
        {
            get
            {
                return segmentID;
            }
        }

        public ItemScore(TestItem ti, TestItemScoreInfo si, double score, bool isSelected, bool isFT)
        {
            this.ti = ti;
            this.si = si;
            this.score = score;
            this.isSelected = isSelected;
            this.isFieldTest = isFT;
        }

        public ItemScore(TestItem ti, TestItemScoreInfo si, double score, string conditionCode, bool isFT, bool isSelected, bool isAttempted)
        {
            this.ti = ti;
            this.si = si;
            this.score = score;
            this.conditionCode = conditionCode;
            this.isSelected = isSelected;
            this.isAttempted = isAttempted;
            this.isFieldTest = isFT;
        }

        public ItemScore(TestItem ti, TestItemScoreInfo si, int position, double score, string conditionCode, string segmentID, bool isFT, bool isSelected, bool isAttempted, bool isDropped)
        {
            this.ti = ti;
            this.si = si;
            this.position = position;
            this.score = score;
            this.conditionCode = conditionCode;
            this.segmentID = segmentID;
            this.isSelected = isSelected;
            this.isAttempted = isAttempted;
            this.isFieldTest = isFT;
            this.isDropped = isDropped;
        }

        // Check if there is some recode rule that causes this score to be treated as not presented
        // e.g. ELPA condition code A
        public bool TreatAsNotPresented()
        {
            if (si.RecodeRuleName.StartsWith("GRR("))
                return false;
            switch (si.RecodeRuleName)
            {
                case "":
                case "Utah_Writing_ER_Recode":
                    return false;
                case "ELPA_CC_Rule":
                case "ELPA_Illocution_Recode":
                case "ELPA_Grammar_Recode":
                    if (conditionCode == "A") return true;
                    return false;
                case "DCAS_Alt_EI_Recode":
                case "DCAS_Alt_PresentedRule":
                    if (score == -1) return true;
                    return false;
                default:
                    throw new Exception("Item " + ti.ItemName + " has an unknown recode rule: " + si.RecodeRuleName);
            }
        }
    }
}
