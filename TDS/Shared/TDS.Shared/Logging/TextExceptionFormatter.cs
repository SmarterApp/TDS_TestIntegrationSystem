/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace TDS.Shared.Logging
{
    /// <summary>
    /// Represents an exception formatter that formats exception objects as text.
    /// </summary>	
    public class TextExceptionFormatter : ExceptionFormatter
    {
        private readonly TextWriter writer;
        private int innerDepth;

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="TextExceptionFormatter"/> using the specified
        /// <see cref="TextWriter"/> and <see cref="Exception"/>
        /// objects.
        /// </summary>
        /// <param name="writer">The stream to write formatting information to.</param>
        /// <param name="exception">The exception to format.</param>
        public TextExceptionFormatter(TextWriter writer, Exception exception)
            : this(writer, exception, Guid.Empty)
        { }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="TextExceptionFormatter"/> using the specified
        /// <see cref="TextWriter"/> and <see cref="Exception"/>
        /// objects.
        /// </summary>
        /// <param name="writer">The stream to write formatting information to.</param>
        /// <param name="exception">The exception to format.</param>
        /// <param name="handlingInstanceId">The id of the handling chain.</param>
        public TextExceptionFormatter(TextWriter writer, Exception exception, Guid handlingInstanceId)
            : base(exception, handlingInstanceId)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        /// <summary>
        /// Gets the underlying <see cref="TextWriter"/>
        /// that the current formatter is writing to.
        /// </summary>
        public TextWriter Writer
        {
            get { return this.writer; }
        }

        /// <summary>
        /// Formats the <see cref="Exception"/> into the underlying stream.
        /// </summary>
        public override void Format()
        {
            if (this.HandlingInstanceId != Guid.Empty)
            {
                this.Writer.WriteLine(
                    "HandlingInstanceID: {0}",
                    HandlingInstanceId.ToString("D", CultureInfo.InvariantCulture));
            }
            base.Format();
        }

        /// <summary>
        /// Writes a generic description to the underlying text stream.
        /// </summary>
        protected override void WriteDescription()
        {
            // An exception of type {0} occurred and was caught.
            // -------------------------------------------------
            string line = string.Format(CultureInfo.CurrentCulture, "An exception of type '{0}' occurred and was caught.", base.Exception.GetType().FullName);
            this.Writer.WriteLine(line);

            string separator = new string('-', line.Length);

            this.Writer.WriteLine(separator);
        }

        /// <summary>
        /// Writes and formats the exception and all nested inner exceptions to the <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="exceptionToFormat">The exception to format.</param>
        /// <param name="outerException">The outer exception. This 
        /// value will be null when writing the outer-most exception.</param>
        protected override void WriteException(Exception exceptionToFormat, Exception outerException)
        {
            if (outerException != null)
            {
                this.innerDepth++;
                this.Indent();
                string temp = "Inner Exception";
                string separator = new string('-', temp.Length);
                this.Writer.WriteLine(temp);
                this.Indent();
                this.Writer.WriteLine(separator);

                base.WriteException(exceptionToFormat, outerException);
                this.innerDepth--;
            }
            else
            {
                base.WriteException(exceptionToFormat, outerException);
            }
        }

        /// <summary>
        /// Writes the current date and time to the <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="utcNow">The current time.</param>
        protected override void WriteDateTime(DateTime utcNow)
        {
            DateTime localTime = utcNow.ToLocalTime();
            string localTimeString = localTime.ToString("G", DateTimeFormatInfo.InvariantInfo);

            this.Writer.WriteLine(localTimeString);
        }

        /// <summary>
        /// Writes the value of the <see cref="Type.AssemblyQualifiedName"/>
        /// property for the specified exception type to the <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="exceptionType">The <see cref="Type"/> of the exception.</param>
        protected override void WriteExceptionType(Type exceptionType)
        {
            IndentAndWriteLine("Type : {0}", exceptionType.AssemblyQualifiedName);
        }

        /// <summary>
        /// Writes the value of the <see cref="Exception.Message"/>
        /// property to the underyling <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="message">The message to write.</param>
        protected override void WriteMessage(string message)
        {
            // only use first 5000 characters of error
            const int maxMessageLength = 5000;
            if (message.Length > maxMessageLength)
            {
                message = message.Substring(0, maxMessageLength);
            } 

            IndentAndWriteLine("Message : {0}", message);
        }

        /// <summary>
        /// Writes the value of the specified source taken
        /// from the value of the <see cref="Exception.Source"/>
        /// property to the <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="source">The source of the exception.</param>
        protected override void WriteSource(string source)
        {
            IndentAndWriteLine("Source : {0}", source);
        }

        /// <summary>
        /// Writes the value of the specified help link taken
        /// from the value of the <see cref="Exception.HelpLink"/>
        /// property to the <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="helpLink">The exception's help link.</param>
        protected override void WriteHelpLink(string helpLink)
        {
            IndentAndWriteLine("Help link : {0}", helpLink);
        }

        /// <summary>
        /// Writes the name and value of the specified property to the <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="propertyInfo">The reflected <see cref="PropertyInfo"/> object.</param>
        /// <param name="value">The value of the <see cref="PropertyInfo"/> object.</param>
        protected override void WritePropertyInfo(PropertyInfo propertyInfo, object value)
        {
            this.Indent();
            this.Writer.Write(propertyInfo.Name);
            this.Writer.Write(" : ");
            this.Writer.WriteLine(value);
        }

        /// <summary>
        /// Writes the name and value of the specified field to the <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="fieldInfo">The reflected <see cref="FieldInfo"/> object.</param>
        /// <param name="value">The value of the <see cref="FieldInfo"/> object.</param>
        protected override void WriteFieldInfo(FieldInfo fieldInfo, object value)
        {
            this.Indent();
            this.Writer.Write(fieldInfo.Name);
            this.Writer.Write(" : ");
            this.Writer.WriteLine(value);
        }

        protected override void WritingDataInfo(IDictionary exceptionData)
        {
            this.Indent();
            this.Writer.Write("Data");
            this.Writer.Write(" : ");

            bool comma = false;
            foreach (DictionaryEntry dictionaryEntry in exceptionData)
            {
                // skip help links
                if (dictionaryEntry.Key.ToString().Contains("HelpLink")) continue;

                if (comma) this.Writer.Write(", ");
                this.Writer.Write(dictionaryEntry.Key);
                this.Writer.Write("=");
                this.Writer.Write("\"");
                this.Writer.Write(dictionaryEntry.Value);
                this.Writer.Write("\"");
                comma = true;
            }

            this.Writer.WriteLine();
        }

        /// <summary>
        /// Writes the value of the <see cref="System.Exception.StackTrace"/> property to the <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="stackTrace">The stack trace of the exception.</param>
        /// <remarks>
        /// If there is no stack trace available, an appropriate message will be displayed.
        /// </remarks>
        protected override void WriteStackTrace(string stackTrace)
        {
            this.Indent();
            this.Writer.Write("Stack Trace");
            this.Writer.Write(" : ");
            this.Writer.WriteLine();

            if (stackTrace == null || stackTrace.Length == 0)
            {
                this.Writer.WriteLine("The stack trace is unavailable.");
            }
            else
            {
                // The stack trace has all '\n's prepended with a number
                // of tabs equal to the InnerDepth property in order
                // to make the formatting pretty.
                string indentation = new String('\t', this.innerDepth);
                string indentedStackTrace = stackTrace.Replace("\n", "\n" + indentation);

                this.Writer.WriteLine(indentedStackTrace);
                this.Writer.WriteLine();
            }
        }

        protected override void WriteSqlErrors(Exception exceptionToFormat)
        {
            if (exceptionToFormat is SqlException)
            {
                SqlException sqlException = exceptionToFormat as SqlException;

                this.Writer.WriteLine();
                this.Writer.WriteLine("Sql Errors:");

                int count = 0;

                foreach (SqlError sqlError in sqlException.Errors)
                {
                    this.Writer.WriteLine();

                    count++;

                    IndentAndWriteLine("Sql Error #{0}:", count);
                    IndentAndWriteLine("Server: {0}", sqlError.Server);
                    IndentAndWriteLine("Class: {0}", sqlError.Class);
                    IndentAndWriteLine("State: {0}", sqlError.State);
                    IndentAndWriteLine("Number: {0}", sqlError.Number);
                    IndentAndWriteLine("LineNumber: {0}", sqlError.LineNumber);
                    IndentAndWriteLine("Source: {0}", sqlError.Source);
                    IndentAndWriteLine("Procedure: {0}", sqlError.Procedure);
                    IndentAndWriteLine("Message: {0}", sqlError.Message);

                    // stop at 10 errors in case of recursion
                    if (count >= 10) return;
                }
            }
        }

        /// <summary>
        /// Writes the additional properties to the <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="additionalInformation">Additional information to be included with the exception report</param>
        protected override void WriteAdditionalInfo(NameValueCollection additionalInformation)
        {
            this.Writer.WriteLine("Additional Info:");
            this.Writer.WriteLine();

            foreach (string name in additionalInformation.AllKeys)
            {
                if (!String.IsNullOrEmpty(name))
                {
                    this.Writer.Write(name);
                    this.Writer.Write(" : ");
                    this.Writer.Write(additionalInformation[name]);
                    this.Writer.Write("\n");
                }
            }
        }

        /// <summary>
        /// Indents the <see cref="TextWriter"/>.
        /// </summary>
        protected virtual void Indent()
        {
            for (int i = 0; i < innerDepth; i++)
            {
                this.Writer.Write("\t");
            }
        }

        private void IndentAndWriteLine(string format, params object[] arg)
        {
            this.Indent();
            this.Writer.WriteLine(format, arg);
        }
    }
}
