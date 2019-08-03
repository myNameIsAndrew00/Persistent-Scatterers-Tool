using MapWebSite.Model;


namespace MapWebSite.Core.Database
{
    public interface IUserRepository
    {
        bool InsertUser(User user);

        bool CheckUser(string username, string password);

        int CreateUserPointsDataset(string username, string datasetName);
    }
}
