using MapWebSite.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Interaction.ViewModel
{
    using UserColorMap = Tuple<string, ColorMap>;

    public class ChosePaletteModel
    {
        public readonly int colorsPerRow = 7;

        public IEnumerable<UserColorMap> UsersColorMaps { get; } = null;

        public ChosePaletteModel(IEnumerable<UserColorMap> usersColorMaps)
        {
            this.UsersColorMaps = usersColorMaps;
        }
    }
}
