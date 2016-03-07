/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;

namespace AIR.Common.WebControls
{
    [DefaultProperty("FileName"), Description("HTML Include Control will allow you to have one central location for all of your HTML that is common across all your web sites."), ToolboxData("<{0}:HtmlInclude runat=server></{0}:HtmlInclude>")]
    public class HtmlInclude : WebControl
    {
        private const string conHTML = "HtmlInclude";
        private string mstrFileName = "";
        private bool mboolCache = true;
        private string mstrHTML = "";
        private string mstrMessage = "Fill in a File Name";
        private bool mboolDesignMode;

        [Bindable(true), Category("Misc"), DefaultValue("")]
        public string FileName
        {
            get
            {
                return mstrFileName;
            }
            set
            {
                mstrFileName = value;
            }
        }

        [Category("Misc"), DefaultValue(true)]
        public bool Cache
        {
            get
            {
                return mboolCache;
            }
            set
            {
                mboolCache = value;
            }
        }

        public string HTML
        {
            get
            {
                return mstrHTML;
            }
            set
            {
                mstrHTML = value;
            }
        }

        protected override void Render(HtmlTextWriter output)
        {
            try
            {
                if (IsFileValid())
                {
                    if (IsDesignMode())
                    {
                        output.Write(GetHtml());
                    }
                    else
                    {
                        if (this.CacheExists())
                        {
                            output.Write(GetFromCache());
                        }
                        else
                        {
                            output.Write(GetHtml());
                        }
                    }
                }
                else
                {
                    output.Write(mstrMessage);
                }
            }
            catch (Exception exp)
            {
                output.Write(exp.Message);
            }
        }

        private bool CacheExists()
        {
            if (HttpContext.Current.Application[conHTML + FileName] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private string GetFromCache()
        {
            try
            {
                return HttpContext.Current.Application[conHTML + FileName].ToString();
            }
            catch
            {
                throw;
            }
        }

        private void SetCache(string Value)
        {
            try
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[conHTML + FileName] = Value;
                HttpContext.Current.Application.UnLock();
            }
            catch
            {
                throw;
            }
        }

        private bool IsDesignMode()
        {
            try
            {
                if (HttpContext.Current == null)
                {
                    mboolDesignMode = true;
                }
                else
                {
                    mboolDesignMode = false;
                }
                return mboolDesignMode;
            }
            catch
            {
                throw;
            }
        }

        private string GetHtml()
        {
            System.IO.TextReader tr;
            try
            {
                tr = File.OpenText(Context.Server.MapPath(mstrFileName));
                mstrHTML = tr.ReadToEnd();
                tr.Close();
                if (!(mboolDesignMode))
                {
                    if (mboolCache)
                    {
                        SetCache(mstrHTML);
                    }
                }
                return mstrHTML;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        private bool IsFileValid()
        {
            bool boolReturn = false;
            if (mstrFileName.Trim().Length == 0)
            {
                mstrMessage = "Fill in a FileName";
            }
            else
            {
                if (File.Exists(Context.Server.MapPath(mstrFileName)))
                {
                    boolReturn = true;
                }
                else
                {
                    mstrMessage = "File: " + mstrFileName + " does not exist";
                }
            }
            return boolReturn;
        }
    }  
}
