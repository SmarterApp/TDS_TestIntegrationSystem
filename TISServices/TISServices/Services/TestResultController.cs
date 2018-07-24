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
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using TDSQASystemAPI.BL;
using TDSQASystemAPI.TestResults;
using TISServices.Extensions;
using TISServices.Utilities;
using OSS.TIS;
using TISServices.Authorization;

namespace TISServices.Services
{    
    [AuthorizeOpenAM]
    public class TestResultController : ApiController
    {
        public const string SCOREMODE_DEFAULT = "default";
        public const string SCOREMODE_VALIDATE = "validate";

        /// <summary>
        /// Receive a TDSReport from TDS in OSS format.  A callback URL is also provided (url encoded)
        /// which will be saved with the file and used to send a acknowledgement when TIS has processed the file.
        /// 
        /// REQUEST FORMAT:
        /// 
        /// URL: http://<hostname>?statusCallback={statusCallbackUriEncoded}&scoremode=validate
        /// Content-Type: application/xml
        /// Method:  POST
        /// Body: example
        /// <TDSReport>
        /// <Test name="(SBAC_PT)SBAC-Perf-MATH-11-Spring-2013-2015" subject="MATH" testID="SBAC-Perf-MATH-11" airbank="1" handscoreproject="" contract="SBAC_PT" mode="online" grade="11" assessmentType="" academicYear="" assessmentVersion="5644" />
        /// <Examinee airkey="8" >
        ///     <ExamineeAttribute context = "INITIAL" name = "Birthdate" value = "01012000" contextDate = "2014-11-07 16:12:19.432" />
        /// ...
        /// </TDSReport>
        /// </summary>
        /// <returns>
        /// HTTP status code and message as content if there's an error.
        /// CODES:
        /// 200: OK
        /// 400: request is not formatted correctly or does not contain the expected data.
        /// 500: an unhandled exception occurred while attempting to insert the file into the database
        /// </returns>
        [HttpPost]
        public HttpResponseMessage Submit(string statusCallback, string scoremode)
        {
            Uri callBackUrl = null;
            Statistics.AddToReceivedRequestCount();

            if (String.IsNullOrEmpty(statusCallback))
            {
                TISServicesLogger.Log("Empty statusCallback.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "This request is not properly formatted.");
            }
            try
            {
                // make sure it's a valid URI
                callBackUrl = new Uri(statusCallback);
            }
            catch
            {
                TISServicesLogger.Log(String.Format("statusCallback could not be converted to a Uri: {0}", statusCallback));
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Illegal statusCallback format.");
            }

            if (String.IsNullOrEmpty(scoremode))
            {
                scoremode = SCOREMODE_DEFAULT;
            }
            else
            {
                TISServicesLogger.Log(String.Format("received a scoremode {0} TRT submission, callback: {1}", scoremode, statusCallback));
            }

            // check to make sure it's a valid XML doc.  XmlRep.InsertXml would do this anyway if we passed a string,
            //  so best to do it here where we can check it.
            XmlDocument doc = null;
            try
            {
                //TODO: validate against XSD, or let TIS handle that?  For now, do not validate.
                doc = new XmlDocument();
                doc.Load(XmlReader.Create(Request.Content.ReadAsStreamAsync().Result));
            }
            catch (Exception ex)
            {
                TISServicesLogger.Log(ex);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Content format is not valid.");
            }

            // sanity check to make sure we're getting the right files
            //TODO: want to add any other validation?  Note that if we do decide to validate against the XSD above, this isn't needed.
            if (!doc.DocumentElement.Name.Equals("TDSReport"))
            {
                TISServicesLogger.Log(String.Format("Unexpected document element: {0}", doc.DocumentElement.Name));
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, String.Format("Invalid test result: {0}", doc.DocumentElement.Name));
            }

            try
            {
                new XmlRepository().InsertXml(XmlRepository.Location.source, doc, callBackUrl.AbsoluteUri, scoremode, new TestResultSerializerFactory());
                Statistics.AddToInsertedRequestCount();
            }
            catch (Exception ex)
            {
                TISServicesLogger.Log(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Could not persist the request.");
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}