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
    public class ParameterValue
    {
        int parameterPosition;
        public int ParameterPosition
        {
            get
            {
                return parameterPosition;
            }
        }

        string indexType;
        public string IndexType
        {
            get
            {
                return indexType;
            }
        }

        string index;
        public string Index
        {
            get
            {
                return index;
            }
        }

        string type;
        public string Type
        {
            get
            {
                return type;
            }
        }

        string value;
        public string Value
        {
            get
            {
                return value;
            }
        }

        public ParameterValue(int parameterPosition, string indexType, string index, string type, string value)
        {
            if (indexType != "" && indexType.ToLower() != "int" && indexType.ToLower() != "string")
                throw new ScoringEngineException("IndexType must be empty or 'int' or 'string', not '" + indexType + "'");
            this.parameterPosition = parameterPosition;
            this.indexType = indexType;
            this.index = index;
            this.type = type;
            this.value = value;
        }
    }
}
