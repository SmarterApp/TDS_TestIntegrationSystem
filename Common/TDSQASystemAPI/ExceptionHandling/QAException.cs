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

namespace TDSQASystemAPI.ExceptionHandling
{
	public class QAException : Exception
	{
		private string message;
		private ExceptionType type;

		public enum ExceptionType { Unknown = 0, InvalidXml = 1, ConfigurationError = 2, ProgrammingError = 3, General = 4 };

		internal QAException()
			: base()
		{
			this.type = ExceptionType.General;
		}

		public QAException(string message, ExceptionType type) : base(message)
		{
			this.type = type;
		}

		public QAException(string message) : base(message)
		{
			this.type = ExceptionType.Unknown;
		}

		public QAException(string message, ExceptionType type, Exception exception) : base(message, exception)
		{
			this.type = type;
		}
	}
}
