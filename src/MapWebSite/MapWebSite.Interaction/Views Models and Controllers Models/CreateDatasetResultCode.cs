using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Domain.ViewModel
{  
   /// <summary>
   /// An enum which handles codes for domain CreateDataset function
   /// </summary>
    public enum CreateDatasetResultCode
    {
        Ok,
        GeoserverError,
        DatasetError,
        BadPaletteError
    }
}
