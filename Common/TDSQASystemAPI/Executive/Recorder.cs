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

using TDSQASystemAPI.Config;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.Utilities;
using TDSQASystemAPI.TestResults;
using TDSQASystemAPI.ExceptionHandling;


namespace TDSQASystemAPI.Executive
{
    internal static class Recorder
    {

        internal static void RecordTestResult(string dbHandle, TestResult tr, string xml)
        {
            string subdirectory;
            FileSaver.FileSaveStatus fileSaveStatus = SaveXMLFile(tr, xml, out subdirectory);

            ResultsDB db = new ResultsDB(dbHandle);
            if ((fileSaveStatus == FileSaver.FileSaveStatus.SaveFailed) || (fileSaveStatus == FileSaver.FileSaveStatus.SavedToBackup))
            {
                db.RecordSaveException(tr, fileSaveStatus);
                Alerter alerter = new Alerter();
                alerter.RaiseAlert("Unable to save XML file to disk. Status: " + fileSaveStatus.ToString(), Alerter.AlertType.FileSaveError);
            }

            ResultsDB.MethodStatus status = db.SaveReceiptInfo(tr, string.Format("{0}\\{1}",subdirectory,tr.XMLFileName));
            if (status == ResultsDB.MethodStatus.Fail)
            {
                Alerter alerter = new Alerter();
                alerter.RaiseAlert(string.Format("Unable to save receipt information to database. Entity: {0}, Test: {1}, Opp: {2}", 
                                                  tr.Testee.EntityKey.ToString(),tr.Name,tr.Opportunity.OpportunityNumber),
                                   Alerter.AlertType.DatabaseAccessError);
            }

            else db.SaveArchiveInfo(tr);
        }


        internal static FileSaver.FileSaveStatus SaveXMLFile(TestResult tr, string xml, out string subdirectory)
        {
            MetaDataEntry primaryPathEntry = ConfigurationHolder.GetFromMetaData(tr.ProjectID,"DataStorage", "PrimaryDirectory");
            MetaDataEntry backupPathEntry = ConfigurationHolder.GetFromMetaData(tr.ProjectID, "DataStorage", "SecondaryDirectory");

            if ((primaryPathEntry == null) || (backupPathEntry==null))
                throw new QAException("Primary and/or secondary xml save path not specified under DataStorage in ProjectMetaData table",QAException.ExceptionType.ConfigurationError);

            FileSaver fileSave = new FileSaver(primaryPathEntry.TextVal, backupPathEntry.TextVal);

            subdirectory = tr.Opportunity.StartDate.Year.ToString() + "-" + tr.Opportunity.StartDate.DayOfYear.ToString();
            FileSaver.FileSaveStatus status = fileSave.SaveFile(tr.XMLFileName,subdirectory, xml);

            Alerter alerter = new Alerter();
            switch (status)
            {
                case FileSaver.FileSaveStatus.SaveFailed:
                    alerter.RaiseAlert(string.Format("Unable to save XML file. Base path: {0}, Backup fail path: {1}", 
                                       primaryPathEntry.TextVal, backupPathEntry.TextVal), Alerter.AlertType.FileSaveError);
                    break;
                case FileSaver.FileSaveStatus.SavedToBackup:
                    alerter.RaiseAlert(string.Format("Unable to access primary storage ({0}). Had to save to backup location", primaryPathEntry.TextVal), 
                                       Alerter.AlertType.FileSaveError);
                    break;
            }
            return status;
        }


        internal static void RecordValidationFailures(string _dbHandleResults, TestResult tr, List<ValidationRecord> records)
        {
            ResultsDB db = new ResultsDB(_dbHandleResults);
            try
            {
                foreach (ValidationRecord vr in records)
                {
                    db.SaveValidationException(tr, vr);
                }
            }
            catch
            {
                Alerter alerter = new Alerter();
                alerter.RaiseAlert("Unable to save validation records to database", Alerter.AlertType.DatabaseAccessError);
            }
        }
    }//end class
}//end namespace
