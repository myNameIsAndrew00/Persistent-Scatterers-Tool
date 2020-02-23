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
            [EnumString("255,0,0")]
            Red,
            [EnumString("255, 213, 122")]
            Orange,
            [EnumString("251, 255, 122")]
            Yellow,
            [EnumString("128, 255, 149")]
            Green,
            [EnumString("107, 188, 255")]
            Blue,
            [EnumString("255, 143, 244")]
            Pink,
            [EnumString("163,163,163")]
            Black
        }

        public string GetColor(string rgbString)
        {
            int[] colors = rgbString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => Convert.ToInt32(value)).ToArray();
            return string.Format("#{0:X2}{1:X2}{2:X2}", colors[0], colors[1], colors[2]);
        }

        public List<string> GetColors(PaletteColorsHue hue)
        {
            List<string> result = new List<string>();

            int[] colors = hue.GetEnumString().Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select( value => Convert.ToInt32(value) ).ToArray();

            int[] initialColors = new int[3];
            colors.CopyTo(initialColors, 0);

            for (int decrement = 0, index = 0; index < 16; decrement += 10, index++)
            {
                result.Add(string.Format("#{0:X2}{1:X2}{2:X2}", colors[0], colors[1] , colors[2]));
                colors[0] -= decrement;
                colors[1] -= decrement;
                colors[2] -= decrement;

                if (colors[0] < 0) colors[0] = initialColors[0];
                if (colors[1] < 0) colors[1] = initialColors[1];
                if (colors[2] < 0) colors[2] = initialColors[2];
            }

            return result;
        }

    }
}
