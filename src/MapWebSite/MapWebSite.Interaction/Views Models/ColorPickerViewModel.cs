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

        public enum PaletteColorsHue
        {
            Red = 0,
            Orange = 30,
            Yellow = 38,
            Green = 80,
            Blue = 160,
            Pink = 220,
            Black = -1
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
                for (int luminosity = 240; luminosity >= 0; luminosity -= 240 / 16) {
                    var color = Helper.ConvertHSLToRGB((double)hue, 0, ((double)luminosity) / 240);
                    result.Add(string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B));
                }
                return result;
            }

            foreach (var saturation in saturations)
                for (int luminosity = 157; luminosity >= 45; luminosity -= 14)
                {
                    var color = Helper.ConvertHSLToRGB((double)hue, saturation, ((double)luminosity) / 240);
                    result.Add(string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B));
                }

            return result;
        }

    }
}
