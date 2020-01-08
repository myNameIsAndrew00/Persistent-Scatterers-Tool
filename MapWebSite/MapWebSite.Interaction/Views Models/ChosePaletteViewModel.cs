using MapWebSite.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Interaction.ViewModel
{
    using UserColorMap = Tuple<string, ColorMap>;

    public class ChosePaletteViewModel
    {
        public readonly int ColorsPerRow = 7;

        public static readonly int ColorPalettesPerPage = 10;

        public IEnumerable<UserColorMap> UsersColorMaps { get; } = null;

        public ChosePaletteViewModel(IEnumerable<UserColorMap> usersColorMaps)
        {
            this.UsersColorMaps = usersColorMaps;
        }
    }
}
