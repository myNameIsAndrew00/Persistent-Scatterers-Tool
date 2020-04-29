using MapWebSite.HtmlHelpers;
using MapWebSite.Domain;
using MapWebSite.Model;
 using MapWebSite.Core;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;
using MapWebSite.Types;
using System.Net.Mime;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace MapWebSite.Controllers
{
    /// <summary>
    /// Use this ApiController to return pages for the settings layer and to interact with it
    /// </summary> 
    [Authorize]
    [Authorize(Roles = "Administrator")]
    public partial class SettingsController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetUsers(int pageIndex, int itemsPerPage)
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();
            IEnumerable<User> users = handler.GetUsers(null, pageIndex, itemsPerPage);
            
            var response = new HttpResponseMessage();
            response.Content = new StringContent(users.JSONSerialize());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
        }

        [HttpGet]
        public HttpResponseMessage GetUserDatasets(string username, int pageIndex, int itemsPerPage)
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();
            var datasets = handler.GetDataSets(username, null, pageIndex, itemsPerPage)?.Select(dataset => new
            {
                dataset.Name,
                dataset.ID,
                dataset.Username,
                Status = dataset.Status.GetEnumString(),
                dataset.IsValid,
                Source = dataset.PointsSource.ToString()
            }); ;

            var response = new HttpResponseMessage();
            response.Content = new StringContent(datasets.JSONSerialize());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
        }

        [HttpGet]
        public HttpResponseMessage GetUserAssociatedDatasetsCount(string username)
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();

            var datasetsCount = new { count = handler.GetUsersAssociatedDatasetsCount(username) };

            var response = new HttpResponseMessage();
            response.Content = new StringContent(datasetsCount.JSONSerialize());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
        }

        /// <summary>
        /// Associate a dataset with a user
        /// </summary>
        /// <param name="data">Json object which contains 'username', 'datasetUser', 'datasetName'</param>
        /// <returns></returns>
        [HttpPost]        
        public HttpResponseMessage AddDatasetToUser([FromBody] JObject data)
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();

            return handler.ChangeUserAssociatedDataset(
                        data["username"].ToString(),
                        data["datasetName"].ToString(),
                        data["datasetUser"].ToString(),
                        true) ?
                new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK }
                :
                new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.InternalServerError };
            //todo: add a better message here
        }

        [HttpPost]
        public HttpResponseMessage RemoveDatasetFromUser([FromBody] JObject data)
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();
           
            return handler.ChangeUserAssociatedDataset(
                        data["username"].ToString(),
                        data["datasetName"].ToString(),
                        data["datasetUser"].ToString(),
                        false) ?
                new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK }
                :
                new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.InternalServerError };
        }

    }
}
