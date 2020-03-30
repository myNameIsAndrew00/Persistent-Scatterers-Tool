using MapWebSite.GeoserverAPI.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MapWebSite.GeoserverAPI.Modules.Styles
{

    [XmlRoot(Namespace= "http://www.opengis.net/sld")]
    public class StyledLayerDescriptor
    {
        public class NamedLayer
        {

            public class UserStyle
            {
                public class FeatureTypeStyle
                {
                    [XmlElement("Rule")]
                    public List<Rule> Rules { get; set; }
                }

                public string Title { get; set; }

                [XmlElement("FeatureTypeStyle")]
                public FeatureTypeStyle FeatureTypeStyleObject { get; set; }

            }

            public string Name { get; set; }

            [XmlElement("UserStyle")]
            public UserStyle UserStyleObject { get; set; }

        }

        [XmlElement("NamedLayer")]
        public NamedLayer NamedLayerObject { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; } = "1.0.0";


    }

}
