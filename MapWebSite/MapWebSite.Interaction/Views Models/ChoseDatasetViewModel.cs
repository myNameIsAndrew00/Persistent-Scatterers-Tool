using MapWebSite.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Domain.ViewModel
{
   

    public class ChoseDatasetViewModel
    { 
        public static readonly int DataPointsPerPage = 10;

        public IEnumerable<PointsDataSetBase> Datasets { get; } = null;

        public ChoseDatasetViewModel(IEnumerable<PointsDataSetBase> usersDatasets)
        {
            this.Datasets = usersDatasets;
        }
    }
}
