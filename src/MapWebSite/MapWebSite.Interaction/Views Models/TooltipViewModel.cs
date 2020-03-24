using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Domain.ViewModel
{
    public class TooltipViewModel
    {
        public string DisplayedMessage { get; set; }
    }

    public class GifTooltipViewModel : TooltipViewModel
    {
        public string GifPath { get; set; }
    }
}
