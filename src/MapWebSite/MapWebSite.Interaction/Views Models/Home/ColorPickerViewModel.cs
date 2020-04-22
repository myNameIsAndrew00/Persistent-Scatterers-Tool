using MapWebSite.Types;
using MapWebSite.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Domain.ViewModel
{
    public class ColorPickerViewModel
    {
        private readonly int defaultColorsCount = 16;
        private readonly int defaultMinimumLuminosity = 45;
        private readonly int defaultMaximumLuminosity = 157;

        public enum PaletteColorsHue
        {
            Red = 0,
            Orange = 40,
            Yellow = 56,
            Green = 97,
            LightBlue = 186,
            Blue = 214,
            Pink = 310,
            Black = -1
        }

        public enum DefaultPalette
        { 
            [EnumString( "[{ \"color\":\"#000000\",\"percent\": 0 }," 
                       +  "{ \"color\":\"#FFFFFF\",\"percent\": 25 }," 
                       +  "{ \"color\":\"#AD91CE\",\"percent\": 40 }," 
                       +  "{ \"color\":\"#9152CE\",\"percent\": 55 }," 
                       +  "{ \"color\":\"#CCCE86\",\"percent\": 75 }]")]
            Crown,

            [EnumString("[ { \"color\":\"#f2438b\" , \"percent\": 0 },"
                       +  "{ \"color\":\"#ff7f00\" , \"percent\": 13 },"
                       +  "{ \"color\":\"#ffbc00\" , \"percent\": 26 },"
                       +  "{ \"color\":\"#FFFF00\" , \"percent\": 39 },"      
                       +  "{ \"color\":\"#d8d852\" , \"percent\": 52 },"
                       +  "{ \"color\":\"#aaaf38\" , \"percent\": 65 },"
                       +  "{ \"color\":\"#38b274\" , \"percent\": 78 },"
                       +  "{ \"color\":\"#76e586\" , \"percent\": 91 }]")]
            Default,

            [EnumString("[ { \"color\":\"#f49a38\" , \"percent\": 0 },"                            
                        + "{ \"color\":\"#59af4a\" , \"percent\": 50 }]")]
            Fox,             

            [EnumString( "[{ \"color\": \"#2032d3\" , \"percent\": 0  },"
                        + "{ \"color\": \"#31bac4\" , \"percent\": 25 }," 
                        + "{ \"color\": \"#d35050\" , \"percent\": 40 }," 
                        + "{ \"color\": \"#66a563\" , \"percent\": 55 },"
                        + "{ \"color\": \"#ba7c52\" , \"percent\": 75 }]")] 
            Kuller
        }

        public string GetColor(PaletteColorsHue hue)
        {
            if ((int)hue == -1) return "#000000";

            var color = Helper.ConvertHSLToRGB((double)hue, 1, 0.54);
            return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        public List<string> GetColors(PaletteColorsHue hue)
        {
            List<string> result = new List<string>();
            List<double> saturations = new List<double>() { 1, 0.44 };

            if((int)hue == -1)
            {
                for (int luminosity = 240; luminosity >= 0; luminosity -= 240 / defaultColorsCount) {
                    var color = Helper.ConvertHSLToRGB((double)hue, 0, ((double)luminosity) / 240);
                    result.Add(string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B));
                }
                return result;
            }

            foreach (var saturation in saturations)
                for (int luminosity = defaultMaximumLuminosity; luminosity >= defaultMinimumLuminosity; luminosity -= 14)
                {
                    var color = Helper.ConvertHSLToRGB((double)hue, saturation, ((double)luminosity) / 240);
                    result.Add(string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B));
                }

            return result;
        }

    }
}
