/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System.Text;

namespace AIR.Common.Utilities
{
    // Download: http://www.microsoft.com/downloads/details.aspx?FamilyId=EFB9C819-53FF-4F82-BFAF-E11625130C25
    // Help: http://msdn2.microsoft.com/en-us/library/aa973813.aspx

    public class AntiXss
    {
        private const string EmptyString_JavaScript = "''";
        private const string EmptyString_VBS = "\"\"";

        private static string EncodeHtml(string strInput)
        {
            return EncodeHtml(strInput, null);
        }

        private static string EncodeHtml(string strInput, int? maxLength)
        {
            if (strInput == null)
            {
                return null;
            }
            if (strInput.Length == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder("", strInput.Length * 2);
            foreach (char ch in strInput)
            {
                if ((((ch > '`') && (ch < '{')) || ((ch > '@') && (ch < '['))) || (((ch == ' ') || ((ch > '/') && (ch < ':'))) || (((ch == '.') || (ch == ',')) || ((ch == '-') || (ch == '_')))))
                {
                    builder.Append(ch);
                }
                else
                {
                    builder.Append("&#" + ((int) ch).ToString() + ";");
                }

                // check if the maxlength of the string was met if this was passed in
                if (maxLength != null && builder.Length >= maxLength) break;
            }

            return builder.ToString();
        }

        private static string EncodeHtmlAttribute(string strInput)
        {
            if (strInput == null)
            {
                return null;
            }
            if (strInput.Length == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder("", strInput.Length * 2);
            foreach (char ch in strInput)
            {
                if ((((ch > '`') && (ch < '{')) || ((ch > '@') && (ch < '['))) || (((ch > '/') && (ch < ':')) || (((ch == '.') || (ch == ',')) || ((ch == '-') || (ch == '_')))))
                {
                    builder.Append(ch);
                }
                else
                {
                    builder.Append("&#" + ((int) ch).ToString() + ";");
                }
            }
            return builder.ToString();
        }

        private static string EncodeJs(string strInput)
        {
            if (strInput == null)
            {
                return null;
            }
            if (strInput.Length == 0)
            {
                return "''";
            }
            StringBuilder builder = new StringBuilder("'", strInput.Length * 2);
            foreach (char ch in strInput)
            {
                if ((((ch > '`') && (ch < '{')) || ((ch > '@') && (ch < '['))) || (((ch == ' ') || ((ch > '/') && (ch < ':'))) || (((ch == '.') || (ch == ',')) || ((ch == '-') || (ch == '_')))))
                {
                    builder.Append(ch);
                }
                else if (ch > '\x007f')
                {
                    builder.Append(@"\u" + TwoByteHex(ch));
                }
                else
                {
                    builder.Append(@"\x" + SingleByteHex(ch));
                }
            }
            builder.Append("'");
            return builder.ToString();
        }

        private static string EncodeUrl(string strInput)
        {
            if (strInput == null)
            {
                return null;
            }
            if (strInput.Length == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder("", strInput.Length * 2);
            foreach (char ch in strInput)
            {
                if ((((ch > '`') && (ch < '{')) || ((ch > '@') && (ch < '['))) || (((ch > '/') && (ch < ':')) || (((ch == '.') || (ch == '-')) || (ch == '_'))))
                {
                    builder.Append(ch);
                }
                else if (ch > '\x007f')
                {
                    builder.Append("%u" + TwoByteHex(ch));
                }
                else
                {
                    builder.Append("%" + SingleByteHex(ch));
                }
            }
            return builder.ToString();
        }

        private static string EncodeVbs(string strInput)
        {
            if (strInput == null)
            {
                return null;
            }
            if (strInput.Length == 0)
            {
                return "\"\"";
            }
            StringBuilder builder = new StringBuilder("", strInput.Length * 2);
            bool flag = false;
            foreach (char ch in strInput)
            {
                if ((((ch > '`') && (ch < '{')) || ((ch > '@') && (ch < '['))) || (((ch == ' ') || ((ch > '/') && (ch < ':'))) || (((ch == '.') || (ch == ',')) || ((ch == '-') || (ch == '_')))))
                {
                    if (!flag)
                    {
                        builder.Append("&\"");
                        flag = true;
                    }
                    builder.Append(ch);
                }
                else
                {
                    if (flag)
                    {
                        builder.Append("\"");
                        flag = false;
                    }
                    builder.Append("&chrw(" + ((uint) ch).ToString() + ")");
                }
            }
            if ((builder.Length > 0) && (builder[0] == '&'))
            {
                builder.Remove(0, 1);
            }
            if (builder.Length == 0)
            {
                builder.Insert(0, "\"\"");
            }
            if (flag)
            {
                builder.Append("\"");
            }
            return builder.ToString();
        }

        private static string EncodeXml(string strInput)
        {
            return EncodeHtml(strInput);
        }

        private static string EncodeXmlAttribute(string strInput)
        {
            return EncodeHtmlAttribute(strInput);
        }

        public static string HtmlAttributeEncode(string s)
        {
            return EncodeHtmlAttribute(s);
        }

		/// <Summary>
		/// 
		/// </Summary>
		/// <Example>
		/// 
		/// </Example>

		/// <Summary>
		/// Untrusted input is used in HTML output except when assigning to an HTML attribute.
		/// </Summary>
		/// <Example>
		/// <a href="http://www.contoso.com">Click Here [Untrusted input]</a>
		/// </Example>
        public static string HtmlEncode(string s)
        {
            return EncodeHtml(s);
        }

        public static string HtmlEncode(string s, int maxLength)
        {
            return EncodeHtml(s, maxLength);
        }

        public static string JavaScriptEncode(string s)
        {
            return EncodeJs(s);
        }

        private static string SingleByteHex(char c)
        {
            uint num = c;
            return num.ToString("x").PadLeft(2, '0');
        }

        private static string TwoByteHex(char c)
        {
            uint num = c;
            return num.ToString("x").PadLeft(4, '0');
        }

        public static string UrlEncode(string s)
        {
            return EncodeUrl(s);
        }

        public static string VisualBasicScriptEncode(string s)
        {
            return EncodeVbs(s);
        }

        public static string XmlAttributeEncode(string s)
        {
            return EncodeXmlAttribute(s);
        }

        public static string XmlEncode(string s)
        {
            return EncodeXml(s);
        }
    }
}

