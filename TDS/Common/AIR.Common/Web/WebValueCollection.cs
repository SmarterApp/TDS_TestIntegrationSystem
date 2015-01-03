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
using System.Text;
using System.Web;

namespace AIR.Common.Web
{
    /// <summary>
    /// A class used for holding web values.
    /// </summary>
    /// <remarks>
    /// Assembly: System.ServiceModel.HttpValueCollection
    /// </remarks>
    public class WebValueCollection : NameValueCollection
    {
        public WebValueCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
            IsReadOnly = false;
        }

        public WebValueCollection(NameValueCollection col) : this()
        {
            this.Add(col);
        }

        public void Reset()
        {
            base.Clear();
        }

        public void Set(string name, object value)
        {
            if (string.IsNullOrEmpty(name) || value == null) return;
            base.Set(name, value.ToString());
        }

        public T Get<T>(string name)
        {
            T value = default(T);

            if (!string.IsNullOrEmpty(Get(name)))
            {
                object nameValue = Get(name);
                return nameValue.ConvertTo<T>();
            }

            return value;
        }

        public T Get<T>(string name, T defaultValue)
        {
            if (!string.IsNullOrEmpty(Get(name)))
            {
                object nameValue = Get(name);
                return nameValue.ConvertTo<T>();
            }
            
            return defaultValue;
        }

        public void FillFromString(string s)
        {
            this.FillFromString(s, false, null);
        }

        public void FillFromString(string data, bool urlencoded, Encoding encoding)
        {
            int num = (data != null) ? data.Length : 0;
            for (int i = 0; i < num; i++)
            {
                int startIndex = i;
                int num4 = -1;
                while (i < num)
                {
                    char ch = data[i];
                    if (ch == '=')
                    {
                        if (num4 < 0)
                        {
                            num4 = i;
                        }
                    }
                    else if (ch == '&')
                    {
                        break;
                    }
                    i++;
                }
                string str = null;
                string str2 = null;
                if (num4 >= 0)
                {
                    str = data.Substring(startIndex, num4 - startIndex);
                    str2 = data.Substring(num4 + 1, (i - num4) - 1);
                }
                else
                {
                    str2 = data.Substring(startIndex, i - startIndex);
                }
                if (urlencoded)
                {
                    base.Add(HttpUtility.UrlDecode(str, encoding), HttpUtility.UrlDecode(str2, encoding));
                }
                else
                {
                    base.Add(str, str2);
                }
                if ((i == (num - 1)) && (data[i] == '&'))
                {
                    base.Add(null, string.Empty);
                }
            }
        }

        public override string ToString()
        {
            return this.ToString(true, null);
        }

        public string ToString(bool urlencoded)
        {
            return this.ToString(urlencoded, null);
        }

        public string ToString(bool urlencoded, IDictionary excludeKeys)
        {
            int count = this.Count;
            
            if (count == 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            
            for (int i = 0; i < count; i++)
            {
                string key = this.GetKey(i);

                if (((excludeKeys == null) || (key == null)) || (excludeKeys[key] == null))
                {
                    string str3;
                    if (urlencoded)
                    {
                        key = HttpUtility.UrlEncodeUnicode(key);
                    }
                    string str2 = !string.IsNullOrEmpty(key) ? (key + "=") : string.Empty;
                    ArrayList list = (ArrayList)base.BaseGet(i);
                    int num3 = (list != null) ? list.Count : 0;
                    
                    if (builder.Length > 0)
                    {
                        builder.Append('&');
                    }
                    
                    if (num3 == 1)
                    {
                        builder.Append(str2);
                        str3 = (string)list[0];
                        if (urlencoded)
                        {
                            str3 = HttpUtility.UrlEncodeUnicode(str3);
                        }
                        builder.Append(str3);
                    }
                    else if (num3 == 0)
                    {
                        builder.Append(str2);
                    }
                    else
                    {
                        for (int j = 0; j < num3; j++)
                        {
                            if (j > 0)
                            {
                                builder.Append('&');
                            }
                            
                            builder.Append(str2);
                            str3 = (string)list[j];
                            
                            if (urlencoded)
                            {
                                str3 = HttpUtility.UrlEncodeUnicode(str3);
                            }
                            
                            builder.Append(str3);
                        }
                    }
                }
            }

            return builder.ToString();
        }
    }

}
