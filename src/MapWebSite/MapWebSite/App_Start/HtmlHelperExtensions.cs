using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Schema;

namespace MapWebSite
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Use this method to obtain the plain form of an svg.
        /// TODO: optimize the time of parsing
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="path">The path where the svg is found on server</param>
        /// <param name="htmlAttributes">additional attributes which must be added to svg</param>
        /// <returns></returns>
        public static MvcHtmlString SVG(this HtmlHelper htmlHelper, string path, object htmlAttributes)
        {  
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(HttpContext.Current.Server.MapPath(path));
             
            
            PropertyInfo[] properties = htmlAttributes?.GetType().GetProperties() ?? null;
           
            if(properties != null)
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (xmlDoc.DocumentElement.Attributes[propertyInfo.Name] != null)
                {
                    xmlDoc.DocumentElement.Attributes[propertyInfo.Name].Value =
                        (string)propertyInfo.GetValue(htmlAttributes, null);
                }
                else
                {
                    XmlAttribute xsiNil = xmlDoc.CreateAttribute(propertyInfo.Name,"");
                    xsiNil.Value = (string)propertyInfo.GetValue(htmlAttributes, null);
                    xmlDoc.DocumentElement.Attributes.Append(xsiNil);
                }
            }
            return new MvcHtmlString(xmlDoc.OuterXml);
        }
    }
}