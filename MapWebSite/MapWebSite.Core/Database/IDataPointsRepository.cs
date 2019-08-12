using MapWebSite.Model;
using System.Threading.Tasks;

namespace MapWebSite.Core.Database
{
    public interface IDataPointsRepository
    {
        Task<bool> InsertPointsDataset(PointsDataSet pointsDataset);


    }
}
