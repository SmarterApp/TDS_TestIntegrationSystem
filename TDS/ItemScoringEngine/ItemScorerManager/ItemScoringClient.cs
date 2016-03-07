/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using AIR.Common.Threading;
using TDS.ItemScoringEngine.Web;

namespace TDS.ItemScoringEngine
{
    /// <summary>
    /// A thread safe class used by clients for sending score requests to a scoring web server.
    /// </summary>
    public class ItemScoringClient : ItemScoringOnline
    {              
        public ItemScoringClient() : base()
        {
            
        }

        public ItemScoringClient(BoundedThreadPool threadPool) : base(threadPool)
        {
            
        }

        /// <summary>
        /// Sends a scoring request and waits for the response.
        /// </summary>
        /// <param name="scoreRequest">The score request object with response info in it.</param>
        public ItemScoreResponse SendRequest(string url, ItemScoreRequest scoreRequest)
        {
            ItemScoreResponse scoreResponse = HttpWebHelper.SendAndReadXml<ItemScoreRequest, ItemScoreResponse>(url, scoreRequest);
            Log("ItemScoringClient SendRequest");
            return scoreResponse;
        }

        /// <summary>
        /// Send a score request and do not wait for a response.
        /// </summary>
        /// <param name="scoreRequest">The score request object with response info in it.</param>
        /// <param name="completedCallback">A function to be called once the score request is recieved by the scoring server.</param>
        public void SendRequestAsync(string url, ItemScoreRequest scoreRequest, Action<ItemScoreResponse> completedCallback)
        {
            _threadPool.Enqueue(() => SendRequest(url, scoreRequest));
        }
    }

}