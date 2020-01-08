using MapWebSite.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Interaction.ViewModel
{
    using UserDataset = Tuple<string, string, int>;

    public class ChoseDatasetViewModel
    { 
        public static readonly int DataPointsPerPage = 10;

        public IEnumerable<UserDataset> Datasets { get; } = null;

        public ChoseDatasetViewModel(IEnumerable<UserDataset> usersDatasets)
        {
            this.Datasets = usersDatasets;
        }
    }
}
