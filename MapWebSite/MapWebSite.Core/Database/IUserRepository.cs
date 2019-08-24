using MapWebSite.Model;
using System.Collections.Generic;

namespace MapWebSite.Core.Database
{
    public interface IUserRepository
    {
        bool InsertUser(User user);

        bool CheckUser(string username, string password);

        int CreateUserPointsDataset(string username, string datasetName);

        bool CreateColorMap(string username, ColorMap colorMap);

        IEnumerable<string> GetColorMaps(string username);

        int GetDatasetID(string username, string datasetName);
    }
}
