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
using System.Threading.Tasks;
using TDSQASystemAPI;
using System.Net.Http;
using System.IO;
using TDSQASystemAPI.Utilities;
using TDSQASystemAPI.ExceptionHandling;

namespace TDSQASystemAPI.Acknowledgement
{
    public class RESTAcknowledgementTarget : IAcknowledgementTarget
    {
        #region IAcknowledgementTarget Members

        public bool Send(Message ack, TDSQASystemAPI.Data.XmlRepositoryItem item)
        {
            if (String.IsNullOrEmpty(item.CallbackURL))
                return false;

            try
            {
                string ser = Serialization.SerializeJson<Message>(ack);
                using (HttpClient client = new HttpClient())
                {
                    using (StringContent content = new StringContent(ser, Encoding.UTF8, "application/json"))
                    {
                        HttpResponseMessage response = client.PostAsync(item.CallbackURL, content).Result;
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            throw new QAException(String.Format("Error posting acknowlegement for opp: {0} to {1}.  Status Code: {2}, Result: {3}", ack.oppKey, item.CallbackURL, response.StatusCode, response.Content.ReadAsStringAsync().Result), QAException.ExceptionType.General);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new QAException(String.Format("Error sending acknowledgement: {0}", ex.Message), QAException.ExceptionType.General, ex);
            }

            return true;
        }

        #endregion
    }
}
