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
using System.Data;

namespace TDSQASystemAPI.BL
{
    public class Sandlot
    {
        private DAL.Sandlot sandlotDAL;

        public Sandlot()
        {
            sandlotDAL = new TDSQASystemAPI.DAL.Sandlot();
        }

        /// <summary>
        /// Returns the sandlot settings for a given environment and short name
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public DataRow GetSandlotSettings(string environment, string shortName)
        {
            return sandlotDAL.GetSandlotSettings(environment, shortName);
        }
    }
}
