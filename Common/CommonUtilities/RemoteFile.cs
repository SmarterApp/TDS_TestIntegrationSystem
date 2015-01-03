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
using System.Text.RegularExpressions;
using System.Globalization;

namespace CommonUtilities
{
    /// <summary>
    /// Exposes properties of files on an FTP server.
    /// </summary>
    public class RemoteFile
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            protected set
            {
                name = value;
            }
        }

        private long bytes;
        public long Bytes
        {
            get
            {
                return bytes;
            }
            protected set
            {
                bytes = value;
            }
        }

        private DateTime createDate;
        public DateTime CreateDate
        {
            get
            {
                return createDate;
            }
            protected set
            {
                createDate = value;
            }
        }

        private bool isDirectory;
        public bool IsDirectory
        {
            get
            {
                return isDirectory;
            }
            protected set
            {
                isDirectory = value;
            }
        }

        private string rawDetails;
        public string RawDetails
        {
            get
            {
                return rawDetails;
            }
        }

        public static RemoteFile Create(bool isDirectory, DateTime lastWriteDateTime, string name, long bytes)
        {
            return new RemoteFile(isDirectory, lastWriteDateTime, name, bytes);
        }

        public static RemoteFile Create(string lineToParse)
        {
            RemoteFile file = null;

            int firstSpace = lineToParse.IndexOf(" ");
            DateTime testDate;
            if (firstSpace > 0 && DateTime.TryParse(lineToParse.Substring(0, firstSpace), out testDate))
            {
                file = new WindowsRemoteFile(lineToParse);
            }
            else if (lineToParse.StartsWith("-") || lineToParse.StartsWith("d"))
            {
                file = new UnixRemoteFile(lineToParse);
            }

            return file;
        }

        private RemoteFile(bool isDirectory, DateTime lastWriteDateTime, string name, long bytes)
        {
            this.rawDetails = String.Empty;
            this.isDirectory = isDirectory;
            this.createDate = lastWriteDateTime;
            this.name = name;
            this.bytes = bytes;
        }

        protected RemoteFile(string lineToParse)
        {
            this.rawDetails = lineToParse;
        }
    }

    public class WindowsRemoteFile : RemoteFile
    {
        public WindowsRemoteFile(string lineToParse)
            : base(lineToParse)
        {
            //Windows:
            //02-14-11  04:07PM                   50 ProductionPaperPencilErrorResolution.txt
            //02-15-11  03:32PM                   <DIR> testdir
            //
            // safe to split this into tokens.  Only the file name can have spaces, and that's 
            //  always the last element
            string[] toks = lineToParse.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            CreateDate = DateTime.Parse(String.Format("{0} {1}", toks[0], toks[1]));
            IsDirectory = toks[2].Equals("<DIR>", StringComparison.InvariantCultureIgnoreCase);
            Bytes = IsDirectory ? 0 : long.Parse(toks[2]);

            // in case there are spaces in the file name, start with the position
            //  after the bytes/<DIR> and concat any remaining toks.
            StringBuilder fileNameSB = new StringBuilder();
            for (int i = 3; i < toks.Length; i++)
            {
                fileNameSB.Append(toks[i]);
            }
            Name = fileNameSB.ToString();
        }
    }

    public class UnixRemoteFile : RemoteFile
    {
        public UnixRemoteFile(string lineToParse)
            : base(lineToParse)
        {
            //Unix/Linux:
            //drwxr-x---  2 air      System            0 Mar 03 14:06 .
            //-rwxr-x---  1 air      System          256 Feb 23 10:41 Sample_ProductionPaperPencilErrorCorrection-02232011104129.txt
            //-rwxr-x---  1 air      System          129 May 02  2011 ProductionPaperPencilErrorCorrection-04302011092129.txt
            //-rwxr-x---  1 air      System          256 Feb  3 10:41:03 2011 Sample_ProductionPaperPencilErrorCorrection-02232011104129.txt
            //----------   0                       16492 Dec  1  5:43:19 2011 ProductionPaperPencilErrorCorrection2012_20111130_1724.TAB
            // if it's a directory, the line starts with a "d"
            IsDirectory = lineToParse.StartsWith("d", StringComparison.InvariantCultureIgnoreCase);

            // not sure if owner group can have spaces, so splitting the entire line into toks to get the create date
            //  is not safe.  Use a regex to find the start of the date.
            //AM: got more complicated with our own SFTP sites, which have the time and the year.
            //  Broke the regex up into parts.
            //Match dateStart = Regex.Match(lineToParse, " (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)([ ]+)([1-9]|0[1-9]|[12][0-9]|3[01])([ ]+)((([0-5]|)[0-9]:([0-5]|)[0-9]((:([0-5]|)[0-9])|))|((19|20)[0-9][0-9])) ", RegexOptions.IgnoreCase);
            string regExMonth = "(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)";
            string regExDay = "([1-9]|0[1-9]|[12][0-9]|3[01])";
            string regExTime = "(([0-5]|)[0-9]:([0-5]|)[0-9]((:([0-5]|)[0-9])|))";
            string regExYear = "((19|20)[0-9][0-9])";
            string dateTimeRegEx = String.Format(" {0}([ ]+){1}([ ]+)(({2}([ ]+){3})|{2}|{3}) ", regExMonth, regExDay, regExTime, regExYear);

            Match dateStart = Regex.Match(lineToParse, dateTimeRegEx, RegexOptions.IgnoreCase);

            if (!dateStart.Success)
            {
                throw new ApplicationException(String.Format("FTP server LS format not supported: {0}", base.RawDetails));
            }

            // Got the date; split it into toks so that it can be formatted into something parsable.
            // If the create date is this year, it may have this format: Feb 23 10:41 = "MMM dd hh:mm"
            //  If the create date is prior to this year, it will have this format: Feb 23 2010 = "MMM dd yyyy"
            string[] toks = dateStart.Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string dayFormat = toks[1].Length == 1 ? "d" : "dd";
            // note: assumes a 24-hour clock (ex: 1PM = 13)
            string hourFormat = "HH", minuteFormat = "mm", secondFormat = "ss";
            MatchCollection colons = Regex.Matches(toks[2], ":");

            // handle 1-digit time components
            if (colons.Count > 0)
            {
                string[] timeToks = toks[2].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (timeToks[0].Length == 1)
                    hourFormat = "H";
                if (timeToks[1].Length == 1)
                    minuteFormat = "m";
                if (colons.Count > 1 && timeToks[2].Length == 1)
                    secondFormat = "s";
            }

            bool dateOk = false;
            DateTime createDate = DateTime.MinValue;
            switch (colons.Count)
            {
                case 0:
                    dateOk = DateTime.TryParseExact(String.Format("{0} {1} {2}", toks[0], toks[1], toks[2]), String.Format("MMM {0} yyyy", dayFormat), CultureInfo.InvariantCulture, DateTimeStyles.None, out createDate);
                    break;
                case 1:
                    dateOk = DateTime.TryParseExact(String.Format("{0} {1} {2} {3}", toks[0], toks[1], toks.Length == 4 ? toks[3] : DateTime.Today.Year.ToString(), toks[2]), String.Format("MMM {0} yyyy {1}:{2}", dayFormat, hourFormat, minuteFormat), CultureInfo.InvariantCulture, DateTimeStyles.None, out createDate);
                    break;
                case 2:
                    dateOk = DateTime.TryParseExact(String.Format("{0} {1} {2} {3}", toks[0], toks[1], toks.Length == 4 ? toks[3] : DateTime.Today.Year.ToString(), toks[2]), String.Format("MMM {0} yyyy {1}:{2}:{3}", dayFormat, hourFormat, minuteFormat, secondFormat), CultureInfo.InvariantCulture, DateTimeStyles.None, out createDate);
                    break;
            }

            if (!dateOk)
            {
                throw new ApplicationException(String.Format("Could not parse the date: {0}", base.RawDetails));
            }

            CreateDate = createDate;

            // set the name, which is the last element in the line
            int fileNameStartIdx = dateStart.Index + dateStart.Value.Length;
            Name = lineToParse.Substring(fileNameStartIdx);

            // finally, split the first part of the line into toks, before the date, to get the bytes:
            toks = lineToParse.Substring(0, dateStart.Index).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            Bytes = long.Parse(toks[toks.Length - 1]);
        }
    }

}
