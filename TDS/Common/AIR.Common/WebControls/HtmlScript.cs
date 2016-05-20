/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AIR.Common.WebControls
{
	[DefaultProperty("Text"),ToolboxData("<{0}:Script runat=server src=\"\" language=\"javascript\"></{0}:Script>")]
	public class HtmlScript : WebControl
	{
		private string source;

		public string Src
		{
			get { return source; }
			set { source = value; }
		}

		protected override void Render(HtmlTextWriter output)
		{
			output.WriteBeginTag("script");
			output.WriteAttribute("src",base.ResolveUrl(this.source));
			output.WriteAttribute("language", "javascript");
			foreach (string key in this.Attributes.Keys)
			{
				output.WriteAttribute(key,this.Attributes[key]);
			}
			output.Write(HtmlTextWriter.TagRightChar);
			output.WriteEndTag("script");
		}
	}
}
