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
using TDSQASystemAPI.Utilities;
using System.Diagnostics;
using System.Timers;
using System.Configuration;
using System.IO;
using System.Threading;
using TDSQASystemAPI.Data;
using TDSQASystemAPI.ExceptionHandling;
using System.Globalization;
using System.Xml;

namespace TDSQASystemAPI
{
	public class QASystemWorkerThread
	{
        /// <summary>
		/// The path of the file this thread is responsible for.
		/// </summary>
		private XmlRepositoryItem xmlRepositoryItem;
		/// <summary>
		/// Whether this worker thread is handling a file that is part of a special pilot testing group.
		/// </summary>
		private bool specialCase = false;

		/// <summary>
		/// Initialize the file name.
		/// </summary>
		/// <param name="file"></param>
        public QASystemWorkerThread(XmlRepositoryItem xmlRepositoryItem)
		{
            this.xmlRepositoryItem = xmlRepositoryItem;
		}

		/// <summary>
		/// 
		/// </summary>
		public void ProcessXmlFile(object qaSystemMainThread)
		{
            try
            {
                QASystem qa = new QASystem("TDSQC", "TDSQC");

                XmlDocument xml = null;
                try
                {
                    xml = new BL.XmlRepository(QASystemConfigSettings.Instance.LongDbCommandTimeout).ProcessXmlFile(xmlRepositoryItem.FileID);
                }
                catch (Exception ex)
                {
                    throw new QAException(String.Format("Error reading XML file for FileID: {0} from XmlRepository: {1}", xmlRepositoryItem.FileID, ex.Message), QAException.ExceptionType.General);
                }

                if (xml == null)
                {
                    // must have been a race condition with another worker thread.
                    //  Just log and skip.
                    Utilities.Logger.Log(true, String.Format("FileID: {0} for OppID: {1}, TesteeKey: {2} was not available to be processed.  Skipping...", xmlRepositoryItem.FileID, xmlRepositoryItem.OppID, xmlRepositoryItem.TesteeKey), EventLogEntryType.Information, false, true);
                }
                else
                {
                    // Process file...
                    qa.ReceiveTestResult(xml, xmlRepositoryItem);
                    ((QASystemMainThread)qaSystemMainThread).filesPerSecond.Increment();
                    //((QASystemMainThread)qaSystemMainThread).totalFilesProcessedToday.Increment();
                }
            }
            catch (QAException ex)
            {
                Logger.Log(true, "Processing file " + xmlRepositoryItem.FileID + " failed because " + ex.Message + ex.StackTrace, EventLogEntryType.Error, true, true);
            }
            catch (Exception e)
            {
                Logger.Log(true, "General Exception for file " + xmlRepositoryItem.FileID + ": " + e.Message + e.StackTrace, EventLogEntryType.Error, true, true);
            }
			finally
			{
				if (qaSystemMainThread != null)
				{
					((QASystemMainThread)qaSystemMainThread).DecrementCounter();
                    ((QASystemMainThread)qaSystemMainThread).UnlockOpp(xmlRepositoryItem);
				}
				else
				{
					throw new QAException(Messages.NullParameterProvided + qaSystemMainThread, QAException.ExceptionType.General);
				}
			}
		}
	}
}
