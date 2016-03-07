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

namespace TDSQASystemAPI.Config
{
    public class MetaDataEntry
    {
        //note that these are not all the group names. There are configurable handscoring groups in the config file
        public enum GroupName { All = 0, DoR, RB, HandScoring, None }

        private int _projectID;
        private string _group;
        private string _variable;
        private double _floatVal;
        private int _intVal;
        private string _textVal;

        public MetaDataEntry(int projectID, string group, string var, double floatVal, int intVal, string textVal)
        {
            _projectID = projectID;
            _group = group;
            _variable = var;
            _floatVal = floatVal;
            _intVal = intVal;
            _textVal = textVal;
        }

        #region properties

        public int ProjectID
        {
            get
            {
                return _projectID;
            }
        }

        public string Group
        {
            get
            {
                return _group;
            }
        }
        public string Variable
        {
            get
            {
                return _variable;
            }
        }

        public string TextVal
        {
            get
            {
                return _textVal;
            }
        }

        public int IntVal
        {
            get
            {
                return _intVal;
            }
        }

        public double FloatVal
        {
            get
            {
                return _floatVal;
            }
        }


        #endregion properties



    }
}
