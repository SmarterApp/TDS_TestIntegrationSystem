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

namespace TDSQASystemAPI.Data
{
    //TODO: make oppID long
    //TODO: migrate to oppKey; may just want to add it as a Nullable<Guid>
    //TODO: handle archive strategy here, possibly even with insert/archive capability and maintenance of arvhive fileID?
    public struct XmlRepositoryItem
    {
        public long FileID;
        public string OppID;
        public long TesteeKey;
        public Guid? SenderGUID;
        public string CallbackURL;
        public string ScoreMode;


        public XmlRepositoryItem(long FileID, string OppID, long testeeKey, Guid? senderGUID, string callbackURL, string scoreMode) 
        {
            this.FileID = FileID;
            this.OppID = OppID;
            this.TesteeKey = testeeKey;
            this.SenderGUID = senderGUID;
            this.CallbackURL = callbackURL;
            this.ScoreMode = scoreMode;
        }
    }
}
