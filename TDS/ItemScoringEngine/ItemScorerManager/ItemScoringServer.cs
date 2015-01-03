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
using TDS.ItemScoringEngine.Web;

namespace TDS.ItemScoringEngine
{
    /// <summary>
    /// A thread safe class used by the server to send score responses back to the client server.
    /// </summary>
    public class ItemScoringServer : ItemScoringOnline
    {

        public void SendResponse(string url, ItemScoreResponse scoreResponse)
        {
            HttpWebHelper.SendXml(url, scoreResponse);
            Log("ItemScoringServer SendResponse");
        }

        public void SendResponse(string url, ItemScoreResponse scoreResponse, StringBuilder acknowledgement)
        {
            HttpWebHelper.SendXml(url, scoreResponse, acknowledgement);            
            Log("ItemScoringServer SendResponse");
        }

        public void SendResponseAsync(string url, ItemScoreResponse scoreResponse)
        {
            _threadPool.Enqueue(() => SendResponse(url, scoreResponse));
        }

    }
}
