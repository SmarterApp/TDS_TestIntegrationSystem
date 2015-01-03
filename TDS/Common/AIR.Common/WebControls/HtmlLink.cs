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
	[DefaultProperty("Text"),ToolboxData("<{0}:Link runat=server Href=\"\" Rel=\"Stylesheet\" Type=\"text/css\"></{0}:Link>")]
	public class HtmlLink : WebControl
	{
		private string _href;
	    private string media;
	    private string type;
	    private string rel;

	    public string Type
	    {
	        get { return type; }
	        set { type = value; }
	    }

	    public string Rel
	    {
	        get { return rel; }
	        set { rel = value; }
	    }

	    public string Media
	    {
	        get { return media; }
	        set { media = value; }
	    }

	    public string Href
		{
			get { return _href; }
			set { _href = value; }
		}

		protected override void Render(HtmlTextWriter output)
		{
			output.WriteBeginTag("link");
			output.WriteAttribute("href",base.ResolveUrl(this.Href));
			output.WriteAttribute("media", media);
			output.WriteAttribute("type", type);
			output.WriteAttribute("rel", rel);

			foreach (string key in this.Attributes.Keys)
			{
				output.WriteAttribute(key,this.Attributes[key]);
			}
			output.Write(HtmlTextWriter.TagRightChar);
			output.WriteEndTag("link");
		}
	}
}
