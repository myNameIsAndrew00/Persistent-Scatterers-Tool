using MapWebSite.Model;
 
namespace MapWebSite.Core.Database
{
    public interface IDataPointsRepository
    {
        bool InsertPointsDataset(PointsDataSet pointsDataset, string username);
    }
}
