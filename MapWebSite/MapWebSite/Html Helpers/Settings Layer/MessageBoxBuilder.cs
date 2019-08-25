using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace MapWebSite.HtmlHelpers
{
    /// <summary>
    /// Used to create html message boxes
    /// </summary>
    public static class MessageBoxBuilder
    {
        public static string Create(string title, string message)
        {
            StringWriter writer = new StringWriter();
            using (HtmlTextWriter htmlWriter = new HtmlTextWriter(writer))
            {
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "message-overlay-container message-box");
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);

                htmlWriter.RenderBeginTag(HtmlTextWriterTag.H3);
                htmlWriter.Write(title);
                htmlWriter.RenderEndTag();

                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Small);
                htmlWriter.Write(message);
                htmlWriter.RenderEndTag();

                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Br);
                htmlWriter.RenderEndTag();

                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Onclick, "hideOverlay()");
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Button);
                htmlWriter.Write("Ok");
                htmlWriter.RenderEndTag();

                htmlWriter.RenderEndTag();

            }

            return writer.ToString();
        }
    }
}